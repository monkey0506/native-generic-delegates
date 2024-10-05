using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class MethodDescriptor : IEquatable<MethodDescriptor>
    {
        private readonly int hashCode;

        public int Arity { get; }
        public InterfaceDescriptor ContainingInterface { get; }
        public string FirstParameterName { get; }
        public string FirstParameterType { get; }
        public string FullName { get; }
        public bool IsFromFunctionPointer { get; }
        public string Name { get; }
        public string Parameters { get; }

        public static bool operator ==(MethodDescriptor? left, MethodDescriptor? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(MethodDescriptor? left, MethodDescriptor? right) => !(left == right);

        private static string GetFullName(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.Arity == 0)
            {
                return methodSymbol.Name;
            }
            var typeParameters = methodSymbol.TypeParameters.Select(x => x.ToDisplayString());
            return $"{methodSymbol.Name}<{string.Join(", ", typeParameters)}>";
        }

        public MethodDescriptor
        (
            InterfaceDescriptor containingInterface,
            IMethodSymbol methodSymbol
        )
        {
            bool isFromFunctionPointer = methodSymbol.Name == Constants.FromFunctionPointerIdentifier;
            if (isFromFunctionPointer)
            {
                FirstParameterName = "functionPtr";
                FirstParameterType = "nint";
                IsFromFunctionPointer = true;
            }
            else
            {
                var category = containingInterface.Category;
                FirstParameterName = category.ToLower();
                FirstParameterType = $"{category}{containingInterface.TypeArgumentList}";
                IsFromFunctionPointer = false;
            }
            Arity = methodSymbol.Arity;
            ContainingInterface = containingInterface;
            FullName = GetFullName(methodSymbol);
            Name = methodSymbol.Name;
            Parameters = GetParameters();
            hashCode = Hash.Combine(Arity, ContainingInterface, Name);
        }

        public override bool Equals(object? obj) => obj is MethodDescriptor other && Equals(other);
        public bool Equals(MethodDescriptor? other) =>
            (other is not null) && (Arity == other.Arity) && (ContainingInterface == other.ContainingInterface) &&
            (Name == other.Name);
        public override int GetHashCode() => hashCode;

        private string GetParameters()
        {
            var marshalReturnAsParam = !ContainingInterface.IsAction ?
                $"MarshalAsAttribute marshalReturnAs,{Constants.NewLineIndent3}" :
                string.Empty;
            var marshalParamsAsParam = ContainingInterface.InvokeParameterCount != 0 ?
                $"MarshalAsAttribute[] marshalParamsAs,{Constants.NewLineIndent3}" :
                string.Empty;
            return
                $"{FirstParameterType} {FirstParameterName},{Constants.NewLineIndent3}{marshalReturnAsParam}" +
                $"{marshalParamsAsParam}CallingConvention callingConvention";
        }
    }
}
