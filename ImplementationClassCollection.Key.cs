using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClassCollection
    {
        private readonly struct Key : IEquatable<Key>
        {
            private readonly int hashCode;

            public readonly MethodReference MethodReference;

            public static bool operator ==(Key left, Key right) => left.Equals(right);
            public static bool operator !=(Key left, Key right) => !(left == right);

            public static implicit operator Key(MethodReference methodReference) => new(methodReference);
            public static implicit operator MethodReference(Key key) => key.MethodReference;

            public Key(MethodReference methodReference)
            {
                MethodReference = methodReference;
                hashCode = Hash.Combine
                (
                    MethodReference.Method,
                    MethodReference.InvocationArgumentCount,
                    MethodReference.MarshalInfo
                );
            }

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => (MethodReference.Method == other.MethodReference.Method) &&
                (MethodReference.InvocationArgumentCount == other.MethodReference.InvocationArgumentCount) &&
                (MethodReference.MarshalInfo == other.MethodReference.MarshalInfo);
            public override int GetHashCode() => hashCode;
        }
    }
}
