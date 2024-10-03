using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class DelegateMarshalling
    {
        private static class Parser
        {
            private static IReadOnlyList<string?> GetMarshalAsCollectionFromArrayInitializer
            (
                IArrayInitializerOperation initializer,
                string parameterName,
                int collectionLength,
                IList<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            )
            {
                var results = new List<string?>(collectionLength);
                foreach (var elementValue in initializer.ElementValues)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = GetMarshalAsFromOperation
                    (
                        elementValue,
                        parameterName,
                        diagnostics,
                        diagnosticTypeSuffix: "[]",
                        cancellationToken
                    );
                    results.Add(result);
                    if (results.Count == collectionLength)
                    {
                        break;
                    }
                }
                return results.AsReadOnly();
            }

            private static IReadOnlyList<string?>? GetMarshalAsCollectionFromCollectionExpression
            (
                ICollectionExpressionOperation collectionExpression,
                string parameterName,
                int collectionLength,
                IList<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            )
            {
                if (collectionExpression.Elements.Any(x => x is ISpreadOperation))
                {
                    diagnostics.Add
                    (
                        Diagnostic.Create
                        (
                            Diagnostics.NGD1003_MarshalAsArgumentSpreadElementNotSupported,
                            collectionExpression.Syntax.GetLocation(),
                            parameterName
                        )
                    );
                    return null;
                }
                var elements = collectionExpression.Elements.Select(static x => x switch
                {
                    ILiteralOperation => x,
                    IObjectCreationOperation => x,
                    IConversionOperation conversion =>
                        conversion.Operand as IObjectCreationOperation,
                    _ => null
                }).Where(static x => x is not null);
                if (!elements.Any())
                {
                    diagnostics.Add
                    (
                        Diagnostic.Create
                        (
                            Diagnostics.NGD1001_MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor,
                            collectionExpression.Syntax.GetLocation(),
                            "[]",
                            parameterName
                        )
                    );
                }
                var results = new List<string?>(collectionLength);
                foreach (var element in elements)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = GetMarshalAsFromOperation
                    (
                        element!,
                        parameterName,
                        diagnostics,
                        diagnosticTypeSuffix: "[]",
                        cancellationToken
                    );
                    results.Add(result);
                    if (results.Count == collectionLength)
                    {
                        break;
                    }
                }
                return results.AsReadOnly();
            }

            private static IReadOnlyList<string?>? GetMarshalAsCollectionFromOperation
            (
                IOperation collection,
                string parameterName,
                int collectionLength,
                IList<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            )
            {
                if ((collectionLength <= 0) || collection.ConstantValue.HasValue)
                {
                    // empty collection or collection operation is `null` literal in source
                    return null;
                }
                if (collection is IArrayCreationOperation arrayCreation)
                {
                    // new MarshalAsAttribute[] { ... }
                    if (arrayCreation.Initializer is not null)
                    {
                        return GetMarshalAsCollectionFromArrayInitializer
                        (
                            arrayCreation.Initializer,
                            parameterName,
                            collectionLength,
                            diagnostics,
                            cancellationToken
                        );
                    }
                    // else, no initializer or no arguments, default to no marshalling
                }
                else if (collection is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly &&
                    fieldReference.Type is IArrayTypeSymbol)
                {
                    // readonly field of MarshalAsAttribute[]
                    return (IReadOnlyList<string?>?)GetMarshalAsFromField
                    (
                        fieldReference,
                        parameterName,
                        collectionLength,
                        diagnostics,
                        cancellationToken
                    );
                }
                else
                {
                    var collectionExpression = collection switch
                    {
                        IConversionOperation conversion => conversion.Operand as ICollectionExpressionOperation,
                        ICollectionExpressionOperation op => op,
                        _ => null
                    };
                    if (collectionExpression is not null)
                    {
                        return GetMarshalAsCollectionFromCollectionExpression
                        (
                            collectionExpression,
                            parameterName,
                            collectionLength,
                            diagnostics,
                            cancellationToken
                        );
                    }
                    else // unknown operation - report diagnostic
                    {
                        diagnostics.Add
                        (
                            Diagnostic.Create
                            (
                                Diagnostics.NGD1001_MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor,
                                collection.Syntax.GetLocation(),
                                "[]",
                                parameterName
                            )
                        );
                    }
                }
                return null;
            }

            private static string? GetMarshalAsFromOperation
            (
                IOperation value,
                string parameterName,
                IList<Diagnostic> diagnostics,
                string diagnosticTypeSuffix,
                CancellationToken cancellationToken
            )
            {
                if (value.ConstantValue.HasValue) // value in source is `null` literal
                {
                    return null;
                }
                if (value is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly &&
                    fieldReference.Type is not IArrayTypeSymbol)
                {
                    return (string?)GetMarshalAsFromField
                    (
                        fieldReference,
                        parameterName,
                        collectionLength: 1,
                        diagnostics,
                        cancellationToken
                    );
                }
                IObjectCreationOperation? objectCreation = value switch
                {
                    IConversionOperation conversion =>
                        conversion.ChildOperations.OfType<IObjectCreationOperation>().FirstOrDefault(),
                    _ => value as IObjectCreationOperation
                };
                if (objectCreation is null)
                {
                    diagnostics.Add
                    (
                        Diagnostic.Create
                        (
                            Diagnostics.NGD1001_MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor,
                            value.Syntax.GetLocation(),
                            diagnosticTypeSuffix,
                            parameterName
                        )
                    );
                    return null;
                }
                var sb = new StringBuilder(objectCreation.Arguments[0].Syntax.ToString());
                if (objectCreation.Initializer is not null)
                {
                    _ = sb.Append(objectCreation.Initializer.Syntax.ToString())
                        .Replace('{', ',')
                        .Replace("]", string.Empty);
                    int i = sb.Length - 1;
                    for ( ; (i >= 0) && char.IsWhiteSpace(sb[i]); --i) { }
                    sb.Length = i + 1;
                }
                return sb.ToString();
            }

            private static object? GetMarshalAsFromField
            (
                IFieldReferenceOperation fieldReference,
                string parameterName,
                int collectionLength,
                IList<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            )
            {
                var fieldDeclaration = fieldReference.Field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken);
                var equalsValueClause = fieldDeclaration.ChildNodes().OfType<EqualsValueClauseSyntax>()
                    .FirstOrDefault();
                if (equalsValueClause is null ||
                    fieldReference.SemanticModel!.GetOperation(equalsValueClause, cancellationToken) is not
                    IFieldInitializerOperation fieldInitializer)
                {
                    return null;
                }
                bool isArray = fieldReference.Field.Type is IArrayTypeSymbol;
                if (isArray)
                {
                    return GetMarshalAsCollectionFromOperation
                    (
                        fieldInitializer.Value,
                        parameterName,
                        collectionLength,
                        diagnostics,
                        cancellationToken
                    );
                }
                return GetMarshalAsFromOperation
                (
                    fieldInitializer.Value,
                    parameterName,
                    diagnostics,
                    diagnosticTypeSuffix: string.Empty,
                    cancellationToken
                );
            }

            public static IReadOnlyList<string?>? GetMarshalParamsAs
            (
                IArgumentOperation? marshalParamsAsArgument,
                int invokeParamCount,
                IList<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            ) => marshalParamsAsArgument is not null ?
                GetMarshalAsCollectionFromOperation
                (
                    marshalParamsAsArgument.Value,
                    marshalParamsAsArgument.Parameter!.Name,
                    invokeParamCount,
                    diagnostics,
                    cancellationToken
                ) :
                null;

            public static string? GetMarshalReturnAs
            (
                IArgumentOperation? marshalReturnAsArgument,
                IList<Diagnostic> diagnostics,
                CancellationToken cancellationToken
            ) => marshalReturnAsArgument is not null ?
                GetMarshalAsFromOperation
                (
                    marshalReturnAsArgument.Value,
                    marshalReturnAsArgument.Parameter!.Name,
                    diagnostics,
                    diagnosticTypeSuffix: string.Empty,
                    cancellationToken
                ) :
                null;
        }
    }
}
