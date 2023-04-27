// NativeGenericDelegates
// https://github.com/monkey0506/native-generic-delegates
// Copyright(C) 2022-2023 Michael Rittenhouse monkey0506@gmail.com

// This C# project and all associated project files are hereby committed to the
// public domain pursuant to the WTFPL http://www.wtfpl.net/about/ without
// warranty of any kind, express or implied, including but not limited to
// fitness for a particular purpose. Attribution is appreciated, but not
// required.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace NativeGenericDelegatesGenerator
{
    internal static class MarshalInfo
    {
        public static ImmutableArray<string?>? GetMarshalAsCollectionFromOperation
        (
            IOperation collection,
            CancellationToken cancellationToken,
            int argumentCount,
            List<Diagnostic> diagnostics,
            Location location
        )
        {
            List<string?> marshalAsParamsStrings = new();
            if (collection is IArrayCreationOperation arrayCreation)
            {
                if (arrayCreation.Initializer is not null && (argumentCount > 0))
                {
                    foreach (var elementValue in arrayCreation.Initializer.ElementValues)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        GetMarshalAsFromOperation
                        (
                            elementValue,
                            cancellationToken,
                            argumentCount,
                            diagnostics,
                            location,
                            marshalAsParamsStrings
                        );
                        if (marshalAsParamsStrings.Count == argumentCount)
                        {
                            break;
                        }
                    }
                    for (int count = marshalAsParamsStrings.Count; count < argumentCount; ++count)
                    {
                        marshalAsParamsStrings.Add(null);
                    }
                }
                // else (no initializer or no arguments), default to no marshaling
            }
            else if (!collection.ConstantValue.HasValue) // argument is not null
            {
                if (collection is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly &&
                    fieldReference.Type is IArrayTypeSymbol)
                {
                    GetMarshalAsFromOperation
                    (
                        collection,
                        cancellationToken,
                        argumentCount,
                        diagnostics,
                        location,
                        marshalAsParamsStrings
                    );
                }
                else
                {
                    diagnostics.Add(Diagnostic.Create
                    (
                        Constants.MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor, location
                    ));
                }
            }
            return marshalAsParamsStrings.Count > 0 ? marshalAsParamsStrings.ToImmutableArray() : null;
        }

        private static void GetMarshalAsFromField
        (
            IFieldReferenceOperation fieldReference,
            CancellationToken cancellationToken,
            int argumentCount,
            List<string?> marshalAsStrings
        )
        {
            // `GetOperation` is only returning `null` for the relevant `SyntaxNode`s here, so we have to manually parse the
            // field initializer. See <https://stackoverflow.com/q/75916082/1136311>.
            bool isArray = fieldReference.Field.Type is IArrayTypeSymbol;
            SyntaxNode fieldDeclaration = fieldReference.Field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken)!;
            StringBuilder sb = new();
            bool isInsideArrayInitializer = false;
            bool isInsideNewExpression = false;
            bool isInsideObjectInitializer = false;
            var addMarshalAsString = () =>
            {
                if (sb.Length != 0)
                {
                    marshalAsStrings.Add(sb.ToString());
                    _ = sb.Clear();
                }
            };
            foreach (var syntaxToken in fieldDeclaration.DescendantTokens())
            {
                if (marshalAsStrings.Count == argumentCount)
                {
                    return;
                }
                var token = syntaxToken.ToString();
                switch (token)
                {
                    case "{":
                        if (isArray && !isInsideArrayInitializer)
                        {
                            isInsideArrayInitializer = true;
                            continue;
                        }
                        isInsideObjectInitializer = true;
                        _ = sb.Append(", ");
                        continue;
                    case "(":
                        isInsideNewExpression = true;
                        continue;
                    case ")":
                        isInsideNewExpression = false;
                        continue;
                    case "}":
                        if (isInsideObjectInitializer)
                        {
                            isInsideObjectInitializer = false;
                            addMarshalAsString();
                            continue;
                        }
                        isInsideArrayInitializer = false;
                        addMarshalAsString();
                        continue;
                    case "new":
                        addMarshalAsString();
                        continue;
                    case "null":
                        if (isArray && !isInsideArrayInitializer)
                        {
                            return;
                        }
                        marshalAsStrings.Add(null);
                        if (!isArray && !isInsideObjectInitializer)
                        {
                            return;
                        }
                        continue;
                    case ",":
                        if (isInsideObjectInitializer)
                        {
                            if (sb.Length != 0)
                            {
                                _ = sb.Append(", ");
                            }
                        }
                        else
                        {
                            addMarshalAsString();
                        }
                        continue;
                    default:
                        break;
                }
                if (isInsideNewExpression)
                {
                    _ = sb.Append(token);
                }
                else if (isInsideObjectInitializer)
                {
                    if (token == "=")
                    {
                        _ = sb.Append(" = ");
                    }
                    else
                    {
                        _ = sb.Append(token);
                    }
                }
            }
            addMarshalAsString();
        }

        private static void GetMarshalAsFromOperation
        (
            IOperation value,
            CancellationToken cancellationToken,
            int argumentCount,
            List<Diagnostic> diagnostics,
            Location location,
            List<string?> marshalAsStrings
        )
        {
            if (value.ConstantValue.HasValue) // value is null
            {
                marshalAsStrings.Add(null);
                return;
            }
            if (value is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly)
            {
                GetMarshalAsFromField(fieldReference, cancellationToken, argumentCount, marshalAsStrings);
                if (fieldReference.Field.Type is IArrayTypeSymbol && (marshalAsStrings.Count > 0))
                {
                    for (int count = marshalAsStrings.Count; count < argumentCount; ++count)
                    {
                        marshalAsStrings.Add(null);
                    }
                }
                return;
            }
            IObjectCreationOperation? objectCreation = value as IObjectCreationOperation;
            if (value is IConversionOperation conversion) // new(...) without class name
            {
                objectCreation = conversion.ChildOperations.OfType<IObjectCreationOperation>().FirstOrDefault();
            }
            if (objectCreation is null)
            {
                diagnostics.Add(Diagnostic.Create(Constants.MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor, location));
                return;
            }
            StringBuilder sb = new(objectCreation.Arguments[0].Syntax.ToString());
            if (objectCreation.Initializer is not null)
            {
                _ = sb.Append(objectCreation.Initializer.Syntax.ToString());
                _ = sb.Replace('{', ',').Replace("}", "");
            }
            marshalAsStrings.Add(sb.ToString());
        }

        public static void GetMarshalAsFromOperation
        (
            IOperation value,
            CancellationToken cancellationToken,
            List<Diagnostic> diagnostics,
            Location location,
            out string? marshalAsString
        )
        {
            List<string?> marshalAsStrings = new(1);
            GetMarshalAsFromOperation(value, cancellationToken, 1, diagnostics, location, marshalAsStrings);
            marshalAsString = marshalAsStrings.FirstOrDefault();
        }
    }
}
