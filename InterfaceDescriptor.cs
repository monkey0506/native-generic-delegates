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
        public InterfaceDescriptor BaseInterface { get; }
        public string Category { get; }
        public string FullName { get; }
        public string FunctionPointerTypeArgumentList { get; }
        public int InvokeParameterCount { get; }
        public bool IsAction { get; }
        public bool IsUnmanaged { get; }
        public string Name { get; }
        public string ReturnKeywordSourceText { get; }
        public string ReturnType { get; }
        public IReadOnlyList<string> TypeArguments { get; }
        public string TypeArgumentList { get; }

        public static bool operator ==(InterfaceDescriptor? left, InterfaceDescriptor? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(InterfaceDescriptor? left, InterfaceDescriptor? right) => !(left == right);

        private InterfaceDescriptor
        (
            string name,
            int arity,
            IEnumerable<string> typeArguments,
            bool isAction,
            bool isUnmanaged
        )
        {
            Arity = arity;
            Name = name;
            TypeArguments = [.. typeArguments];
            TypeArgumentList = Arity != 0 ?
                $"<{string.Join(", ", TypeArguments)}>" :
                string.Empty;
            FullName = $"{Name}{TypeArgumentList}";
            if (isUnmanaged)
            {
                var baseArity = Arity / 2;
                var baseName = name.Replace("Unmanaged", "Native");
                BaseInterface = new(baseName, baseArity, typeArguments.Take(baseArity), isAction, false);
                if (arity == 0)
                {
                    FunctionPointerTypeArgumentList = "<void>";
                }
                else
                {
                    typeArguments = typeArguments.Skip(baseArity);
                    var voidReturn = isAction ? ", void" : string.Empty;
                    FunctionPointerTypeArgumentList = $"<{string.Join(", ", typeArguments)}{voidReturn}>";
                }
                IsUnmanaged = true;
            }
            else
            {
                BaseInterface = this;
                FunctionPointerTypeArgumentList = string.Empty;
                IsUnmanaged = false;
            }
            if (isAction)
            {
                Category = Constants.CategoryAction;
                InvokeParameterCount = BaseInterface.Arity;
                IsAction = true;
                ReturnKeywordSourceText = string.Empty;
                ReturnType = "void";
            }
            else
            {
                Category = Constants.CategoryFunc;
                InvokeParameterCount = BaseInterface.Arity - 1;
                IsAction = false;
                ReturnKeywordSourceText = "return ";
                ReturnType = TypeArguments.Last();
            }
            hashCode = Hash.Combine(Arity, FullName);
        }

        public InterfaceDescriptor(INamedTypeSymbol interfaceSymbol) : this
        (
            interfaceSymbol.Name,
            interfaceSymbol.Arity,
            interfaceSymbol.TypeArguments.Select(static x => x.ToDisplayString()),
            interfaceSymbol.Name.Contains(Constants.CategoryAction),
            interfaceSymbol.Name.Contains("Unmanaged")
        )
        {
        }

        public override bool Equals(object? obj) => obj is InterfaceDescriptor other && Equals(other);
        public bool Equals(InterfaceDescriptor? other) =>
            (other is not null) && (Arity == other.Arity) && (FullName == other.FullName);
        public override int GetHashCode() => hashCode;
    }
}
