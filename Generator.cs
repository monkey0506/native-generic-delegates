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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NativeGenericDelegatesGenerator
{
    [Generator]
    internal class Generator : IIncrementalGenerator
    {
        /// <summary>
        /// <inheritdoc cref="IIncrementalGenerator.Initialize" path="//summary"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The translation phases for generating the source for native generic delegates are:
        /// </para><para>
        /// <list type="bullet">
        /// <item>
        /// From the syntax tree, select any <see cref="InvocationExpressionSyntax"/> nodes that begin with the text
        /// "INativeAction" or "INativeFunc".
        /// </item><item>
        /// Filter the selected nodes using the <see cref="IMethodSymbol"/> to validate that the method is a native generic
        /// delegate interface method call with no open generic type parameters. The validated symbol is then paired with a <see
        /// cref="GeneratorSyntaxContext"/> in a <see cref="MethodSymbolWithContext"/>. Invalid symbols are filtered out of this
        /// translation.
        /// </item><item>
        /// Custom marshaling behavior is parsed from the user-supplied arguments (e.g., what the user passed for the
        /// <c>marshalParamsAs</c> argument). A <see cref="MethodSymbolWithMarshalAndDiagnosticInfo"/> is created which may
        /// contain a <see cref="Diagnostic"/> that will be reported in a later translation phase. If this object does contain
        /// diagnostic info, then no further translations are performed on this object except to report the diagnostic.
        /// </item><item>
        /// Created <see cref="Diagnostic"/>s, if any, will be reported at this translation phase.
        /// </item><item>
        /// The selected <see cref="MethodSymbolWithMarshalAndDiagnosticInfo"/>s that do not have any <see cref="Diagnostic"/>
        /// are filtered one additional time to create a unique set. See <see cref="MethodSymbolWithMarshalInfo"/> for details on
        /// how this unique set is formed.
        /// </item><item>
        /// Each item in the selected set is translated into a <see cref="NativeGenericDelegateInfo"/>.
        /// </item><item>
        /// Each item is translated into a <see cref="NativeGenericDelegateConcreteClassInfo"/>.
        /// </item><item>
        /// The data set is translated into a <see cref="PartialImplementations"/> object which will create the final generated
        /// source.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="initContext">
        /// <inheritdoc cref="IIncrementalGenerator.Initialize" path="//param[@name='context']"/>
        /// </param>
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
