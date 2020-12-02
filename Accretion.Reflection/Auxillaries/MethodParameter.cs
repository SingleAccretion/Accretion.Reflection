using System;
using System.Reflection;

namespace Accretion.Reflection
{
    internal readonly struct MethodParameter
    {
        public MethodParameter(ParameterInfo parameter) :
            this(parameter.ParameterType, parameter.IsOut, parameter.IsIn, parameter.HasDefaultValue, parameter.IsOptional, parameter.DefaultValue) { }

        public MethodParameter(Type type, bool isOut, bool isIn, bool hasDefaultValue, bool isOptional, object? defaultValue)
        {
            Type = type;
            IsOut = isOut;
            IsIn = isIn;
            HasDefaultValue = hasDefaultValue;
            IsOptional = isOptional;
            DefaultValue = defaultValue;
        }

        public Type Type { get; }
        public object? DefaultValue { get; }
        public bool HasDefaultValue { get; }
        public bool IsOptional { get; }
        public bool IsOut { get; }
        public bool IsIn { get; }

        public override string ToString() => $"{(IsIn ? "in" : IsOut ? "out" : Type.IsByRef ? "ref" : "")} {Type}";
    }
}
