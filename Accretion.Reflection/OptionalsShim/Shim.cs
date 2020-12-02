using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static System.Reflection.Emit.OpCodes;

namespace Accretion.Reflection.Emit
{
    public static class Shim
    {
        /// <summary>
        /// Dynamically creates a delegate that calls the source method with optional parameters set to their default values.
        /// Instance methods are converted to static methods with the first parameter representing <see langword="this" />.
        /// For value types, it must be passed by <see langword="ref" />.
        /// </summary>
        /// <param name="source"> The source method. Must have a signature that is compatible with the target's. Cannot be <see langword="null" />. </param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public static TTarget Create<TTarget>(MethodInfo source) where TTarget : Delegate
        {
            Verify.IsNotNull(source, nameof(source));

            var invokeMethod = typeof(TTarget).GetMethod("Invoke");
            var targetMethod = new MethodSignature(invokeMethod.GetParameters(), invokeMethod.ReturnType);
            var sourceMethod = new MethodSignature(source);

            return (TTarget)CreateMethodShim(targetMethod, sourceMethod, source).CreateDelegate(typeof(TTarget));
        }

        /// <summary>
        /// Dynamically creates a delegate representing a constructor that calls the source constructor with optional parameters set to their default values.
        /// Initializer constructors are converted to static methods with the first parameter representing <see langword="this" />.
        /// For value types, it must be passed by <see langword="ref" />.
        /// </summary>
        /// <param name="source"> The source method. Must have a signature that is compatible with the target's. Cannot be <see cref="null"/>. </param>
        /// <param name="kind"> The kind of delegate returned. </param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public static TTarget Create<TTarget>(ConstructorInfo source, ConstructorDelegateKind kind) where TTarget : Delegate
        {
            Verify.IsNotNull(source, nameof(source));
            Verify.IsValidEnum(kind, nameof(kind));

            var invokeMethod = typeof(TTarget).GetMethod("Invoke");
            var target = new MethodSignature(invokeMethod.GetParameters(), invokeMethod.ReturnType);
            var shim = kind == ConstructorDelegateKind.Initializer ?
                CreateMethodShim(target, new MethodSignature(source), source) :
                CreateConstructorFactoryShim(target, source);

            return (TTarget)shim.CreateDelegate(typeof(TTarget));
        }

        /// <summary>
        /// Emits a load of the provided optional parameter's default value to the provided IL stream.
        /// Optional parameters without default values are treated as if they had a default value of <see langword="default" />.
        /// </summary>
        /// <param name="il"> IL stream that the generated load will be emitted to. </param>
        /// <param name="parameter"> Optional parameter to emit. Cannot be <see langword="null" />. </param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public static void EmitOptionalParameterLoad(ILGenerator il, ParameterInfo parameter)
        {
            Verify.IsNotNull(il, nameof(il));
            Verify.IsNotNull(parameter, nameof(parameter));
            Verify.IsTrue(parameter.HasDefaultValue || parameter.IsOptional, "Emitting a load of a required parameter is not possible.", nameof(parameter));

            EmitOptionalParameterLoad(il, new MethodParameter(parameter));
        }

        private static DynamicMethod CreateMethodShim(MethodSignature target, MethodSignature source, MethodBase sourceToken)
        {
            Verify.IsTrue(target.ReturnType.IsAssignableFrom(source.ReturnType) || target.ReturnType == typeof(void),
                $"Target's return type of {target.ReturnType} is not compatible with the source's return type of {source.ReturnType}.");

            var sourceParameters = source.Parameters;
            var targetParameters = target.Parameters;
            Verify.IsTrue(targetParameters.Select(x => x.Type).SequenceEqual(sourceParameters.Where(x => !(x.IsOptional || x.HasDefaultValue)).Select(x => x.Type)),
                $"The target's parameters do not exactly match required parameters of the source.");

            var shim = CreateDynamicMethod(target);
            var il = shim.GetILGenerator();

            EmitParameterLoads(il, sourceParameters);
            if (sourceToken is MethodInfo methodInfo)
            {
                il.Emit(Call, methodInfo);
            }
            else
            {
                il.Emit(Call, (ConstructorInfo)sourceToken);
            }

            if (target.ReturnType == typeof(void) && source.ReturnType != typeof(void))
            {
                il.Emit(Pop);
            }
            il.Emit(Ret);

            return shim;
        }

