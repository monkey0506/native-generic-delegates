using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClass
    {
        public readonly struct Key(MethodReference methodReference) : IEquatable<Key>
        {
            private readonly int hashCode = methodReference.GetHashCode();

            public readonly MethodReference MethodReference = methodReference;

            public static bool operator ==(Key left, Key right) => left.Equals(right);
            public static bool operator !=(Key left, Key right) => !(left == right);

            public static implicit operator Key(MethodReference methodReference) => new(methodReference);

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => MethodReference == other.MethodReference;
            public override int GetHashCode() => hashCode;
        }
    }
}
