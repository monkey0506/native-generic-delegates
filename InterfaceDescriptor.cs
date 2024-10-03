using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class InterfaceDescriptor : IEquatable<InterfaceDescriptor>
    {
        private readonly int hashCode;

        public int Arity { get; }
        public string Category { get; }
        public string FullName { get; }
        public int InvokeParameterCount { get; }
        public bool IsAction { get; }
        public string Name { get; }
        public IReadOnlyList<string> TypeArguments { get; }
        public string TypeArgumentList { get; }

        public static bool operator ==(InterfaceDescriptor? left, InterfaceDescriptor? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(InterfaceDescriptor? left, InterfaceDescriptor? right) => !(left == right);

        public InterfaceDescriptor(INamedTypeSymbol interfaceSymbol)
        {
            bool isAction = interfaceSymbol.Name.Contains(Constants.CategoryAction);
            Arity = interfaceSymbol.Arity;
            Category = isAction ? Constants.CategoryAction : Constants.CategoryFunc;
            IsAction = isAction;
            InvokeParameterCount = interfaceSymbol.Arity - (isAction ? 0 : 1);
            Name = interfaceSymbol.Name;
            TypeArguments = [.. interfaceSymbol.TypeArguments.Select(x => x.ToDisplayString())];
            TypeArgumentList =
                Arity == 0 ?
                    string.Empty :
                    $"<{string.Join(", ", TypeArguments)}>";
            FullName = $"{Name}{TypeArgumentList}";
            hashCode = Hash.Combine(Arity, FullName);
        }

        public override bool Equals(object? obj) => obj is InterfaceDescriptor other && Equals(other);
        public bool Equals(InterfaceDescriptor? other) =>
            (other is not null) && (Arity == other.Arity) && (FullName == other.FullName);
        public override int GetHashCode() => hashCode;
    }
}
