using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterfaceDescriptor : IEquatable<InterfaceDescriptor>
    {
        private readonly int hashCode;

        public readonly int Arity;
        public readonly string FullName;
        public readonly int InvokeParameterCount;
        public readonly bool IsAction;
        public readonly string Name;
        public readonly ImmutableArray<string> TypeArguments;
        public readonly string TypeArgumentList;

        public static bool operator ==(InterfaceDescriptor left, InterfaceDescriptor right) => left.Equals(right);
        public static bool operator !=(InterfaceDescriptor left, InterfaceDescriptor right) => !(left == right);

        public InterfaceDescriptor(INamedTypeSymbol interfaceSymbol)
        {
            Arity = interfaceSymbol.Arity;
            IsAction = interfaceSymbol.Name.Contains("Action");
            InvokeParameterCount = interfaceSymbol.Arity - (IsAction ? 0 : 1);
            Name = interfaceSymbol.Name;
            TypeArguments = interfaceSymbol.TypeArguments.Select(x => x.ToDisplayString()).ToImmutableArray();
            TypeArgumentList = Arity == 0 ?
                "" :
                $"<{string.Join(", ", TypeArguments)}>";
            FullName = $"{Name}{TypeArgumentList}";
            hashCode = Hash.Combine(Arity, FullName);
        }

        public override bool Equals(object? obj)
        {
            return obj is InterfaceDescriptor other && Equals(other);
        }

        public bool Equals(InterfaceDescriptor other)
        {
            return (Arity == other.Arity) && (FullName == other.FullName);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
