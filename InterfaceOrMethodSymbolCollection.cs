using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct InterfaceOrMethodSymbolCollection :
        IEquatable<InterfaceOrMethodSymbolCollection>,
        IEnumerable<ISymbol>
    {
        private readonly int hashCode;
        private readonly ImmutableList<ISymbol> symbols;

        public static bool operator ==
        (
            InterfaceOrMethodSymbolCollection left,
            InterfaceOrMethodSymbolCollection right
        ) =>left.Equals(right);

        public static bool operator !=
        (
            InterfaceOrMethodSymbolCollection left,
            InterfaceOrMethodSymbolCollection right
        ) => !(left == right);

        public static IncrementalValueProvider<InterfaceOrMethodSymbolCollection> GetSymbols
        (
            IncrementalValueProvider<Compilation> compilationProvider
        ) => compilationProvider.Select
        (
            static (compilation, cancellationToken) =>
                new InterfaceOrMethodSymbolCollection(compilation, cancellationToken)
        );

        public InterfaceOrMethodSymbolCollection(Compilation compilation, CancellationToken cancellationToken)
        {
            Debug.Assert
            (
                Constants.Actions.MetadataNames.Length == Constants.Funcs.MetadataNames.Length,
                $"{nameof(Constants)}: Metdata name arrays must be of same length."
            );
            var builder = ImmutableList.CreateBuilder<ISymbol>();
            IOrderedEnumerable<ISymbol>? genericMethods;
            for (int i = 0; i < Constants.Actions.MetadataNames.Length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var interfaceSymbol = compilation.GetTypeByMetadataName(Constants.Actions.MetadataNames[i])!;
                if (interfaceSymbol is not null)
                {
                    // GetMembers *seems* to be ordered, but this is not a documented part of the API
                    // explicitly order the members for Equals comparisons
                    genericMethods = interfaceSymbol.GetMembers()
                        .Where(static x => x is IMethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
                        .OrderBy(static x => x.Name);
                    builder.Add(interfaceSymbol);
                    builder.AddRange(genericMethods);
                }
                interfaceSymbol = compilation.GetTypeByMetadataName(Constants.Funcs.MetadataNames[i])!;
                genericMethods = interfaceSymbol.GetMembers()
                    .Where(static x => x is IMethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
                    .OrderBy(static x => x.Name);
                builder.Add(interfaceSymbol);
                builder.AddRange(genericMethods);
            }
            symbols = builder.ToImmutable();
            hashCode = Hash.Combine(symbols);
        }

        public override bool Equals(object? obj) => obj is InterfaceOrMethodSymbolCollection other && Equals(other);

        public bool Equals(InterfaceOrMethodSymbolCollection other)
        {
            if (symbols.Count != other.symbols.Count)
            {
                return false;
            }
            for (int i = 0; i < symbols.Count; ++i)
            {
                if (!SymbolEqualityComparer.Default.Equals(symbols[i], other.symbols[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerator<ISymbol> GetEnumerator() => symbols.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hashCode;
    }
}
