using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Accretion.Reflection.Emit;
using Xunit;
using static ILMethodsWithDefaultParameters;

namespace Accretion.Reflection.Tests
{
    // There are two versions of the tests: in C# and in IL
    // We have to do both because C# semantics do not allow (the compiler does not emit):
    // Non-trailing default parameters
    // Non-null pointers
    // Non-default DateTimes
    // Mixed and matched primitives
    // We have to consider:
    // Classes (except string)
    // Strings (both null and literals)
    // Primitives: Boolean, Char, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double
    // IntPtr, UIntPtr and pointers
    // DateTime, Decimal, Enums
    // Custom value types (including ByRefLike)
    // Custom value types encoded with a custom CustomConstantAttribute
    // Nullables
    // Optional parameters that do not have default values
    public class OptionalsShimTests
    {
        private const string StringConstant = "LiteralString";
        private const double Float64Constant = 64d;
        private const float Float32Contant = 32f;
        private const sbyte Int8Constant = 8;
        private const int Int32Constant = 32;

        [Fact]
        public void ThrowsOnNullInputs()
        {
            var placeholder = GetType().GetMethod(nameof(ThrowsOnNullInputs));
            Assert.Throws<ArgumentNullException>(() => Shim.Create<Action>(null));
            Assert.Throws<ArgumentNullException>(() => Shim.Create<Action>(null, ConstructorDelegateKind.Factory));
        }

