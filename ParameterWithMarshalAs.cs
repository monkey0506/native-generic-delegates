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

        /// <summary>
        /// Gets the desired marshaling behavior for a parameter.
        /// </summary>
        /// <param name="marshalAs">The desired marshaling behavior.</param>
        /// <returns>
        /// If <paramref name="marshalAs"/> is <see langword="null"/>, <see cref="INativeAction.NoCustomMarshaling"/>, or <see
        /// cref="INativeFunc.NoCustomMarshaling"/>, this returns <see langword="null"/> and no custom marshaling will be applied
        /// to the parameter. Any other value will return the marshaling behavior for the parameter.
        /// </returns>
        private static MarshalAsAttribute? GetMarshalAs(MarshalAsAttribute? marshalAs)
        {
            // NOTE: `INativeAction.NoCustomMarshaling` and `INativeFunc.NoCustomMarshaling` are the same reference.
            return ReferenceEquals(marshalAs, INativeAction.NoCustomMarshaling) ? null : marshalAs;
        }

        public ParameterWithMarshalAs(Type parameterType, MarshalAsAttribute? marshalAs)
        {
            ParameterType = parameterType;
            MarshalAs = GetMarshalAs(marshalAs);
        }

        public ParameterWithMarshalAs(ParameterInfo parameterInfo, MarshalAsAttribute? marshalAs) :
            this
            (
                parameterInfo.ParameterType,
                // If `marshalAs` is `null`, we read marshaling behavior (if any) from the parameter, otherwise just forward it
                // to the `Type`-based constructor. The user must explicitly pass `INativeAction.NoCustomMarshaling` or
                // `INativeFunc.NoCustomMarshaling` if they do not want to inherit the marshaling behavior from the parameter.
                marshalAs is null ?
                    (MarshalAsAttribute?)parameterInfo.GetCustomAttribute(typeof(MarshalAsAttribute)) :
                    marshalAs
            )
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
