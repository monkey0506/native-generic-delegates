using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class MarshalMap : IReadOnlyDictionary<string, string?>
    {
        private readonly ImmutableDictionary<string, string?> map;

        public string? this[string key] => map[key];
        public int Count => map.Count;
        public IEnumerable<string> Keys => map.Keys;
        public IEnumerable<string?> Values => map.Values;

        public static MarshalMap? Parse
        (
            IArgumentOperation? marshalMapArgument,
            IList<Diagnostic> diagnostics,
            CancellationToken cancellationToken
        )
        {
            _ = diagnostics;
            if (marshalMapArgument is null)
            {
                return null;
            }
            var builder = ImmutableDictionary.CreateBuilder<string, string?>();
            var value = marshalMapArgument.Value;
            var invalidArgumentDiagnostic = Diagnostic.Create
            (
                Diagnostics.NGD1004_InvalidMarshalMapArgument,
                marshalMapArgument.Syntax.GetLocation(),
                marshalMapArgument.Parameter!.Name
            );
            if (value is IFieldReferenceOperation fieldReference && fieldReference.Field.IsReadOnly)
            {
                var fieldDeclaration = fieldReference.Field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken);
                var equalsValueClause = fieldDeclaration.ChildNodes().OfType<EqualsValueClauseSyntax>()
                    .FirstOrDefault();
                SemanticModel? semanticModel = equalsValueClause is not null ?
                    fieldReference.SemanticModel!.Compilation.GetSemanticModel(equalsValueClause.SyntaxTree) :
                    null;
                if (semanticModel?.GetOperation(equalsValueClause!, cancellationToken) is not
                    IFieldInitializerOperation fieldInitializer)
                {
                    diagnostics.Add(invalidArgumentDiagnostic);
                    return null;
                }
                value = fieldInitializer.Value;
            }
            IObjectCreationOperation? mapCreation = value switch
            {
                IConversionOperation conversion => conversion.Operand as IObjectCreationOperation,
                _ => value as IObjectCreationOperation
            };
            if ((mapCreation?.Initializer is null) || (mapCreation.Initializer.Initializers.Length == 0))
            {
                return new(builder.ToImmutable());
            }
            var initializers = mapCreation.Initializer.Initializers;
            foreach (var op in initializers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ITypeOfOperation? typeOf;
                IOperation? marshalAs;
                if (op is IInvocationOperation invocation)
                {
                    if (invocation.Arguments.Length != 2)
                    {
                        diagnostics.Add(invalidArgumentDiagnostic);
                        return null;
                    }
                    typeOf = invocation.Arguments[0].Value as ITypeOfOperation;
                    value = invocation.Arguments[1].Value;
                    marshalAs = value switch
                    {
                        IConversionOperation conversion => conversion.Operand as IObjectCreationOperation,
                        _ => value as IObjectCreationOperation
                    };
                }
                else
                {
                    diagnostics.Add(invalidArgumentDiagnostic);
                    return null;
                }
                if ((typeOf is null) || (marshalAs is null))
                {
                    diagnostics.Add(invalidArgumentDiagnostic);
                    return null;
                }
                var key = typeOf.TypeOperand.ToDisplayString();
                var marshalAsValue = DelegateMarshalling.Parser.GetMarshalAsFromOperation
                (
                    marshalAs,
                    marshalMapArgument.Parameter!.Name,
                    diagnostics,
                    diagnosticTypeSuffix: "",
                    cancellationToken
                );
                builder[key] = marshalAsValue;
            }
            return new(builder.ToImmutable());
        }

        private MarshalMap(ImmutableDictionary<string, string?> dictionary)
        {
            map = dictionary;
        }

        public bool ContainsKey(string key) => map.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, string?>> GetEnumerator() => map.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => map.GetEnumerator();
        public bool TryGetValue(string key, out string? value) => map.TryGetValue(key, out value);
    }
}
