using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class MarshalMap : IReadOnlyDictionary<string, string?>
    {
        private readonly ImmutableDictionary<string, string?> map;

        public string? this[string key] => map[key];
        public int Count => map.Count;
        public IEnumerable<string> Keys => map.Keys;
        public IEnumerable<string?> Values => map.Values;

        public static MarshalMap? Parse(IOperation? value)
        {
            if (value is null)
            {
                return null;
            }
            var builder = ImmutableDictionary.CreateBuilder<string, string?>();
            var mapCreation = value switch
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
                ITypeOfOperation? typeOf;
                IOperation? marshalAs;
                if (op is not IInvocationOperation invocation)
                {
                    return null;
                }
                if (invocation.Arguments.Length < 2)
                {
                    return null;
                }
                typeOf = invocation.Arguments[0].Value as ITypeOfOperation;
                value = invocation.Arguments[1].Value;
                marshalAs = value switch
                {
                    IConversionOperation conversion => conversion.Operand as IObjectCreationOperation,
                    _ => value as IObjectCreationOperation
                };
                if ((typeOf is null) || (marshalAs is null))
                {
                    return null;
                }
                var key = typeOf.TypeOperand.ToDisplayString();
                var marshalAsValue = MarshalInfo.Parser.GetMarshalAsFromOperation(marshalAs);
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
