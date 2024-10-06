using System;
using System.Diagnostics;

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
                hashCode = Hash.Combine(MethodReference.Method, MethodReference.Marshalling);
                Debug.WriteLine($"hashCode: {hashCode}");
            }

            public override bool Equals(object? obj) => obj is Key other && Equals(other);
            public bool Equals(Key other) => (MethodReference.Method == other.MethodReference.Method) &&
                (MethodReference.Marshalling == other.MethodReference.Marshalling);
            //public bool Equals(Key other)
            //{
            //    Debug.WriteLine($"interface: {MethodReference.Method.ContainingInterface.FullName} / {other.MethodReference.Method.ContainingInterface.FullName} / equal? {MethodReference.Method.ContainingInterface == other.MethodReference.Method.ContainingInterface}");
            //    Debug.WriteLine($"method: {MethodReference.Method.Name} / {other.MethodReference.Method.Name} / equal? {MethodReference.Method == other.MethodReference.Method}");
            //    Debug.WriteLine($"arity: {MethodReference.Method.Arity} / {other.MethodReference.Method.Arity}");
            //    Debug.WriteLine($"aritys equal? {MethodReference.Method.Arity == other.MethodReference.Method.Arity}");
            //    Debug.WriteLine($"interfaces equal? {MethodReference.Method.ContainingInterface == other.MethodReference.Method.ContainingInterface}");
            //    Debug.WriteLine($"names equal? {MethodReference.Method.Name == other.MethodReference.Method.Name}");
            //    Debug.WriteLine($"methods equal? {MethodReference.Method == other.MethodReference.Method}");
            //    //public bool Equals(MethodDescriptor? other) =>
            //    //    (other is not null) && (Arity == other.Arity) && (ContainingInterface == other.ContainingInterface) &&
            //    //    (Name == other.Name);
            //    return (MethodReference.Method == other.MethodReference.Method) && (MethodReference.Marshalling == other.MethodReference.Marshalling);
            //}
            public override int GetHashCode() => hashCode;
        }
    }
}
