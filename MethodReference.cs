using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class MethodReference : IEquatable<MethodReference>
    {
        private readonly int hashCode;

        public int Character { get; }
        public string FilePath { get; }
        public string InterceptorAttributeSourceText { get; }
        public InterfaceDescriptor Interface { get; }
        public bool IsInterfaceOrMethodOpenGeneric { get; }
        public int Line { get; }
        public DelegateMarshalling Marshalling { get; }
        public MethodDescriptor Method { get; }

        public static bool operator ==(MethodReference? left, MethodReference? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(MethodReference? left, MethodReference? right) => !(left == right);

        public static MethodReference? GetReference
        (
            GenericSymbolReference interfaceOrMethodReference,
            IList<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            INamedTypeSymbol? interfaceSymbol;
            IMethodSymbol? methodSymbol;
            InvocationExpressionSyntax? invocationExpression;
            var node = interfaceOrMethodReference.Node;
            if (node is InvocationExpressionSyntax genericMethodInvocationExpression)
            {
                // generic methods (currently none)
                methodSymbol = (IMethodSymbol)interfaceOrMethodReference.Symbol;
                interfaceSymbol = methodSymbol.ContainingType;
                invocationExpression = genericMethodInvocationExpression;
            }
            else if (node.Parent?.Parent is InvocationExpressionSyntax methodInvocationExpression)
            {
                // non-generic methods (FromAction, FromFunc, FromFunctionPointer)
                var methodNameSyntax = ((MemberAccessExpressionSyntax)node.Parent).Name;
                if (methodNameSyntax.Arity != 0)
                {
                    return null;
                }
                interfaceSymbol = (INamedTypeSymbol)interfaceOrMethodReference.Symbol;
                methodSymbol = interfaceSymbol.GetMembers(methodNameSyntax.Identifier.ToString()).Cast<IMethodSymbol>()
                    .First(x => !x.IsGenericMethod);
                invocationExpression = methodInvocationExpression;
            }
            else
            {
                return null;
            }
            var interfaceDescriptor = new InterfaceDescriptor(interfaceSymbol);
            var methodDescriptor = new MethodDescriptor
            (
                interfaceDescriptor,
                methodSymbol
            );
            var methodMarshalling = new DelegateMarshalling
            (
                invocationExpression,
                interfaceOrMethodReference.SemanticModel!,
                interfaceDescriptor,
                diagnostics,
                cancellationToken
            );
            return new MethodReference
            (
                interfaceDescriptor,
                methodDescriptor,
                invocationExpression,
                methodMarshalling,
                !interfaceOrMethodReference.IsSyntaxReferenceClosedTypeOrMethod
            );
        }

        private MethodReference
        (
            InterfaceDescriptor interfaceDescriptor,
            MethodDescriptor methodDescriptor,
            InvocationExpressionSyntax invocationExpression,
            DelegateMarshalling marshalling,
            bool isInterfaceOrMethodOpenGeneric
        )
        {
            var methodNode = ((MemberAccessExpressionSyntax)invocationExpression.Expression).Name;
            var linePosition = methodNode.GetLocation().GetLineSpan().Span.Start;
            Character = linePosition.Character + 1;
            FilePath = invocationExpression.SyntaxTree.FilePath;
            Interface = interfaceDescriptor;
            IsInterfaceOrMethodOpenGeneric = isInterfaceOrMethodOpenGeneric;
            Line = linePosition.Line + 1;
            Marshalling = marshalling;
            Method = methodDescriptor;
            InterceptorAttributeSourceText = $"[InterceptsLocation(@\"{FilePath}\", {Line}, {Character})]";
            hashCode = Hash.Combine(Character, FilePath, Interface, Line);
        }

        public override bool Equals(object? obj) => obj is MethodReference other && Equals(other);
        public bool Equals(MethodReference? other) => (other is not null) && (Character == other.Character) &&
            (Line == other.Line) && (FilePath == other.FilePath);
        public override int GetHashCode() => hashCode;
    }
}
