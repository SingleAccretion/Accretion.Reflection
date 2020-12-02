using System;
using System.Linq;
using System.Reflection;

namespace Accretion.Reflection
{
    internal readonly struct MethodSignature
    {
        public MethodSignature(ConstructorInfo methodInfo) : this(GetMethodParameters(methodInfo), typeof(void)) { }
        public MethodSignature(MethodInfo methodInfo) : this(GetMethodParameters(methodInfo), methodInfo.ReturnType) { }

        public MethodSignature(ParameterInfo[] parameters, Type returnType) : this(parameters.Select(x => new MethodParameter(x)).ToArray(), returnType) { }

        private MethodSignature(MethodParameter[] parameters, Type returnType)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }

        public MethodParameter[] Parameters { get; }
        public Type ReturnType { get; }

        public static MethodParameter[] GetMethodParameters(MethodBase methodBase)
        {
            var sourceParameters = methodBase.GetParameters();
            var instanceShift = methodBase.IsStatic ? 0 : 1;
            var parameters = new MethodParameter[sourceParameters.Length + instanceShift];

            if (!methodBase.IsStatic)
            {
                var instanceType = methodBase.DeclaringType;
                instanceType = instanceType.IsValueType ? instanceType.MakeByRefType() : instanceType;
                parameters[0] = new MethodParameter(instanceType, false, false, false, false, null);
            }
            for (int i = instanceShift, j = 0; i < parameters.Length; i++, j++)
            {
                parameters[i] = new MethodParameter(sourceParameters[j]);
            }

            return parameters;
        }
    }
}
