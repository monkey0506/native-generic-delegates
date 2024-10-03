using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClass
    {
        public readonly struct ClassID(MethodDescriptor method, DelegateMarshalling marshalling) : IEquatable<ClassID>
        {
            private readonly int hashCode = Hash.Combine(method, marshalling);

            public static bool operator ==(ClassID left, ClassID right) => left.Equals(right);
            public static bool operator !=(ClassID left, ClassID right) => !(left == right);

            public override bool Equals(object? obj) => obj is ClassID other && Equals(other);
            public bool Equals(ClassID other) => hashCode == other.hashCode;
            public override int GetHashCode() => hashCode;
            public override string ToString() => hashCode < 0 ? $"S{-hashCode}" : $"U{hashCode}";
        }
    }
}
