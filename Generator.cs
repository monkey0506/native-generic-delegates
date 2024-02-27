﻿using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    [Generator]
    internal sealed class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static context =>
                context.AddSource(Constants.DeclarationsSourceFileName, PostInitialization.GetSource()));
            var interfaceOrMethodSymbols = InterfaceOrMethodSymbolCollection.GetSymbols(context.CompilationProvider);
            var interfaceOrMethodReferences =
                InterfaceOrMethodReferenceCollection.GetReferences(context, interfaceOrMethodSymbols);
            var methodReferencesOrDiagnostics =
                MethodReferenceCollection.GetReferencesOrDiagnostics(interfaceOrMethodReferences);
            var diagnostics = methodReferencesOrDiagnostics.Select(static (x, _) => x.Item2);
            context.RegisterSourceOutput(diagnostics, static (context, diagnostics) =>
            {
                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            });
            var methodReferences = methodReferencesOrDiagnostics.Select(static (x, _) => x.Item1);
            var classDescriptors = ClassDescriptorCollection.GetDescriptors(methodReferences);
            context.RegisterImplementationSourceOutput(classDescriptors, static (context, classDescriptors) =>
            {
                var sb = new StringBuilder
                (
$@"// <auto-generated>
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace {Constants.RootNamespace}
{{"
                );
                foreach (var classDescriptor in classDescriptors)
                {
                    _ = sb.AppendLine().Append("    ").Append(classDescriptor.SourceText);
                }
                _ = sb.Append(classDescriptors.GetOpenInterceptorsSourceText()).AppendLine("}");
                context.AddSource(Constants.SourceFileName, sb.ToString());
            });
        }
    }
}
