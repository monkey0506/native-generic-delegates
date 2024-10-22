using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct InterfaceReferenceCollection :
        IEquatable<InterfaceReferenceCollection>,
        IEnumerable<InterfaceReference>
    {
        private readonly int hashCode;
        private readonly ImmutableHashSet<InterfaceReference> interfaceReferences;
        private readonly ImmutableHashSet<GenericSymbolReference> methodReferences;

        public static bool operator ==(InterfaceReferenceCollection left, InterfaceReferenceCollection right) =>
            left.Equals(right);
        public static bool operator !=(InterfaceReferenceCollection left, InterfaceReferenceCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<InterfaceReferenceCollection> GetReferences
        (
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<InterfaceOrMethodSymbolCollection> symbolsProvider
        )
        {
            var nonGenericInterfaceReferenceProvider = context.SyntaxProvider.CreateSyntaxProvider
            (
                (node, _) =>
                {
                    if ((node is not MemberAccessExpressionSyntax memberAccessExpression) ||
                        (memberAccessExpression.Expression is not IdentifierNameSyntax identifierName) ||
                        (node.Parent is not InvocationExpressionSyntax))
                    {
                        return false;
                    }
                    string memberName = memberAccessExpression.Name.Identifier.ValueText;
                    string parentName = identifierName.Identifier.ValueText;
                    return ((memberName == "FromAction") || (memberName == "FromFunctionPointer")) &&
                        ((parentName == "INativeAction") || (parentName == "IUnmanagedAction"));
                },
                (context, cancellationToken) => (IInvocationOperation)context.SemanticModel
                    .GetOperation(context.Node.Parent!, cancellationToken)!
            ).Collect();
            var treeProvider = GenericSymbolReferenceTree.FromIncrementalGeneratorInitializationContext(context);
            return symbolsProvider.Combine(nonGenericInterfaceReferenceProvider).Combine(treeProvider).Select
            (
                static (x, cancellationToken) =>
                {
                    var (symbols, nonGenericInterfaceReferences) = x.Left;
                    using var tree = x.Right; // Dispose tree after we extract the symbol references we need
                    var interfaceReferences = ImmutableHashSet.CreateBuilder<InterfaceReference>();
                    var methodReferences = ImmutableHashSet.CreateBuilder<GenericSymbolReference>();
                    foreach (var symbol in symbols)
                    {
                        switch (symbol)
                        {
                            case INamedTypeSymbol { IsGenericType: true }:
                                interfaceReferences.UnionWith
                                (
                                    tree.GetBranchesBySymbol(symbol, cancellationToken)
                                        .Select(x => InterfaceReference.GetReference(x, cancellationToken))
                                        .Where(static x => x is not null)!
                                );
                                break;
                            case INamedTypeSymbol { IsGenericType: false }:
                                break;
                            case IMethodSymbol methodSymbol:
                                methodReferences.UnionWith(tree.GetBranchesBySymbol(symbol, cancellationToken));
                                break;
                            default:
                                throw new UnreachableException();
                        }
                    }
                    foreach
                    (
                        var reference in nonGenericInterfaceReferences
                            .Select(static x => InterfaceReference.GetReference(x))
                            .Where(static x => x is not null)
                    )
                    {
                        _ = interfaceReferences.Add(reference!);
                    }
                    return new InterfaceReferenceCollection
                    (
                        interfaceReferences.ToImmutable(),
                        methodReferences.ToImmutable()
                    );
                }
            );
        }

        private InterfaceReferenceCollection
        (
            ImmutableHashSet<InterfaceReference> interfaceReferences,
            ImmutableHashSet<GenericSymbolReference> methodReferences
        )
        {
            this.interfaceReferences = interfaceReferences;
            this.methodReferences = methodReferences;
            hashCode = Hash.Combine(interfaceReferences, methodReferences);
        }

        public override bool Equals(object? obj) => obj is InterfaceReferenceCollection other && Equals(other);
        public bool Equals(InterfaceReferenceCollection other) =>
            interfaceReferences.SetEquals(other.interfaceReferences) &&
            methodReferences.SetEquals(other.methodReferences);
        public IEnumerator<InterfaceReference> GetEnumerator() => interfaceReferences.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;

        public IReadOnlyCollection<GenericSymbolReference> GetGenericMethodReferences
        (
            InterfaceReference interfaceReference
        )
        {
            var methodSymbol = interfaceReference.MethodInvocation.TargetMethod.OriginalDefinition;
            var node = interfaceReference.MethodInvocation.Syntax;
            return methodReferences.Where
            (
                x => SymbolEqualityComparer.Default.Equals(x.Symbol.OriginalDefinition, methodSymbol) &&
                    x.Node.IsEquivalentTo(node)
            ).ToImmutableList();
        }
    }
}
