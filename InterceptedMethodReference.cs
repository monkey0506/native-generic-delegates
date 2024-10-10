using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class InterceptedMethodReference : IEquatable<InterceptedMethodReference>
    {
        private readonly int hashCode;

        public ClosedGenericInterceptor Interceptor { get; }
        public MethodReference MethodReference { get; }

        public static bool operator ==(InterceptedMethodReference? left, InterceptedMethodReference? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(InterceptedMethodReference? left, InterceptedMethodReference? right) =>
            !(left == right);

        public InterceptedMethodReference(MethodReference methodReference, ClosedGenericInterceptor interceptor)
        {
            Interceptor = interceptor;
            MethodReference = methodReference;
            hashCode = Hash.Combine(MethodReference.Method, MethodReference.MarshalInfo);
        }

        public override bool Equals(object? obj) => obj is InterceptedMethodReference other && Equals(other);
        public bool Equals(InterceptedMethodReference? other) => (other is not null) &&
            (MethodReference.Method == other.MethodReference.Method) &&
            (MethodReference.MarshalInfo == other.MethodReference.MarshalInfo);
        public override int GetHashCode() => hashCode;
    }
}
