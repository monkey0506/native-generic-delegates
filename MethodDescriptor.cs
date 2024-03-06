using Microsoft.CodeAnalysis;
using System;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct MethodDescriptor : IEquatable<MethodDescriptor>
    {
        private readonly int hashCode;

        public readonly int Arity;
        public readonly string ContainingInterface;
        public readonly bool IsFromFunctionPointer;
        public readonly string Name;

        public static bool operator ==(MethodDescriptor left, MethodDescriptor right) => left.Equals(right);
        public static bool operator !=(MethodDescriptor left, MethodDescriptor right) => !(left == right);

        public MethodDescriptor(string containingInterface, IMethodSymbol methodSymbol)
        {
            Arity = methodSymbol.Arity;
            ContainingInterface = containingInterface;
            IsFromFunctionPointer = methodSymbol.Name == Constants.FromFunctionPointerIdentifier;
            Name = methodSymbol.Name;
            hashCode = Hash.Combine(Arity, ContainingInterface, Name);
        }

        public override bool Equals(object? obj)
        {
            return obj is MethodDescriptor other && Equals(other);
        }

        public bool Equals(MethodDescriptor other)
        {
            return (Arity == other.Arity) && (ContainingInterface == other.ContainingInterface) && (Name == other.Name);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
