using System;
using System.Reflection;
using GrEmit;

namespace Accretion.Intervals
{
    internal readonly struct Parameter
    {
        private readonly object _defaultValue;
        private readonly Action<GroboIL, Parameter> _emitter;

        public Parameter(ParameterInfo parameterInfo) : this(parameterInfo, parameterInfo.HasDefaultValue ? Emitter.ForOptionalParameters : Emitter.ForRequiredParameters) { }

        public Parameter(ParameterInfo parameterInfo, Action<GroboIL, Parameter> emitter) : 
            this(parameterInfo.ParameterType, parameterInfo.Position, parameterInfo.HasDefaultValue, parameterInfo.DefaultValue, emitter) { }

        private Parameter(Type type, int index, bool isDefaulted, object defaultValue, Action<GroboIL, Parameter> emitter)
        {
            Type = type;
            Index = index;
            IsDefaulted = isDefaulted;
            _defaultValue = defaultValue;
            _emitter = emitter;
        }

        public Type Type { get; }
        public int Index { get; }
        public bool IsDefaulted { get; }
        public object DefaultValue => IsDefaulted ? _defaultValue : throw new InvalidOperationException("Required parameters do not have default values.");

        public void Emit(GroboIL il) => _emitter(il, this);
    }
}
