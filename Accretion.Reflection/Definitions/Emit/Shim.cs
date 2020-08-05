using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GrEmit;

namespace Accretion.Reflection.Emit
{
    public static class Shim
    {
        /// <summary>
        /// Creates a dynamic method with the signature of the target method that calls the source method.
        /// Parameters to it are emitted with calls to <see cref="Emitter.EmitParameterLoad(GroboIL, ParameterInfo)"/> for each of the source's parameters.
        /// </summary>
        /// <param name="target">The target method. Cannot be <see cref="null"/>.</param>
        /// <param name="source">The source method. Must have a return type that is compatible with the target's return type. Cannot be <see cref="null"/>.</param>
        /// <param name="emitter">The emitter. Cannot be <see cref="null"/>.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public static DynamicMethod Create(MethodInfo target, MethodInfo source, Emitter emitter)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (emitter is null)
            {
                throw new ArgumentNullException(nameof(emitter));
            }

            if (!target.ReturnType.IsAssignableFrom(source.ReturnType) && target.ReturnType != typeof(void))
            {
                throw new ArgumentException($"Target's return type of {target.ReturnType} is not compatible with the source's return type of {source.ReturnType}.");
            }

            var shim = new DynamicMethod(Guid.NewGuid().ToString(), target.ReturnType, target.GetParameters().Select(x => x.ParameterType).ToArray());
            using (var il = new GroboIL(shim))
            {
                foreach (var parameter in source.GetParameters())
                {
                    emitter.EmitParameterLoad(il, parameter);
                }

                il.Call(source);
                il.Ret();
            }

            return shim;
        }
    }
}
