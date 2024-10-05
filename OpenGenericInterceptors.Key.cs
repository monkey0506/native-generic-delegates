using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class OpenGenericInterceptors
    {
        private readonly struct Key(MethodDescriptor method, DelegateMarshalling marshalling) : IEquatable<Key>
        {
            private readonly int hashCode = Hash.Combine
            (
                method.Name,
                method.Arity,
                marshalling,
                method.ContainingInterface.Name,
                method.ContainingInterface.Arity
            );

            public static bool operator ==(Key left, Key right) => left.Equals(right);
            public static bool operator !=(Key left, Key right) => !(left == right);

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => hashCode == other.hashCode;
            public override int GetHashCode() => hashCode;
        }
    }
}
