using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class OpenGenericInterceptors
    {
        private readonly struct Key(ImplementationClass implementationClass) : IEquatable<Key>
        {
            private readonly int hashCode = implementationClass.ID.GetHashCode();

            public static bool operator ==(Key left, Key right) => left.Equals(right);
            public static bool operator !=(Key left, Key right) => !(left == right);

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => hashCode == other.hashCode;
            public override int GetHashCode() => hashCode;
        }
    }
}
