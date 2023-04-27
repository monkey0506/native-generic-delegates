using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NativeGenericDelegatesGenerator
{
    internal sealed class RuntimeMarshalAsAttributeArrayCollection
    {
        private readonly Dictionary<int, string> attributes = new();
        private readonly Dictionary<string, int> lookup = new();

        public RuntimeMarshalAsAttributeCollection MarshalAsAttributeCollection { get; }

        public RuntimeMarshalAsAttributeArrayCollection(RuntimeMarshalAsAttributeCollection marshalAsAttributeCollection)
        {
            MarshalAsAttributeCollection = marshalAsAttributeCollection;
        }

        private int AddOrLookup(string value)
        {
            if (lookup.TryGetValue(value, out int index))
            {
                return index;
            }
            index = lookup.Count;
            attributes[index] = value;
            lookup[value] = index;
            return index;
        }

        public int AddOrLookup(ImmutableArray<string?>? attributeStrings)
        {
            if (attributeStrings is null)
            {
                return AddOrLookup("null");
            }
            StringBuilder sb = new($"new MarshalAsAttribute?[] {{ ");
            for (int i = 0; i < attributeStrings.Value.Length; ++i)
            {
                int index = MarshalAsAttributeCollection.AddOrLookup(attributeStrings.Value[i]);
                _ = sb.Append($"MarshalInfo.Attributes[{index}]");
                if ((i + 1) < attributeStrings.Value.Length)
                {
                    _ = sb.Append(", ");
                }
            }
            _ = sb.Append(" }");
            return AddOrLookup(sb.ToString());
        }

        public string ToRuntimeString()
        {
            StringBuilder sb = new($@"new MarshalAsAttribute?[]?[]
        {{
");
            foreach (var kv in attributes)
            {
                _ = sb.AppendLine($"            {kv.Value},");
            }
            _ = sb.Append("        }");
            return sb.ToString();
        }
    }
}
