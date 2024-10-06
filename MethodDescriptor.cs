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
        public string InterceptorParameters { get; }
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
            InterceptorParameters = GetParameters(getInterceptorParameters: true);
            Name = methodSymbol.Name;
            Parameters = GetParameters(getInterceptorParameters: false);
            hashCode = Hash.Combine(Arity, ContainingInterface, Name);
        }

        public override bool Equals(object? obj) => obj is MethodDescriptor other && Equals(other);
        public bool Equals(MethodDescriptor? other) =>
            (other is not null) && (Arity == other.Arity) && (ContainingInterface == other.ContainingInterface) &&
            (Name == other.Name);
        public override int GetHashCode() => hashCode;

        private string GetParameters(bool getInterceptorParameters)
        {
            var isAction = ContainingInterface.IsAction;
            var invokeParamCount = ContainingInterface.InvokeParameterCount;
            var hasMarshalParams = !isAction || (invokeParamCount != 0);
            var callingConvention = $"CallingConvention callingConvention" +
                (hasMarshalParams ? $", {Constants.NewLineIndent3}" : string.Empty);
            var marshalMap = hasMarshalParams ? $"MarshalMap marshalMap,{Constants.NewLineIndent3}" : string.Empty;
            var marshalReturnAsParam = !ContainingInterface.IsAction ?
                $"MarshalAsAttribute marshalReturnAs" +
                    (invokeParamCount != 0 ? $",{Constants.NewLineIndent3}" : string.Empty) :
                string.Empty;
            var marshalParamsAsParam = invokeParamCount != 0 ?
                $"MarshalAsAttribute[] marshalParamsAs" :
                string.Empty;
            var firstParamType = FirstParameterType;
            if (getInterceptorParameters && !IsFromFunctionPointer && (ContainingInterface.Arity > 0))
            {
                var typeParameters = Constants.InterceptorTypeParameters[ContainingInterface.Arity];
                firstParamType = $"{ContainingInterface.Category}<{typeParameters}>";
            }
            var firstParam = $"{firstParamType} {FirstParameterName},{Constants.NewLineIndent3}";
            return $"{firstParam}{callingConvention}{marshalMap}{marshalReturnAsParam}{marshalParamsAsParam}";
        }
    }
}
