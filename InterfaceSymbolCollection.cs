using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Threading;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterfaceSymbolCollection : IEquatable<InterfaceSymbolCollection>
    {
        private readonly int hashCode;

        public readonly INamedTypeSymbol[] ActionInterfaceSymbols;
        public readonly IMethodSymbol[] ActionFromFunctionPointerGenericSymbols;
        public readonly INamedTypeSymbol[] FuncInterfaceSymbols;
        public readonly IMethodSymbol[] FuncFromFunctionPointerGenericSymbols;

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
            ActionInterfaceSymbols = new INamedTypeSymbol[17];
            ActionFromFunctionPointerGenericSymbols = new IMethodSymbol[17];
            FuncInterfaceSymbols = new INamedTypeSymbol[17];
            FuncFromFunctionPointerGenericSymbols = new IMethodSymbol[17];
            for (int i = 0; i < 17; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var action = compilation.GetTypeByMetadataName(Constants.Actions.MetadataNames[i])!;
                var actionFromFuncPointer = action.GetMembers(Constants.FromFunctionPointerIdentifier).Cast<IMethodSymbol>()
                    .Where(x => i == 0 || x.IsGenericMethod).First();
                var func = compilation.GetTypeByMetadataName(Constants.Funcs.MetadataNames[i])!;
                var funcFromFuncPointer = func.GetMembers(Constants.FromFunctionPointerIdentifier).Cast<IMethodSymbol>()
                    .Where(x => x.IsGenericMethod).First();
                ActionInterfaceSymbols[i] = action;
                ActionFromFunctionPointerGenericSymbols[i] = actionFromFuncPointer;
                FuncInterfaceSymbols[i] = func;
                FuncFromFunctionPointerGenericSymbols[i] = funcFromFuncPointer;
            }
            hashCode = Hash.Combine
            (
                ActionInterfaceSymbols,
                ActionFromFunctionPointerGenericSymbols,
                FuncInterfaceSymbols,
                FuncFromFunctionPointerGenericSymbols
            );
        }

        public override bool Equals(object obj)
        {
            return obj is InterfaceSymbolCollection other && Equals(other);
        }

        public bool Equals(InterfaceSymbolCollection other)
        {
            for (int i = 0; i < 17; ++i)
            {
                if (!SymbolEqualityComparer.Default.Equals(ActionInterfaceSymbols[i], other.ActionInterfaceSymbols[i]) ||
                    !SymbolEqualityComparer.Default.Equals
                    (
                        ActionFromFunctionPointerGenericSymbols[i],
                        other.ActionFromFunctionPointerGenericSymbols[i]
                    ) ||
                    !SymbolEqualityComparer.Default.Equals(FuncInterfaceSymbols[i], other.FuncInterfaceSymbols[i]) ||
                    !SymbolEqualityComparer.Default.Equals
                    (
                        FuncFromFunctionPointerGenericSymbols[i],
                        other.FuncFromFunctionPointerGenericSymbols[i])
                    )
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
