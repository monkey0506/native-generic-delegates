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
        public string FirstArgument { get; }
        public string FirstParameterName { get; }
        public string FirstParameterType { get; }
        public string FullName { get; }
        public InterceptorDescriptor Interceptor { get; }
        public bool IsFromFunctionPointer { get; }
        public bool IsFromUnsafeFunctionPointer { get; }
        public string Name { get; }
        public string Parameters { get; }
        public string UnsafeKeywordSourceText { get; }

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
            if (methodSymbol.Parameters.FirstOrDefault()?.Type is IFunctionPointerTypeSymbol functionPtrSymbol)
            {
                FirstParameterName = "functionPtr";
                FirstArgument = $"(nint){FirstParameterName}";
                FirstParameterType = functionPtrSymbol.ToDisplayString();
                IsFromFunctionPointer = true;
                IsFromUnsafeFunctionPointer = true;
            }
            else if (methodSymbol.Name == Constants.FromFunctionPointerIdentifier)
            {
                FirstParameterName = "functionPtr";
                FirstArgument = FirstParameterName;
                FirstParameterType = "nint";
                IsFromFunctionPointer = true;
                IsFromUnsafeFunctionPointer = false;
            }
            else
            {
                var category = containingInterface.Category;
                FirstParameterName = category.ToLower();
                FirstArgument = FirstParameterName;
                FirstParameterType = $"{category}{containingInterface.BaseInterface.TypeArgumentList}";
                IsFromFunctionPointer = false;
                IsFromUnsafeFunctionPointer = false;
            }
            Arity = methodSymbol.Arity;
            ContainingInterface = containingInterface;
            FullName = GetFullName(methodSymbol);
            Name = methodSymbol.Name;
            Parameters = GetParameters(getInterceptorParameters: false);
            UnsafeKeywordSourceText = IsFromUnsafeFunctionPointer ? "unsafe " : string.Empty;
            Interceptor = new InterceptorDescriptor(this, GetParameters(getInterceptorParameters: true));
            hashCode = Hash.Combine(Arity, ContainingInterface, FullName);
        }

        public override bool Equals(object? obj) => obj is MethodDescriptor other && Equals(other);
        public bool Equals(MethodDescriptor? other) =>
            (other is not null) && (Arity == other.Arity) && (ContainingInterface == other.ContainingInterface) &&
            (FullName == other.FullName);
        public override int GetHashCode() => hashCode;

        private string GetParameters(bool getInterceptorParameters)
        {
            if (IsFromUnsafeFunctionPointer)
            {
                return $"{FirstParameterType} {FirstParameterName}";
            }
            var firstParamType = FirstParameterType;
            if (getInterceptorParameters && !IsFromFunctionPointer && (ContainingInterface.Arity > 0))
            {
                var typeParameters = Constants.InterceptorTypeParameters[ContainingInterface.Arity];
                firstParamType = $"{ContainingInterface.Category}<{typeParameters}>";
            }
            return $"{firstParamType} {FirstParameterName},{Constants.NewLineIndent3}CallingConvention " +
                "callingConvention";
        }
    }
}