        [Fact]
        public void ThrowsOnMismatchedParameterTypes()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithoutOptionalParameters));
            Assert.Throws<ArgumentException>(() => Shim.Create<Func<long, string, int, object, bool>>(source));
            Assert.Throws<ArgumentException>(() => Shim.Create<Func<double, string, int, string, bool>>(source));
            Assert.Throws<ArgumentException>(() => Shim.Create<Func<double, string, uint, object, bool>>(source));
            
            source = typeof(StructWithConstructor).GetMethod(nameof(StructWithConstructor.MutatingInstanceMethod));
            Assert.Throws<ArgumentException>(() => Shim.Create<Action<StructWithConstructor>>(source));
            Assert.Throws<ArgumentException>(() => Shim.Create<Action>(source));
        }

        [Fact]
        public void ThrowsOnMismatchedReturnTypes()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithFloat32ReturnType));
            Assert.Throws<ArgumentException>(() => Shim.Create<Func<int>>(source));
        }

        [Fact]
        public void SupportsOptionalInParameters()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithOptionalInParameters));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void SupportsValidCSharpOptionalParameters()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithAllPossibleCSharpOptionalParameters));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void SupportsDecimalOptionalParameters()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithDecimalOptionalParameters));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void SupportsShimsWithoutOptionalParameters()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithoutOptionalParameters));
            var shim = Shim.Create<Func<double, string, int, object, bool>>(source);
            Assert.True(shim(Float64Constant, StringConstant, Int32Constant, null));
        }

        [Fact]
        public void SupportsPurelyOptionalParameters()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithPurelyOptionalParameters));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void SupportsNonTrailingPurelyOptionalParameters()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWothNonTrailingPurelyOptionalParameters));
            var shim = Shim.Create<Func<int, double, bool>>(source);
            Assert.True(shim(Int32Constant, Float64Constant));
        }

        [Fact]
        public void SupportsNonTrailingOptionalParameters()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(MethodWithNonTrailingOptionalParameters));
            var shim = Shim.Create<Func<float, sbyte, bool>>(source);
            Assert.True(shim(Float32Contant, Int8Constant));
        }

        [Fact]
        public void SupportsInitializerStyleClassConstructors()
        {
            var ctor = typeof(ClassWithConstructor).GetConstructors().First();
            var initter = Shim.Create<Action<ClassWithConstructor>>(ctor, ConstructorDelegateKind.Initializer);
            var rawInstance = (ClassWithConstructor)RuntimeHelpers.GetUninitializedObject(typeof(ClassWithConstructor));
            initter(rawInstance);

            Assert.True(rawInstance.HasBeenConstructedProperly);
        }

        [Fact]
        public void SupportsFactoryStyleClassConstructors()
        {
            var ctor = typeof(ClassWithConstructor).GetConstructors().First();
            var factory = Shim.Create<Func<ClassWithConstructor>>(ctor, ConstructorDelegateKind.Factory);

            Assert.True(factory().HasBeenConstructedProperly);
        }

        [Fact]
        public void SupportsInitializerStyleStructConstructors()
        {
            var ctor = typeof(StructWithConstructor).GetConstructors().First();
            var initter = Shim.Create<RefAction<StructWithConstructor>>(ctor, ConstructorDelegateKind.Initializer);
            var instance = default(StructWithConstructor);
            initter(ref instance);

            Assert.True(instance.HasBeenConstructedProperly);
        }

        [Fact]
        public void SupportsFactoryStyleStructConstructors()
        {
            var ctor = typeof(StructWithConstructor).GetConstructors().First();
            var factory = Shim.Create<Func<StructWithConstructor>>(ctor, ConstructorDelegateKind.Factory);

            Assert.True(factory().HasBeenConstructedProperly);
        }

        [Fact]
        public void SupportsClassInstanceMethods()
        {
            var source = typeof(ClassWithConstructor).GetMethod(nameof(ClassWithConstructor.PureInstanceMethod));
            var shim = Shim.Create<Func<ClassWithConstructor, bool, bool>>(source);
            var instance = new ClassWithConstructor();
            Assert.True(shim(instance, true));
        }

        [Fact]
        public void SupportsStructInstanceMethods()
        {
            var instance = new StructWithConstructor();
            
            var pureSource = typeof(StructWithConstructor).GetMethod(nameof(StructWithConstructor.PureInstanceMethod));
            var pureShim = Shim.Create<RefFunc<StructWithConstructor, bool, bool>>(pureSource);
            Assert.True(pureShim(ref instance, true));

            var mutatingSource = typeof(StructWithConstructor).GetMethod(nameof(StructWithConstructor.MutatingInstanceMethod));
            var mutatingShim = Shim.Create<RefAction<StructWithConstructor>>(mutatingSource);
            mutatingShim(ref instance);
            Assert.True(instance.HasBeenMutated);
        }

        [Fact]
        public void SupportsDiscardingReturnValues()
        {
            var source = typeof(OptionalsShimTests).GetMethod(nameof(MethodWithStringReturnType));
            var shim = Shim.Create<RefAction<bool>>(source);
            var success = false;
            shim(ref success);

            Assert.True(success);
        }

        public static bool MethodWithoutOptionalParameters(double float64, string str, int int32, object @object) =>
            float64 == Float64Constant &&
            str == StringConstant &&
            int32 == Int32Constant &&
            @object == null;

        public static bool MethodWithOptionalInParameters(
            [Optional] in long int64,
            in int int32 = Int32Constant,
            in double float64 = Float64Constant,
            in object obj = null,
            in Guid guid = default) =>
            int64 == 0 &&
            int32 == Int32Constant &&
            float64 == Float64Constant &&
            obj == null &&
            guid.Equals(default);

        public static bool MethodWithPurelyOptionalParameters([Optional] int int32, [Optional] decimal decimal128, [Optional] string str) =>
            int32 == 0 &&
            decimal128 == 0 &&
            str == null;

        public static bool MethodWothNonTrailingPurelyOptionalParameters(int int32, [Optional] float float32, double float64) =>
            int32 == Int32Constant &&
            float32 == 0 &&
            float64 == Float64Constant;

        public static bool MethodWithDecimalOptionalParameters(decimal decimalFraction = 12.2m, decimal decimalWhole = 10m, decimal decimalLarge = 30000000000m) =>
            decimalFraction == 12.2m &&
            decimalWhole == 10m &&
            decimalLarge == 30000000000m;

        public static unsafe bool MethodWithAllPossibleCSharpOptionalParameters(
            object @object = null,
            string nullString = null,
            string literalString = StringConstant,
            bool boolean = true,
            char character = 'c',
            sbyte int8 = Int8Constant,
            byte uint8 = 8,
            short int16 = 16,
            ushort uint16 = 16,
            int int32 = Int32Constant,
            uint uint32 = Int32Constant,
            long int64 = 64,
            ulong uint64 = 64,
            IntPtr zeroIntPtr = default,
            UIntPtr zeroUIntPtr = default,
            void* nullPtr = null,
            DateTime defaultDateTime = default,
            decimal decimal128 = 128m,
            TypeCode enumeration = TypeCode.Int32,
            BindingFlags flagsEnumeration = BindingFlags.Instance | BindingFlags.Public,
            float float32 = Float32Contant,
            double float64 = Float64Constant,
            Guid defaultCustomStruct = default,
            ReadOnlySpan<char> defaultCustomByRefLike = default,
            bool? nonNullNullableBoolean = true,
            char? nonNullNullableCharacter = 'c',
            sbyte? nonNullNullableInt8 = Int8Constant,
            byte? nonNullNullableUint8 = 8,
            short? nonNullNullableInt16 = 16,
            ushort? nonNullNullableUint16 = 16,
            int? nonNullNullableInt32 = Int32Constant,
            uint? nonNullNullableUint32 = Int32Constant,
            long? nonNullNullableInt64 = 64,
            ulong? nonNullNullableUint64 = 64,
            decimal? nonNullNullableDecimal128 = 128m,
            TypeCode? nonNullNullableEnumeration = TypeCode.Int32,
            BindingFlags? nonNullNullableFlagsEnumeration = BindingFlags.Instance | BindingFlags.Public,
            float? nonNullNullableFloat32 = Float32Contant,
            double? nonNullNullableFloat64 = Float64Constant,
            bool? nullNullableBoolean = null,
            char? nullNullableCharacter = null,
            sbyte? nullNullableInt8 = null,
            byte? nullNullableUint8 = null,
            short? nullNullableInt16 = null,
            ushort? nullNullableUint16 = null,
            int? nullNullableInt32 = null,
            uint? nullNullableUint32 = null,
            long? nullNullableInt64 = null,
            ulong? nullNullableUint64 = null,
            IntPtr? nullNullableZeroIntPtr = null,
            UIntPtr? nullNullableZeroUIntPtr = null,
            DateTime? nullNullableDefaultDateTime = null,
            decimal? nullNullableDecimal128 = null,
            TypeCode? nullNullableEnumeration = null,
            BindingFlags? nullNullableFlagsEnumeration = null,
            float? nullNullableFloat32 = null,
            double? nullNullableFloat64 = null,
            Guid? nullNullableDefaultCustomStruct = null) =>
            @object == null &&
            nullString == null &&
            literalString == StringConstant &&
            boolean == true &&
            character == 'c' &&
            int8 == Int8Constant &&
            uint8 == 8 &&
            int16 == 16 &&
            uint16 == 16 &&
            int32 == Int32Constant &&
            uint32 == Int32Constant &&
            int64 == 64 &&
            uint64 == 64 &&
            zeroIntPtr == default &&
            zeroUIntPtr == default &&
            nullPtr == null &&
            defaultDateTime == default &&
            decimal128 == 128m &&
            enumeration == TypeCode.Int32 &&
            flagsEnumeration == (BindingFlags.Instance | BindingFlags.Public) &&
            float32 == Float32Contant &&
            float64 == Float64Constant &&
            defaultCustomStruct == default &&
            defaultCustomByRefLike == default &&
            nonNullNullableBoolean == true &&
            nonNullNullableCharacter == 'c' &&
            nonNullNullableInt8 == Int8Constant &&
            nonNullNullableUint8 == 8 &&
            nonNullNullableInt16 == 16 &&
            nonNullNullableUint16 == 16 &&
            nonNullNullableInt32 == Int32Constant &&
            nonNullNullableUint32 == Int32Constant &&
            nonNullNullableInt64 == 64 &&
            nonNullNullableUint64 == 64 &&
            nonNullNullableDecimal128 == 128m &&
            nonNullNullableEnumeration == TypeCode.Int32 &&
            nonNullNullableFlagsEnumeration == (BindingFlags.Instance | BindingFlags.Public) &&
            nonNullNullableFloat32 == Float32Contant &&
            nonNullNullableFloat64 == Float64Constant &&
            nullNullableBoolean == null &&
            nullNullableCharacter == null &&
            nullNullableInt8 == null &&
            nullNullableUint8 == null &&
            nullNullableInt16 == null &&
            nullNullableUint16 == null &&
            nullNullableInt32 == null &&
            nullNullableUint32 == null &&
            nullNullableInt64 == null &&
            nullNullableUint64 == null &&
            nullNullableZeroIntPtr == null &&
            nullNullableZeroUIntPtr == null &&
            nullNullableDefaultDateTime == null &&
            nullNullableDecimal128 == null &&
            nullNullableEnumeration == null &&
            nullNullableFlagsEnumeration == null &&
            nullNullableFloat32 == null &&
            nullNullableFloat64 == null &&
            nullNullableDefaultCustomStruct == null;

        public static float MethodWithFloat32ReturnType() => Float32Contant;

        public static string MethodWithStringReturnType(out bool hasRun)
        {
            hasRun = true;
            return string.Empty;
        }

        private class ClassWithConstructor
        {
            public ClassWithConstructor(object obj = null, Guid guid = default, float float32 = Float32Contant, int int32 = Int32Constant) =>
                HasBeenConstructedProperly = obj == null &&
                                        guid.Equals(default) &&
                                        float32 == Float32Contant &&
                                        int32 == Int32Constant;

            public bool HasBeenConstructedProperly { get; }

            public bool PureInstanceMethod(bool boolean, string str = StringConstant, float float32 = Float32Contant, DateTime dateTime = default) =>
                boolean &&
                str == StringConstant &&
                float32 == Float32Contant &&
                dateTime == default;
        }

        private struct StructWithConstructor
        {
            public StructWithConstructor(object obj = null, Guid guid = default, float float32 = Float32Contant, int int32 = Int32Constant) : this() =>
                HasBeenConstructedProperly = obj == null &&
                                        guid.Equals(default) &&
                                        float32 == Float32Contant &&
                                        int32 == Int32Constant;

            public bool HasBeenConstructedProperly { get; }
            public bool HasBeenMutated { get; set; }

            public bool PureInstanceMethod(bool boolean, string str = StringConstant, float float32 = Float32Contant, DateTime dateTime = default) =>
                boolean &&
                str == StringConstant &&
                float32 == Float32Contant &&
                dateTime == default;

            public void MutatingInstanceMethod() => HasBeenMutated = true;
        }
    }
}