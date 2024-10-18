using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class OpenGenericInterceptors
    {
        private readonly struct Key(ImplementationClass.Key classKey) : IEquatable<Key>
        {
            private readonly int hashCode = classKey.MethodReference.Location.GetHashCode();
            private readonly InterceptedLocation location = classKey.MethodReference.Location;

            public MethodDescriptor Method { get; } = classKey.MethodReference.Method;

            public static bool operator ==(Key left, Key right) => left.Equals(right);
            public static bool operator !=(Key left, Key right) => !(left == right);

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => location == other.location;
            public override int GetHashCode() => hashCode;
        }
    }
}
