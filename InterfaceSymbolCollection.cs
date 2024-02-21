using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterfaceSymbolCollection : IEquatable<InterfaceSymbolCollection>, IEnumerable<ISymbol>
    {
        private readonly int hashCode;
        private readonly ImmutableList<ISymbol> symbols;

        public static bool operator ==(InterfaceSymbolCollection left, InterfaceSymbolCollection right) => left.Equals(right);
        public static bool operator !=(InterfaceSymbolCollection left, InterfaceSymbolCollection right) => !(left == right);

        public static IncrementalValueProvider<InterfaceSymbolCollection> GetSymbols
        (
            IncrementalValueProvider<Compilation> compilationProvider
        )
        {
            return compilationProvider.Select
            (
                static (compilation, cancellationToken) => new InterfaceSymbolCollection(compilation, cancellationToken)
            );
        }

        public InterfaceSymbolCollection(Compilation compilation, CancellationToken cancellationToken)
        {
            var list = new List<ISymbol>(Constants.InterfaceAndGenericMethodSymbolCount);
            for (int i = 0; i < Constants.InterfaceSymbolCountPerKind; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var interfaceSymbol = compilation.GetTypeByMetadataName(Constants.Actions.MetadataNames[i])!;
                // GetMembers *seems* to be ordered, but this is not a documented part of the API
                // explicitly order the members for Equals comparisons
                var genericMethods = interfaceSymbol.GetMembers()
                    .Where(x => x is IMethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
                    .OrderBy(x => x.Name);
                list.Add(interfaceSymbol);
                list.AddRange(genericMethods);
                interfaceSymbol = compilation.GetTypeByMetadataName(Constants.Funcs.MetadataNames[i])!;
                genericMethods = interfaceSymbol.GetMembers()
                    .Where(x => x is IMethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
                    .OrderBy(x => x.Name);
                list.Add(interfaceSymbol);
                list.AddRange(genericMethods);
            }
            symbols = list.ToImmutableList();
            hashCode = Hash.Combine(symbols);
        }

        public override bool Equals(object obj)
        {
            return obj is InterfaceSymbolCollection other && Equals(other);
        }

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

        public IEnumerator<ISymbol> GetEnumerator()
        {
            return symbols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
