using Microsoft.CodeAnalysis;
using Monkeymoto.GeneratorUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterfaceReferenceCollection : IEquatable<InterfaceReferenceCollection>
    {
        private readonly int hashCode;

        public readonly ImmutableHashSet<GenericSymbolReference> ActionReferences;
        public readonly ImmutableHashSet<GenericSymbolReference> ActionFromFunctionPointerGenericReferences;
        public readonly ImmutableHashSet<GenericSymbolReference> FuncReferences;
        public readonly ImmutableHashSet<GenericSymbolReference> FuncFromFunctionPointerGenericReferences;

        public static bool operator ==(InterfaceReferenceCollection left, InterfaceReferenceCollection right) =>
            left.Equals(right);
        public static bool operator !=(InterfaceReferenceCollection left, InterfaceReferenceCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<InterfaceReferenceCollection> GetReferences
        (
            IncrementalGeneratorInitializationContext context,
            IncrementalValueProvider<InterfaceSymbolCollection> symbolsProvider
        )
        {
            var treeProvider = GenericSymbolReferenceTree.FromIncrementalGeneratorInitializationContext(context);
            return symbolsProvider.Combine(treeProvider).Select(static (x, cancellationToken) =>
            {
                var symbols = x.Left;
                using var tree = x.Right; // Dispose tree after we extract the symbol references we need
                HashSet<GenericSymbolReference> actionReferences = [];
                HashSet<GenericSymbolReference> actionFromFunctionPointerGenericReferences = [];
                HashSet<GenericSymbolReference> funcReferences = [];
                HashSet<GenericSymbolReference> funcFromFunctionPointerGenericReferences = [];
                for (int i = 0; i < 17; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    actionReferences.UnionWith(tree.GetBranchesBySymbol(symbols.ActionInterfaceSymbols[i], cancellationToken));
                    actionFromFunctionPointerGenericReferences.UnionWith
                    (
                        tree.GetBranchesBySymbol(symbols.ActionFromFunctionPointerGenericSymbols[i], cancellationToken)
                    );
                    funcReferences.UnionWith(tree.GetBranchesBySymbol(symbols.FuncInterfaceSymbols[i], cancellationToken));
                    funcFromFunctionPointerGenericReferences.UnionWith
                    (
                        tree.GetBranchesBySymbol(symbols.FuncFromFunctionPointerGenericSymbols[i], cancellationToken)
                    );
                }
                return new InterfaceReferenceCollection
                (
                    actionReferences,
                    actionFromFunctionPointerGenericReferences,
                    funcReferences,
                    funcFromFunctionPointerGenericReferences
                );
            });
        }

        private InterfaceReferenceCollection
        (
            HashSet<GenericSymbolReference> actionReferences,
            HashSet<GenericSymbolReference> actionFromFunctionPointerGenericReferences,
            HashSet<GenericSymbolReference> funcReferences,
            HashSet<GenericSymbolReference> funcFromFunctionPointerGenericReferences
        )
        {
            ActionReferences = actionReferences.ToImmutableHashSet();
            ActionFromFunctionPointerGenericReferences = actionFromFunctionPointerGenericReferences.ToImmutableHashSet();
            FuncReferences = funcReferences.ToImmutableHashSet();
            FuncFromFunctionPointerGenericReferences = funcFromFunctionPointerGenericReferences.ToImmutableHashSet();
            hashCode = Hash.Combine
            (
                ActionReferences,
                ActionFromFunctionPointerGenericReferences,
                FuncReferences,
                FuncFromFunctionPointerGenericReferences
            );
        }

        public override bool Equals(object obj)
        {
            return obj is InterfaceReferenceCollection other && Equals(other);
        }

        public bool Equals(InterfaceReferenceCollection other)
        {
            return ActionReferences.SetEquals(other.ActionReferences) &&
                ActionFromFunctionPointerGenericReferences.SetEquals(other.ActionFromFunctionPointerGenericReferences) &&
                FuncReferences.SetEquals(other.FuncReferences) &&
                FuncFromFunctionPointerGenericReferences.SetEquals(other.FuncFromFunctionPointerGenericReferences);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
