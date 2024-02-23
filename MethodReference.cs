using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct MethodReference : IEquatable<MethodReference>
    {
        private readonly int hashCode;

        public readonly ArgumentInfo ArgumentInfo;
        public readonly int Character;
        public readonly string FilePath;
        public readonly int InvokeParameterCount;
        public readonly bool IsAction;
        public readonly bool IsFromFunctionPointer;
        public readonly bool IsSyntaxReferenceClosedTypeOrMethod;
        public readonly int Line;
        public readonly IMethodSymbol Method;

        public static bool operator ==(MethodReference left, MethodReference right) => left.Equals(right);
        public static bool operator !=(MethodReference left, MethodReference right) => !(left == right);

        public static MethodReference? GetReference
        (
            GenericSymbolReference genericSymbolReference,
            List<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            if (genericSymbolReference.Node is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                // FromFunctionPointer<...> (generic)
                return new
                (
                    (IMethodSymbol)genericSymbolReference.Symbol,
                    genericSymbolReference.SemanticModel!,
                    invocationExpressionSyntax,
                    genericSymbolReference.IsSyntaxReferenceClosedTypeOrMethod,
                    diagnostics,
                    cancellationToken
                );
            }
            else if (genericSymbolReference.Node.Parent?.Parent is InvocationExpressionSyntax)
            {
                // FromAction, FromFunc, FromFunctionPointer (non-generic)
                var methodNameSyntax = ((MemberAccessExpressionSyntax)genericSymbolReference.Node.Parent).Name;
                if (methodNameSyntax.Arity != 0)
                {
                    return null;
                }
                var method = ((INamedTypeSymbol)genericSymbolReference.Symbol)
                    .GetMembers(methodNameSyntax.Identifier.ToString()).Cast<IMethodSymbol>().First(x => !x.IsGenericMethod);
                return new
                (
                    method,
                    genericSymbolReference.SemanticModel!,
                    (InvocationExpressionSyntax)genericSymbolReference.Node.Parent.Parent,
                    genericSymbolReference.IsSyntaxReferenceClosedTypeOrMethod,
                    diagnostics,
                    cancellationToken
                );
            }
            return null;
        }

        private MethodReference
        (
            IMethodSymbol method,
            SemanticModel semanticModel,
            InvocationExpressionSyntax invocationExpressionSyntax,
            bool isSyntaxReferenceClosedTypeOrMethod,
            List<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            var interfaceType = method.ContainingType;
            bool isAction = interfaceType.Name.Contains("Action");
            int invokeParameterCount = interfaceType.Arity - (isAction ? 0 : 1);
            var methodNode = ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).Name;
            var linePosition = methodNode.GetLocation().GetLineSpan().Span.Start;
            ArgumentInfo = new
            (
                invocationExpressionSyntax,
                semanticModel,
                invokeParameterCount,
                diagnostics,
                cancellationToken
            );
            Character = linePosition.Character + 1;
            FilePath = invocationExpressionSyntax.SyntaxTree.FilePath;
            InvokeParameterCount = invokeParameterCount;
            IsAction = isAction;
            IsFromFunctionPointer = method.Name == Constants.FromFunctionPointerIdentifier;
            IsSyntaxReferenceClosedTypeOrMethod = isSyntaxReferenceClosedTypeOrMethod;
            Line = linePosition.Line + 1;
            Method = method;
            hashCode = Hash.Combine(Character, FilePath, Line);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodReference other && Equals(other);
        }

        public bool Equals(MethodReference other)
        {
            return (Character == other.Character) && (FilePath == other.FilePath) && (Line == other.Line);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
