using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal readonly struct InterfaceSymbolCollection :
        IEquatable<InterfaceSymbolCollection>,
        IEnumerable<ISymbol>
    {
        private readonly int hashCode;
        private readonly ImmutableList<ISymbol> symbols;

        public static bool operator ==(InterfaceSymbolCollection left, InterfaceSymbolCollection right) =>
            left.Equals(right);
        public static bool operator !=(InterfaceSymbolCollection left, InterfaceSymbolCollection right) =>
            !(left == right);

        public static IncrementalValueProvider<InterfaceSymbolCollection> GetSymbols
        (
            IncrementalValueProvider<Compilation> compilationProvider
        ) => compilationProvider.Select
        (
            static (compilation, cancellationToken) => new InterfaceSymbolCollection(compilation, cancellationToken)
        );

        public InterfaceSymbolCollection(Compilation compilation, CancellationToken cancellationToken)
        {
            var builder = ImmutableList.CreateBuilder<ISymbol>();
            for (int i = 0; i < Constants.InterfaceSymbolCountPerCategory; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var interfaceSymbol = compilation.GetTypeByMetadataName(Constants.Actions.MetadataNames[i])!;
                builder.Add(interfaceSymbol);
                interfaceSymbol = compilation.GetTypeByMetadataName(Constants.Funcs.MetadataNames[i])!;
                builder.Add(interfaceSymbol);
            }
            symbols = builder.ToImmutable();
            hashCode = Hash.Combine(symbols);
        }

        public override bool Equals(object? obj) => obj is InterfaceSymbolCollection other && Equals(other);

        public bool Equals(InterfaceSymbolCollection other)
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
