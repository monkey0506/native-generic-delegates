using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClass
    {
        public readonly struct Key(MethodReference methodReference) : IEquatable<Key>
        {
            private readonly int hashCode = Hash.Combine
            (
                methodReference.Method,
                methodReference.InvocationArgumentCount,
                methodReference.MarshalInfo
            );

            public readonly MethodReference MethodReference = methodReference;

            public static bool operator ==(Key left, Key right) => left.Equals(right);
            public static bool operator !=(Key left, Key right) => !(left == right);

            public static implicit operator Key(MethodReference methodReference) => new(methodReference);

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => (MethodReference.Method == other.MethodReference.Method) &&
                (MethodReference.InvocationArgumentCount == other.MethodReference.InvocationArgumentCount) &&
                (MethodReference.MarshalInfo == other.MethodReference.MarshalInfo);
            public override int GetHashCode() => hashCode;
        }
    }
}
