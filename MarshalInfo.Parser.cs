using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class MarshalInfo
    {
        internal static class Parser
        {
            private static IReadOnlyList<string?>? GetMarshalAsCollectionFromElements
            (
                IEnumerable<IOperation?>? elements,
                int collectionLength
            )
            {
                if (elements is null)
                {
                    return null;
                }
                var results = new List<string?>(collectionLength);
                foreach (var elementValue in elements)
                {
                    var result = GetMarshalAsFromOperation(elementValue);
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
                ICollectionExpressionOperation? collectionExpression,
                int collectionLength
            )
            {
                if ((collectionExpression is null) || collectionExpression.Elements.Any(x => x is ISpreadOperation))
                {
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
                return GetMarshalAsCollectionFromElements(elements, collectionLength);
            }

            private static IReadOnlyList<string?>? GetMarshalAsCollectionFromOperation
            (
                IOperation? collection,
                int collectionLength
            )
            {
                if ((collectionLength <= 0) || (collection is null) || collection.ConstantValue.HasValue)
                {
                    // empty collection or collection operation is `null` literal in source
                    return null;
                }
                return collection switch
                {
                    IArrayCreationOperation arrayCreation =>
                        GetMarshalAsCollectionFromElements(arrayCreation.Initializer?.ElementValues, collectionLength),
                    IConversionOperation conversion =>
                        GetMarshalAsCollectionFromCollectionExpression
                        (
                            conversion.Operand as ICollectionExpressionOperation,
                            collectionLength
                        ),
                    ICollectionExpressionOperation collectionExpression =>
                        GetMarshalAsCollectionFromCollectionExpression(collectionExpression, collectionLength),
                    _ => null
                };
            }

            public static string? GetMarshalAsFromOperation(IOperation? value)
            {
                var objectCreation = value switch
                {
                    IConversionOperation conversion => conversion.Operand as IObjectCreationOperation,
                    _ => value as IObjectCreationOperation
                };
                if (objectCreation is null)
                {
                    return null;
                }
                var sb = new StringBuilder(objectCreation.Arguments[0].Syntax.ToString());
                if (objectCreation.Initializer is not null)
                {
                    _ = sb.Append(objectCreation.Initializer.Syntax.ToString())
                        .Replace('{', ',')
                        .Replace("}", string.Empty);
                    int i = sb.Length - 1;
                    for ( ; (i >= 0) && char.IsWhiteSpace(sb[i]); --i) { }
                    sb.Length = i + 1;
                }
                return sb.ToString();
            }

            public static IReadOnlyList<string?>? GetMarshalParamsAs
            (
                IObjectCreationOperation? marshalParamsAs,
                int invokeParamCount
            ) => GetMarshalAsCollectionFromOperation(marshalParamsAs, invokeParamCount);

            public static string? GetMarshalReturnAs(IObjectCreationOperation? marshalReturnAs) =>
                GetMarshalAsFromOperation(marshalReturnAs);
        }
    }
}
