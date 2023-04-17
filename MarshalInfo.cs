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
        public static ImmutableArray<string?>? GetMarshalAsCollectionFromOperation(IOperation collection, CancellationToken cancellationToken, int argumentCount, List<Diagnostic> diagnostics, Location location)
        {
            List<string?> marshalAsParamsStrings = new();
            if (collection is IArrayCreationOperation arrayCreation)
            {
                var arrayLength = arrayCreation.DimensionSizes[0].ConstantValue;
                if (!arrayLength.HasValue || ((int)arrayLength.Value!) != argumentCount)
                {
                    diagnostics.Add(Diagnostic.Create(Constants.InvalidMarshalParamsAsArrayLengthDescriptor, location));
                }
                else if (arrayCreation.Initializer is not null)
                {
                    foreach (var elementValue in arrayCreation.Initializer.ElementValues)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        GetMarshalAsFromOperation(elementValue, cancellationToken, argumentCount, diagnostics, location, marshalAsParamsStrings);
                    }
                }
                // else (no initializer), default to no marshaling
            }
            else if (!collection.ConstantValue.HasValue) // argument is not null
            {
                if (collection is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly && fieldReference.Type is IArrayTypeSymbol)
                {
                    GetMarshalAsFromOperation(collection, cancellationToken, argumentCount, diagnostics, location, marshalAsParamsStrings);
                }
                else
                {
                    diagnostics.Add(Diagnostic.Create(Constants.MarshalAsArgumentMustUseObjectCreationSyntaxDescriptor, location));
                }
            }
            return marshalAsParamsStrings.Count > 0 ? marshalAsParamsStrings.ToImmutableArray() : null;
        }

        private static void GetMarshalAsFromField(IFieldReferenceOperation fieldReference, CancellationToken cancellationToken, int argumentCount, List<Diagnostic> diagnostics, Location location, List<string?> marshalAsStrings)
        {
            // `GetOperation` is only returning `null` for the relevant `SyntaxNode`s here, so we have to manually parse the field initializer
            // see <https://stackoverflow.com/q/75916082/1136311>
            bool isArray = fieldReference.Field.Type is IArrayTypeSymbol;
            SyntaxNode fieldDeclaration = fieldReference.Field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken)!;
            StringBuilder sb = new();
            bool isInsideArrayInitializer = false;
            bool isInsideNewExpression = false;
            bool isInsideObjectInitializer = false;
            bool addedArrayLengthDiagnostic = false;
            var addMarshalAsString = () =>
            {
                if (sb.Length != 0)
                {
                    marshalAsStrings.Add(sb.ToString());
                    if (isArray && !addedArrayLengthDiagnostic && marshalAsStrings.Count > argumentCount)
                    {
                        addedArrayLengthDiagnostic = true;
                        diagnostics.Add(Diagnostic.Create(Constants.InvalidMarshalParamsAsArrayLengthDescriptor, location));
                    }
                    _ = sb.Clear();
                }
            };
            foreach (var syntaxToken in fieldDeclaration.DescendantTokens())
            {
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
                    case "null!": // TODO: are `null` and `!` parsed as separate tokens?
                        marshalAsStrings.Add(null);
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

        private static void GetMarshalAsFromOperation(IOperation value, CancellationToken cancellationToken, int argumentCount, List<Diagnostic> diagnostics, Location location, List<string?> marshalAsStrings)
        {
            if (value.ConstantValue.HasValue) // value is null
            {
                return;
            }
            if (value is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly)
            {
                GetMarshalAsFromField(fieldReference, cancellationToken, argumentCount, diagnostics, location, marshalAsStrings);
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

        public static void GetMarshalAsFromOperation(IOperation value, CancellationToken cancellationToken, List<Diagnostic> diagnostics, Location location, out string? marshalAsString)
        {
            List<string?> marshalAsStrings = new(1);
            GetMarshalAsFromOperation(value, cancellationToken, 0, diagnostics, location, marshalAsStrings);
            marshalAsString = marshalAsStrings.FirstOrDefault();
        }
    }
}