        private static DynamicMethod CreateConstructorFactoryShim(MethodSignature factory, ConstructorInfo source)
        {
            Verify.IsTrue(factory.ReturnType.IsAssignableFrom(source.DeclaringType),
                $"The factory must have a return type compatible with {source.DeclaringType}.");

            var sourceParameters = source.GetParameters();
            var targetParameters = factory.Parameters;
            Verify.IsTrue(targetParameters.Select(x => x.Type).SequenceEqual(sourceParameters.Where(x => !(x.IsOptional || x.HasDefaultValue)).Select(x => x.ParameterType)),
                $"The factory's parameters must exactly match required parameters of the source.");

            var shim = CreateDynamicMethod(factory);
            var il = shim.GetILGenerator();

            EmitParameterLoads(il, sourceParameters.Select(x => new MethodParameter(x)).ToArray());
            il.Emit(Newobj, source);
            il.Emit(Ret);

            return shim;
        }

        private static void EmitParameterLoads(ILGenerator il, MethodParameter[] parameters)
        {
            var targetParameterPosition = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.HasDefaultValue || parameter.IsOptional)
                {
                    EmitOptionalParameterLoad(il, parameter);
                }
                else
                {
                    il.Emit(Ldarg, targetParameterPosition);
                    targetParameterPosition++;
                }
            }
        }

        private static void EmitOptionalParameterLoad(ILGenerator il, MethodParameter parameter)
        {
            var type = parameter.Type;
            var defaultValue = (parameter.IsOptional && !parameter.HasDefaultValue) ? null : parameter.DefaultValue;

            // ByRefLike types cannot be boxed which means that as of July 2020 "value" cannot be a ByRefLike and not null
            // This may change in the future with some updates to the reflection stack, but we're in the clear with objects for now
            if (defaultValue is null)
            {
                EmitDefaultValueLoad(il, type);
            }
            else
            {
                // We pretend all pointers "are" of type UIntPtr so that the numeric value can be boxed
                // Likewise, nullables are converted to BoxedNullable and byrefs (for "in" parameters) to BoxedByRef
                var realDefaultValue = As(type, defaultValue);
                if (realDefaultValue is null)
                {
                    throw new InvalidProgramException($"Could not convert default parameter value {defaultValue} of type {defaultValue.GetType()} to the parameter's type {type}.");
                }

                EmitValueLoad(il, realDefaultValue);
            }
        }

        private static void EmitDefaultValueLoad(ILGenerator il, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(Ldc_I4_0);
                    return;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(Ldc_I8, 0L);
                    return;
                case TypeCode.Single:
                    il.Emit(Ldc_R4, 0f);
                    return;
                case TypeCode.Double:
                    il.Emit(Ldc_R8, 0d);
                    return;
            }

            if (type.IsPointer || type == typeof(UIntPtr))
            {
                il.Emit(Ldc_I4_0);
                il.Emit(Conv_U);
            }
            else if (type == typeof(IntPtr))
            {
                il.Emit(Ldc_I4_0);
                il.Emit(Conv_I);
            }
            else if (type.IsEnum)
            {
                EmitDefaultValueLoad(il, Enum.GetUnderlyingType(type));
            }
            else if (type.IsValueType)
            {
                var local = il.DeclareLocal(type);
                il.Emit(Ldloca, local);
                il.Emit(Initobj, type);
                il.Emit(Ldloc, local);
            }
            else if (type.IsByRef)
            {
                var elementType = type.GetElementType();
                EmitDefaultValueLoad(il, elementType);
                var local = il.DeclareLocal(elementType);
                il.Emit(Stloc, local);
                il.Emit(Ldloca, local);
            }
            else
            {
                il.Emit(Ldnull);
            }
        }

        private static unsafe void EmitValueLoad(ILGenerator il, object value)
        {
            // value.GetType() is the "real" type, only all pointers are UIntPtr and nullables are BoxedNullable
            // It can be:
            // Boolean, Char, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal, DateTime, Enum, IntPtr, UIntPtr
            // Or their nullable equivalents
            // Or string
            // Or byref of any of the above types
            switch (value)
            {
                case bool boolean: il.Emit(boolean ? Ldc_I4_1 : Ldc_I4_0); break;
                case char character: il.Emit(Ldc_I4, character); break;
                case sbyte int8: il.Emit(Ldc_I4, (int)int8); break;
                case byte uint8: il.Emit(Ldc_I4, (int)uint8); break;
                case short int16: il.Emit(Ldc_I4, (int)int16); break;
                case ushort uint16: il.Emit(Ldc_I4, uint16); break;
                case int int32: il.Emit(Ldc_I4, int32); break;
                case uint uint32: il.Emit(Ldc_I4, (int)uint32); break;
                case long int64: il.Emit(Ldc_I8, int64); break;
                case ulong uint64: il.Emit(Ldc_I8, (long)uint64); break;
                case float float32: il.Emit(Ldc_R4, float32); break;
                case double float64: il.Emit(Ldc_R8, float64); break;
                case decimal decimal128:
                    var decimalPtr = (long*)&decimal128;
                    var low = decimalPtr[0];
                    var high = decimalPtr[1];

                    var decimalLocal = il.DeclareLocal(typeof(decimal));
                    il.Emit(Ldloca, decimalLocal);

                    il.Emit(Dup);
                    il.Emit(Ldc_I8, low);
                    il.Emit(Stind_I8);

                    il.Emit(Ldc_I4, sizeof(long));
                    il.Emit(Conv_I);
                    il.Emit(Add);
                    il.Emit(Ldc_I8, high);
                    il.Emit(Stind_I8);

                    il.Emit(Ldloc, decimalLocal);
                    break;
                case DateTime dateTime:
                    var local = il.DeclareLocal(typeof(DateTime));
                    il.Emit(Ldloca, local);
                    il.Emit(Ldc_I8, dateTime.Ticks);
                    il.Emit(Ldc_I4, (int)dateTime.Kind);
                    il.Emit(Call, typeof(DateTime).GetConstructor(new[] { typeof(long), typeof(DateTimeKind) }));
                    il.Emit(Ldloc, local);
                    break;
                case nuint nuint:
                    il.Emit(Ldc_I4, (int)nuint);
                    il.Emit(Conv_U);
                    break;
                case nint nint:
                    il.Emit(Ldc_I4, (int)nint);
                    il.Emit(Conv_I);
                    break;
                case Enum enumeration:
                    var underlyingType = Enum.GetUnderlyingType(enumeration.GetType());
                    var underlyingValue = Convert.ChangeType(enumeration, underlyingType);
                    EmitValueLoad(il, underlyingValue);
                    break;
                case BoxedNullable boxedNullable:
                    EmitValueLoad(il, boxedNullable.UnderlyingValue);
                    il.Emit(Newobj, boxedNullable.NullableType.GetConstructor(new[] { boxedNullable.UnderlyingType }));
                    break;
                case string str: il.Emit(Ldstr, str); break;
                case BoxedByRef byRef:
                    var elementType = byRef.ElementType;
                    local = il.DeclareLocal(elementType);
                    EmitValueLoad(il, byRef.RawValue);
                    il.Emit(Stloc, local);
                    il.Emit(Ldloca, local);
                    break;
                default: throw new ArgumentException($"Emitting the load of value {value} of type {value.GetType()} is not supported.");
            }
        }

        private static object? As(Type targetType, object value)
        {
            // Value can be of type:
            // Boolean, Char, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double - as per ECMA 335 II.22.9
            // Or Enum
            // Or Decimal, DateTime - as per custom attributes supported by the reflection stack
            // Or byref variants of the above
            // Please note that this code cannot be complete since the value can theoretically be anything defined by a user with some CustomConstantAttribute
            // ParameterInfo.DefaultValue will return the raw value defined in metadata/attributes unless:
            // ParameterInfo.ParameterType is DataTime or Decimal and it is decorated with the relevant attributes
            // Or ParameterInfo.ParameterInfo is enum

            // Apparently, the type code for enums returns the type code for the underlying type, so we have to dodge that bullet
            var targetTypeCode = !targetType.IsEnum ? Type.GetTypeCode(targetType) : TypeCode.Empty;

            // We choose the "strict" approach to interpreting metadata
            switch (targetTypeCode)
            {
                case TypeCode.Boolean:
                    return value switch
                    {
                        bool boolean => boolean,
                        sbyte int8 and (0 or 1) => int8 == 1,
                        byte uint8 and (0 or 1) => uint8 == 1,
                        short int16 and (0 or 1) => int16 == 1,
                        ushort uint16 and (0 or 1) => uint16 == 1,
                        int int32 and (0 or 1) => int32 == 1,
                        uint uint32 and (0 or 1) => uint32 == 1,
                        long int64 and (0 or 1) => int64 == 1,
                        ulong uint64 and (0 or 1) => uint64 == 1,
                        _ => null
                    };
                case TypeCode.SByte:
                    return value switch
                    {
                        sbyte int8 => int8,
                        byte uint8 when uint8 <= sbyte.MaxValue => (sbyte)uint8,
                        short int16 and >= sbyte.MinValue and <= sbyte.MaxValue => (sbyte)int16,
                        ushort uint16 when uint16 <= sbyte.MaxValue => (sbyte)uint16,
                        int int32 and >= sbyte.MinValue and <= sbyte.MaxValue => (sbyte)int32,
                        uint uint32 when uint32 <= sbyte.MaxValue => (sbyte)uint32,
                        long int64 and >= sbyte.MinValue and <= sbyte.MaxValue => (sbyte)int64,
                        ulong uint64 when uint64 <= (long)sbyte.MaxValue => (sbyte)uint64,
                        _ => null
                    };
                case TypeCode.Byte:
                    return value switch
                    {
                        sbyte int8 and >= 0 => (byte)int8,
                        byte uint8 => uint8,
                        short int16 and >= 0 and <= byte.MaxValue => (byte)int16,
                        ushort uint16 and <= byte.MaxValue => (byte)uint16,
                        int int32 and >= 0 and <= byte.MaxValue => (byte)int32,
                        uint uint32 and <= byte.MaxValue => (byte)uint32,
                        long int64 and >= 0 and <= byte.MaxValue => (byte)int64,
                        ulong uint64 and <= byte.MaxValue => (byte)uint64,
                        _ => null
                    };
                case TypeCode.Char:
                    return value switch
                    {
                        char character => character,
                        sbyte int8 and >= 0 => (char)int8,
                        byte uint8 => (char)uint8,
                        short int16 and >= 0 => (char)int16,
                        ushort uint16 => (char)uint16,
                        int int32 and >= char.MinValue and <= char.MaxValue => (char)int32,
                        uint uint32 and <= char.MaxValue => (char)uint32,
                        long int64 and >= char.MinValue and <= char.MaxValue => (char)int64,
                        ulong uint64 and <= char.MaxValue => (char)uint64,
                        _ => null
                    };
                case TypeCode.Int16:
                    return value switch
                    {
                        sbyte int8 => (short)int8,
                        byte uint8 => (short)uint8,
                        short int16 => int16,
                        ushort uint16 when uint16 <= short.MaxValue => (short)uint16,
                        int int32 and >= short.MinValue and <= short.MaxValue => (short)int32,
                        uint uint32 when uint32 <= short.MaxValue => (short)uint32,
                        long int64 and >= short.MinValue and <= short.MaxValue => (short)int64,
                        ulong uint64 when uint64 <= (long)short.MaxValue => (short)uint64,
                        _ => null
                    };
                case TypeCode.UInt16:
                    return value switch
                    {
                        sbyte int8 when int8 >= 0 => (ushort)int8,
                        byte uint8 => (ushort)uint8,
                        short int16 when int16 >= 0 => (ushort)int16,
                        ushort uint16 => uint16,
                        int int32 and >= 0 and <= ushort.MaxValue => (ushort)int32,
                        uint uint32 and <= ushort.MaxValue => (ushort)uint32,
                        long int64 and >= 0 and <= ushort.MaxValue => (ushort)int64,
                        ulong uint64 and <= ushort.MaxValue => (ushort)uint64,
                        _ => null
                    };
                case TypeCode.Int32:
                    return value switch
                    {
                        sbyte int8 => (int)int8,
                        byte uint8 => (int)uint8,
                        short int16 => (int)int16,
                        ushort uint16 => (int)uint16,
                        int int32 => int32,
                        uint uint32 and <= int.MaxValue => (int)uint32,
                        long int64 and >= int.MinValue and <= int.MaxValue => (int)int64,
                        ulong uint64 and <= int.MaxValue => (int)uint64,
                        _ => null
                    };
                case TypeCode.UInt32:
                    return value switch
                    {
                        sbyte int8 and >= 0 => (uint)int8,
                        byte uint8 => (uint)uint8,
                        short int16 and >= 0 => (uint)int16,
                        ushort uint16 => (uint)uint16,
                        int int32 and >= 0 => (uint)int32,
                        uint uint32 => uint32,
                        long int64 and >= 0 and <= uint.MaxValue => (uint)int64,
                        ulong uint64 and <= uint.MaxValue => (uint)uint64,
                        _ => null
                    };
                case TypeCode.Int64:
                    return value switch
                    {
                        sbyte int8 => (long)int8,
                        byte uint8 => (long)uint8,
                        short int16 => (long)int16,
                        ushort uint16 => (long)uint16,
                        int int32 => (long)int32,
                        uint uint32 => (long)uint32,
                        long int64 => int64,
                        ulong uint64 when uint64 <= long.MaxValue => uint64,
                        _ => null
                    };
                case TypeCode.UInt64:
                    return value switch
                    {
                        sbyte int8 and >= 0 => (ulong)int8,
                        byte uint8 => (ulong)uint8,
                        short int16 and >= 0 => (ulong)int16,
                        ushort uint16 => (ulong)uint16,
                        int int32 and >= 0 => (ulong)int32,
                        uint uint32 => (ulong)uint32,
                        long int64 and >= 0 => (ulong)int64,
                        ulong uint64 => uint64,
                        _ => null
                    };
                case TypeCode.Single:
                    return value switch
                    {
                        float float32 => float32,
                        double float64 => (float)float64,
                        _ => null
                    };
                case TypeCode.Double: return value as double?;
            }

            // Strictly speaking, if targetTypes is Enum-derived, value will always be strongly typed
            // This code supports the case of Nullable<Enum>, which is reported as the underlying integral type
            if (targetType.IsEnum && targetType != value.GetType())
            {
                var underlyingType = Enum.GetUnderlyingType(targetType);
                var underlyingValue = As(underlyingType, value);

                return underlyingValue is null ? null : Enum.ToObject(targetType, underlyingValue);
            }
            else if (targetType.IsPointer || targetType == typeof(UIntPtr))
            {
                // Native unsigned integer constants can only be represented as integers not wider than UInt32 in portable metadata
                var literal = As(typeof(uint), value) as uint?;
                return literal is null ? null : new UIntPtr(literal.Value);
            }
            else if (targetType == typeof(IntPtr))
            {
                var literal = As(typeof(int), value) as int?;
                return literal is null ? null : new IntPtr(literal.Value);
            }
            else if (Nullable.GetUnderlyingType(targetType) is Type underlyingType)
            {
                var underlyingValue = As(underlyingType, value);
                // This is a workaround required to preserve the type information of T?, as it would ordinarily be boxed to T
                return underlyingValue is null ? null : new BoxedNullable(targetType, underlyingValue);
            }
            else if (targetType.IsByRef)
            {
                var elementType = targetType.GetElementType();
                var rawValue = As(elementType, value);
                return rawValue is null ? null : new BoxedByRef(elementType, rawValue);
            }
            else
            {
                // This takes care of Decimal, DateTime, Enum and String
                if (targetType == value.GetType())
                {
                    return value;
                }

                return null;
            }
        }

        private static DynamicMethod CreateDynamicMethod(MethodSignature target) => new DynamicMethod(Guid.NewGuid().ToString(), target.ReturnType, target.Parameters.Select(x => x.Type).ToArray());
    }
}
