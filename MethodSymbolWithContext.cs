﻿// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace NativeGenericDelegatesGenerator
{
    /// <summary>
    /// Represents an <see cref="IMethodSymbol"/> (representing a native generic delegate method, such as
    /// <c>INativeAction.FromAction</c>) paired with a <see cref="GeneratorSyntaxContext"/> (representing the method invocation
    /// site in user code).
    /// </summary>
    internal readonly struct MethodSymbolWithContext
    {
        public readonly GeneratorSyntaxContext Context;
        public readonly bool IsAction;
        public readonly IMethodSymbol MethodSymbol;

        public static ImmutableArray<MethodSymbolWithContext> GetSymbols
        (
            ImmutableArray<GeneratorSyntaxContext> contextArray,
            CancellationToken cancellationToken
        )
        {
            List<MethodSymbolWithContext> symbolsWithContext = new();
            foreach (var context in contextArray)
            {
                cancellationToken.ThrowIfCancellationRequested();
                IMethodSymbol? methodSymbol = (IMethodSymbol?)context.SemanticModel.GetSymbolInfo
                (
                    ((InvocationExpressionSyntax)context.Node).Expression,
                    cancellationToken
                ).Symbol;
                switch (methodSymbol?.Name)
                {
                    case Constants.FromActionIdentifer:
                    case Constants.FromFuncIdentifier:
                    case Constants.FromFunctionPointerIdentifier:
                        break;
                    default:
                        continue;
                }
                INamedTypeSymbol? interfaceSymbol = methodSymbol.ContainingType;
                bool isAction = interfaceSymbol?.Name == Constants.INativeActionIdentifier;
                if ((interfaceSymbol is null) || (interfaceSymbol.ContainingNamespace is null) ||
                    (interfaceSymbol.ContainingNamespace.ContainingNamespace is null) ||
                    (!interfaceSymbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace) ||
                    (interfaceSymbol.ContainingNamespace.Name != Constants.RootNamespace) ||
                    (interfaceSymbol.TypeKind != TypeKind.Interface) || !interfaceSymbol.IsGenericType ||
                    (interfaceSymbol.Arity == 0) || (!isAction && (interfaceSymbol.Name != Constants.INativeFuncIdentifier)) ||
                    (methodSymbol.Parameters.Length != (isAction ? 3 : 4)))
                {
                    continue;
                }
                if (methodSymbol.IsGenericMethod)
                {
                    if (methodSymbol.Arity != interfaceSymbol.Arity)
                    {
                        continue;
                    }
                    if (methodSymbol.TypeArguments.Where
                    (
                        x => x is not INamedTypeSymbol namedTypeArgument || namedTypeArgument.IsGenericType
                    ).Any())
                    {
                        continue;
                    }
                }
                if (interfaceSymbol.TypeArguments.Where
                (
                    x => x is not INamedTypeSymbol namedTypeArgument || namedTypeArgument.IsGenericType
                ).Any())
                {
                    continue;
                }
                symbolsWithContext.Add(new(methodSymbol, context, isAction));
            }
            return symbolsWithContext.ToImmutableArray();
        }

        public MethodSymbolWithContext(IMethodSymbol methodSymbol, GeneratorSyntaxContext context, bool isAction)
        {
            Context = context;
            IsAction = isAction;
            MethodSymbol = methodSymbol;
        }
    }
}
