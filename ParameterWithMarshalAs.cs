using System;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NativeGenericDelegates
{
    /// <summary>
    /// Represents a native generic delegate method parameter with optional marshaling behavior.
    /// </summary>
    internal sealed class ParameterWithMarshalAs :
        IEquatable<ParameterWithMarshalAs>,
        IEqualityOperators<ParameterWithMarshalAs, ParameterWithMarshalAs, bool>
    {
        /// <summary>
        /// The type of this parameter.
        /// </summary>
        public Type ParameterType { get; }
        /// <summary>
        /// The optional marshaling behavior of this parameter.
        /// </summary>
        public MarshalAsAttribute? MarshalAs { get; }

        public static bool operator ==(ParameterWithMarshalAs? left, ParameterWithMarshalAs? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(ParameterWithMarshalAs? left, ParameterWithMarshalAs? right) => !(left == right);

        public ParameterWithMarshalAs(Type parameterType, MarshalAsAttribute? marshalAs)
        {
            ParameterType = parameterType;
            MarshalAs = marshalAs;
        }

        public ParameterWithMarshalAs(ParameterInfo parameterInfo) :
            this(parameterInfo.ParameterType, (MarshalAsAttribute?)parameterInfo.GetCustomAttribute(typeof(MarshalAsAttribute)))
        { }

        public override bool Equals(object? obj)
        {
            return obj is ParameterWithMarshalAs other && Equals(other);
        }

        public bool Equals(ParameterWithMarshalAs? other)
        {
            return other is not null && ParameterType == other.ParameterType &&
                (MarshalAs?.Equals(other.MarshalAs) ?? other.MarshalAs is null);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ParameterType, MarshalAs);
        }

        public override string ToString()
        {
            return $"{nameof(ParameterWithMarshalAs)} {{ {nameof(ParameterType)} = {ParameterType.Name},  {nameof(MarshalAs)} " +
                $"= {MarshalAs?.Value.ToString() ?? "None"} }}";
        }
    }
}
