using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct MarshalInfo
    {
        private static class Parser
        {
            public static void GetMarshalAsCollectionFromOperation
            (
                IOperation collection,
                string parameterName,
                int collectionLength,
                List<Diagnostic> diagnostics,
                out IReadOnlyList<string?>? marshalAsStrings,
                CancellationToken cancellationToken
            )
            {
                if ((collectionLength <= 0) || collection.ConstantValue.HasValue)
                {
                    // empty collection or collection operation is "null" in source
                    marshalAsStrings = null;
                    return;
                }
                List<string?> results = [];
                if (collection is IArrayCreationOperation arrayCreation)
                {
                    // new MarshalAsAttribute[] { ... }
                    if (arrayCreation.Initializer is not null)
                    {
                        foreach (var elementValue in arrayCreation.Initializer.ElementValues)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            GetMarshalAsFromOperation
                            (
                                elementValue,
                                parameterName,
                                diagnostics,
                                diagnosticTypeSuffix: "[]",
                                out string? result,
                                cancellationToken
                            );
                            results.Add(result);
                            if (results.Count == collectionLength)
                            {
                                break;
                            }
                        }
                    }
                    // else, no initializer or no arguments, default to no marshalling
                }
                else if (collection is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly &&
                    fieldReference.Type is IArrayTypeSymbol)
                {
                    // readonly field of MarshalAsAttribute[]
                    GetMarshalAsFromField
                    (
                        fieldReference,
                        parameterName,
                        collectionLength,
                        diagnostics,
                        out object? marshalAs,
                        cancellationToken
                    );
                    marshalAsStrings = marshalAs as IReadOnlyList<string?>;
                    return;
                }
                else if (collection is IConversionOperation conversion && (conversion.Operand.Kind == OperationKind.None))
                {
                    // without API support, assume that this is a C# 12 collection expression
                    // see <https://github.com/dotnet/roslyn/issues/66418>
                    // support for this syntax is intentionally limited - for example, the spread operator isn't supported
                    var children = conversion.Descendants().Select(x =>
                    {
                        return x switch
                        {
                            ILiteralOperation => x,
                            IObjectCreationOperation => x,
                            IConversionOperation conversion =>
                                conversion.ChildOperations.OfType<IObjectCreationOperation>().FirstOrDefault(),
                            _ => null
                        };
                    }).Where(x => x is not null);
                    if (!children.Any())
                    {
                        diagnostics.Add
                        (
                            Diagnostic.Create
                            (
                                Constants.Diagnostics.NGD1001,
                                collection.Syntax.GetLocation(),
                                "[]",
                                parameterName
                            )
                        );
                    }
                    foreach (var child in children)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        GetMarshalAsFromOperation
                        (
                            child!,
                            parameterName,
                            diagnostics,
                            diagnosticTypeSuffix: "[]",
                            out string? result,
                            cancellationToken
                        );
                        results.Add(result);
                        if (results.Count == collectionLength)
                        {
                            break;
                        }
                    }
                }
                else // unknown operation - report diagnostic
                {
                    diagnostics.Add
                    (
                        Diagnostic.Create(Constants.Diagnostics.NGD1001, collection.Syntax.GetLocation(), "[]", parameterName)
                    );
                }
                if (results.Count > 0)
                {
                    for (int count = results.Count; count < collectionLength; ++count)
                    {
                        results.Add(null);
                    }
                }
                marshalAsStrings = results.Count > 0 ? results.AsReadOnly() : null;
            }

            private static void GetMarshalAsFromField
            (
                IFieldReferenceOperation fieldReference,
                string parameterName,
                int collectionLength,
                List<Diagnostic> diagnostics,
                out object? marshalAs,
                CancellationToken cancellationToken
            )
            {
                marshalAs = null;
                var fieldDeclaration = fieldReference.Field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken);
                var equalsValueClause = fieldDeclaration.ChildNodes().OfType<EqualsValueClauseSyntax>().FirstOrDefault();
                if
                (
                    equalsValueClause is null ||
                    fieldReference.SemanticModel!.GetOperation(equalsValueClause, cancellationToken)
                        is not IFieldInitializerOperation fieldInitializer
                )
                {
                    return;
                }
                bool isArray = fieldReference.Field.Type is IArrayTypeSymbol;
                if (isArray)
                {
                    GetMarshalAsCollectionFromOperation
                    (
                        fieldInitializer.Value,
                        parameterName,
                        collectionLength,
                        diagnostics,
                        out IReadOnlyList<string?>? marshalAsStrings,
                        cancellationToken
                    );
                    marshalAs = marshalAsStrings;
                }
                else
                {
                    GetMarshalAsFromOperation
                    (
                        fieldInitializer.Value,
                        parameterName,
                        diagnostics,
                        diagnosticTypeSuffix: "",
                        out string? marshalAsString,
                        cancellationToken
                    );
                    marshalAs = marshalAsString;
                }
            }

            public static void GetMarshalAsFromOperation
            (
                IOperation value,
                string parameterName,
                List<Diagnostic> diagnostics,
                string diagnosticTypeSuffix,
                out string? marshalAsString,
                CancellationToken cancellationToken
            )
            {
                if (value.ConstantValue.HasValue) // value in source is "null"
                {
                    marshalAsString = null;
                    return;
                }
                if (value is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly &&
                    fieldReference.Type is not IArrayTypeSymbol)
                {
                    GetMarshalAsFromField
                    (
                        fieldReference,
                        parameterName,
                        1,
                        diagnostics,
                        out object? marshalAs,
                        cancellationToken
                    );
                    marshalAsString = marshalAs as string;
                    return;
                }
                IObjectCreationOperation? objectCreation = value switch
                {
                    IConversionOperation conversion =>
                        conversion.ChildOperations.OfType<IObjectCreationOperation>().FirstOrDefault(),
                    _ => value as IObjectCreationOperation
                };
                if (objectCreation is null)
                {
                    marshalAsString = null;
                    diagnostics.Add
                    (
                        Diagnostic.Create
                        (
                            Constants.Diagnostics.NGD1001,
                            value.Syntax.GetLocation(),
                            diagnosticTypeSuffix,
                            parameterName
                        )
                    );
                    return;
                }
                var sb = new StringBuilder(objectCreation.Arguments[0].Syntax.ToString());
                if (objectCreation.Initializer is not null)
                {
                    _ = sb.Append(objectCreation.Initializer.Syntax.ToString())
                        .Replace('{', ',')
                        .Replace("}", "");
                    int i = sb.Length - 1;
                    for ( ; (i >= 0) && char.IsWhiteSpace(sb[i]); --i) { }
                    sb.Length = i + 1;
                }
                marshalAsString = sb.ToString();
            }

            public static IReadOnlyList<string?>? GetMarshalParamsAs
            (
                IArgumentOperation? marshalParamsAsArgument,
                int invokeParamsCount,
                List<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            )
            {
                if (marshalParamsAsArgument is null)
                {
                    return null;
                }
                GetMarshalAsCollectionFromOperation
                (
                    marshalParamsAsArgument.Value,
                    marshalParamsAsArgument.Parameter!.Name,
                    invokeParamsCount,
                    diagnostics,
                    out var result,
                    cancellationToken
                );
                return result;
            }

            public static string? GetMarshalReturnAs
            (
                IArgumentOperation? marshalReturnAsArgument,
                List<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            )
            {
                if (marshalReturnAsArgument is null)
                {
                    return null;
                }
                GetMarshalAsFromOperation
                (
                    marshalReturnAsArgument.Value,
                    marshalReturnAsArgument.Parameter!.Name,
                    diagnostics,
                    diagnosticTypeSuffix: "",
                    out var result,
                    cancellationToken
                );
                return result;
            }
        }
    }
}
