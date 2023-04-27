// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

// About this version
//
// v1.0.0 of this project aimed to provide dynamically instanciated Delegate
// objects that implemented one of the provided interfaces, but relied on
// classes from System.Reflection.Emit, which is incompatible with some .NET
// platforms (those using AOT compilation).
//
// This version instead aims to create classes that implement the provided
// interfaces at compile-time, using an incremental source generator. This
// version is a work-in-progress that may have bugs and is not feature
// complete. Importantly, the generated class objects are not instances of the
// Delegate or MulticastDelegate types.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NativeGenericDelegatesGenerator
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var methodSymbolsWithContext = initContext.SyntaxProvider.CreateSyntaxProvider(static (node, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (node is InvocationExpressionSyntax)
                {
                    string s = node.ToString();
                    if (s.StartsWith(Constants.INativeActionGenericIdentifier) ||
                        s.StartsWith(Constants.INativeFuncGenericIdentifier))
                    {
                        return true;
                    }
                }
                return false;
            },
            static (context, _) => context).Collect().SelectMany(MethodSymbolWithContext.GetSymbols);
            var methodSymbolsWithMarshalAndDiagnosticInfo = methodSymbolsWithContext.Select
            (
                MethodSymbolWithMarshalAndDiagnosticInfo.GetSymbol
            );
            var diagnostics = methodSymbolsWithMarshalAndDiagnosticInfo.Where(static x => x.Diagnostics is not null).Select
            (
                static (x, _) => x.Diagnostics
            );
            initContext.RegisterSourceOutput(diagnostics, static (source, diagnostics) =>
            {
                foreach (var diagnostic in diagnostics!)
                {
                    source.ReportDiagnostic(diagnostic);
                }
            });
            var methodSymbolsWithMarshalInfo = methodSymbolsWithMarshalAndDiagnosticInfo.Where(static x => x.Diagnostics is null)
                .Collect().SelectMany(static (symbolsWithMarshalInfo, cancellationToken) =>
            {
                // this hash set will ensure each combination of a method symbol and marshaling behavior are unique
                // if the method is the generic FromFunctionPointer then the IMethodSymbol is used for comparison, otherwise the
                // INamedTypeSymbol (e.g., INativeAction<...>) is used for comparison
                HashSet<MethodSymbolWithMarshalInfo> infoSet = new();
                foreach (var symbolWithMarshalInfo in symbolsWithMarshalInfo)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _ = infoSet.Add(symbolWithMarshalInfo);
                }
                return infoSet.ToImmutableArray();
            });
            var infos = methodSymbolsWithMarshalInfo.Collect()
                .SelectMany(static (methodSymbolsWithMarshalInfo, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                List<NativeGenericDelegateInfo> nativeGenericDelegateInfos = new(methodSymbolsWithMarshalInfo.Length);
                RuntimeMarshalAsAttributeArrayCollection marshalAsArrayCollection =
                    new(new RuntimeMarshalAsAttributeCollection());
                foreach (var methodSymbolWithMarshalInfo in methodSymbolsWithMarshalInfo)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    nativeGenericDelegateInfos.Add
                    (
                        new(methodSymbolWithMarshalInfo, cancellationToken, marshalAsArrayCollection)
                    );
                }
                return nativeGenericDelegateInfos.ToImmutableArray();
            }).Select(static (info, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return new NativeGenericDelegateConcreteClassInfo(in info);
            }).Collect();
            initContext.RegisterPostInitializationOutput(static (context) =>
                context.AddSource(Constants.DeclarationsSourceFileName, PostInitialization.GetSource()));
            initContext.RegisterSourceOutput(infos, static (context, infos) =>
            {
                PartialImplementations partialImplementations = new(context, infos);
                context.AddSource(Constants.SourceFileName, partialImplementations.GetSource());
            });
        }
    }
}
