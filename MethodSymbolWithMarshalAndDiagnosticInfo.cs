// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace NativeGenericDelegatesGenerator
{
    internal readonly struct MethodSymbolWithMarshalAndDiagnosticInfo
    {
        public readonly ImmutableArray<Diagnostic>? Diagnostics;
        private readonly MethodSymbolWithMarshalInfo methodSymbolWithMarshalInfo;
        public ImmutableArray<string?>? MarshalParamsAs => methodSymbolWithMarshalInfo.MarshalParamsAs;
        public string? MarshalReturnAs => methodSymbolWithMarshalInfo.MarshalReturnAs;
        public IMethodSymbol MethodSymbol => methodSymbolWithMarshalInfo.MethodSymbol;

        public static implicit operator MethodSymbolWithMarshalInfo(MethodSymbolWithMarshalAndDiagnosticInfo info) =>
            info.methodSymbolWithMarshalInfo;

        public static MethodSymbolWithMarshalAndDiagnosticInfo GetSymbol
        (
            MethodSymbolWithContext methodSymbolWithContext,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            var context = methodSymbolWithContext.Context;
            IMethodSymbol methodSymbol = methodSymbolWithContext.MethodSymbol;
            IArgumentOperation? marshalParamsAsArgument = null;
            IArgumentOperation? marshalReturnAsArgument = null;
            foreach (var argumentNode in context.Node.DescendantNodes().OfType<ArgumentSyntax>())
            {
                var argument = (IArgumentOperation?)context.SemanticModel.GetOperation(argumentNode, cancellationToken);
                switch (argument?.Parameter?.Name)
                {
                    case "marshalParamsAs":
                        marshalParamsAsArgument = argument;
                        break;
                    case "marshalReturnAs":
                        marshalReturnAsArgument = argument;
                        break;
                    default:
                        break;
                }
            }
            string? marshalReturnAsString = null;
            ImmutableArray<string?>? marshalParamsAsStrings = null;
            List<Diagnostic> diagnostics = new();
            Location location = context.Node.GetLocation();
            if (marshalReturnAsArgument is not null)
            {
                MarshalInfo.GetMarshalAsFromOperation
                (
                    marshalReturnAsArgument.Value,
                    cancellationToken,
                    diagnostics,
                    location,
                    out marshalReturnAsString
                );
            }
            if (marshalParamsAsArgument is not null)
            {
                marshalParamsAsStrings = MarshalInfo.GetMarshalAsCollectionFromOperation
                (
                    marshalParamsAsArgument.Value,
                    cancellationToken,
                    methodSymbol.ContainingType!.Arity - (methodSymbolWithContext.IsAction ? 0 : 1),
                    diagnostics,
                    location
                );
            }
            return new MethodSymbolWithMarshalAndDiagnosticInfo
            (
                methodSymbol,
                marshalReturnAsString,
                marshalParamsAsStrings,
                diagnostics.Count > 0 ? diagnostics.ToImmutableArray() : null
            );
        }

        public MethodSymbolWithMarshalAndDiagnosticInfo
        (
            IMethodSymbol methodSymbol,
            string? marshalReturnAs,
            ImmutableArray<string?>? marshalParamsAs,
            ImmutableArray<Diagnostic>? diagnostics
        )
        {
            Diagnostics = diagnostics;
            methodSymbolWithMarshalInfo = new(methodSymbol, marshalReturnAs, marshalParamsAs);
        }
    }
}
