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
        public readonly InterfaceDescriptor Interface;
        public readonly bool IsSyntaxReferenceClosedTypeOrMethod;
        public readonly int Line;
        public readonly MethodDescriptor Method;

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
                // generic methods (currently none)
                return new
                (
                    ((IMethodSymbol)genericSymbolReference.Symbol).ContainingType,
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
                // non-generic methods (FromAction, FromFunc, FromFunctionPointer)
                var methodNameSyntax = ((MemberAccessExpressionSyntax)genericSymbolReference.Node.Parent).Name;
                if (methodNameSyntax.Arity != 0)
                {
                    return null;
                }
                var method = ((INamedTypeSymbol)genericSymbolReference.Symbol)
                    .GetMembers(methodNameSyntax.Identifier.ToString()).Cast<IMethodSymbol>().First(x => !x.IsGenericMethod);
                return new
                (
                    (INamedTypeSymbol)genericSymbolReference.Symbol,
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
            INamedTypeSymbol interfaceSymbol,
            IMethodSymbol methodSymbol,
            SemanticModel semanticModel,
            InvocationExpressionSyntax invocationExpressionSyntax,
            bool isSyntaxReferenceClosedTypeOrMethod,
            List<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            var methodNode = ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).Name;
            var linePosition = methodNode.GetLocation().GetLineSpan().Span.Start;
            Interface = new InterfaceDescriptor(interfaceSymbol);
            ArgumentInfo = new
            (
                invocationExpressionSyntax,
                semanticModel,
                Interface.InvokeParameterCount,
                diagnostics,
                cancellationToken
            );
            Character = linePosition.Character + 1;
            FilePath = invocationExpressionSyntax.SyntaxTree.FilePath;
            IsSyntaxReferenceClosedTypeOrMethod = isSyntaxReferenceClosedTypeOrMethod;
            Line = linePosition.Line + 1;
            Method = new MethodDescriptor(Interface.FullName, methodSymbol);
            hashCode = Hash.Combine(Character, FilePath, Line, Interface);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodReference other && Equals(other);
        }

        public bool Equals(MethodReference other)
        {
            if (IsSyntaxReferenceClosedTypeOrMethod)
            {
                if (!other.IsSyntaxReferenceClosedTypeOrMethod)
                {
                    return false;
                }
                return Interface == other.Interface;
            }
            else if (other.IsSyntaxReferenceClosedTypeOrMethod)
            {
                return false;
            }
            return (Character == other.Character) && (FilePath == other.FilePath) && (Line == other.Line);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
