using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GrEmit;

namespace Accretion.Intervals
{
    internal static class ShimGenerator
    {
        public static TTarget WithDefaultParametersPassed<TTarget>(MethodInfo sourceMethod) where TTarget : Delegate => WithDefaultParametersPassed<TTarget>(sourceMethod, ValueEmitter.Default);

        public static TTarget WithDefaultParametersPassed<TTarget>(MethodInfo sourceMethod, ValueEmitter emitter) where TTarget : Delegate
        {
            var targetMethod = typeof(TTarget).GetMethod("Invoke");
            if (targetMethod.ReturnType != sourceMethod.ReturnType)
            {
                throw new ArgumentException($"Return type must be the same for the source and the target.");
            }

            var sourceParameters = sourceMethod.GetParameters();
            var targetParameters = targetMethod.GetParameters();

            //Fast path for cases where we do not need to create a shim
            if (!sourceParameters.Any(x => x.HasDefaultValue))
            {
                return (TTarget)Delegate.CreateDelegate(typeof(TTarget), sourceMethod);
            }

            var shim = new DynamicMethod(Guid.NewGuid().ToString(), targetMethod.ReturnType, targetMethod.GetParameters().Select(x => x.ParameterType).ToArray());
            using (var il = new GroboIL(shim))
            {
                var j = 0;
                for (int i = 0; i < sourceParameters.Length; i++)
                {
                    var sourceParameter = sourceParameters[i];
                    if (sourceParameter.HasDefaultValue)
                    {
                        emitter.EmitDefaultParameterValue(il, sourceParameter);
                    }
                    else
                    {
                        if (targetParameters[j].ParameterType == sourceParameter.ParameterType)
                        {
                            il.Ldarg(j);
                            j++;
                        }
                        else
                        {
                            throw new ArgumentException($"The type of the required source parameter {sourceParameter} does not match that of the target {targetParameters[j]}.");
                        }
                    }
                }

                if (j != targetParameters.Length)
                {
                    throw new ArgumentException("The parameters of target must be exactly the required parameters of source.");
                }

                il.Call(sourceMethod);
                il.Ret();
            }

            return (TTarget)shim.CreateDelegate(typeof(TTarget));
        }

        private static DynamicMethod Create(Type returnType, IEnumerable<Parameter> parameters, MethodInfo sourceMethod)
        {
            var requiredParameterTypes = parameters.Where(x => !x.IsDefaulted).Select(x => x.Type).ToArray();

            var shim = new DynamicMethod(Guid.NewGuid().ToString(), returnType, requiredParameterTypes);

            using (var il = new GroboIL(shim))
            {
                foreach (var parameter in parameters)
                {
                    parameter.Emit(il);
                }

                il.Call(sourceMethod);
                il.Ret();
            }

            return shim;
        }
    }
}
