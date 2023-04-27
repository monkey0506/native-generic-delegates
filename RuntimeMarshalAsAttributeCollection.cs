using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeGenericDelegatesGenerator
{
    /// <summary>
    /// Represents a unique set of <see cref="MarshalAsAttribute"/>s at runtime.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class generates source that will represent teh attributes at runtime. These attributes are then used at runtime for
    /// the type comparisons required to create a native generic delegate, including the runtime array comparisons generated
    /// using a <see cref="RuntimeMarshalAsAttributeArrayCollection"/> that references this collection.
    /// </para>
    /// </remarks>
    internal sealed class RuntimeMarshalAsAttributeCollection
    {
        private readonly Dictionary<int, string> attributes = new();
        private readonly Dictionary<string, int> lookup = new();

        public string? this[int index] => attributes[index];

        public int AddOrLookup(string? attribute)
        {
            if (attribute is null)
            {
                attribute = "null";
            }
            if (lookup.TryGetValue(attribute, out int index))
            {
                return index;
            }
            index = lookup.Count;
            attributes[index] = attribute;
            lookup[attribute] = index;
            return index;
        }

        private static string GetMarshalAsAttributeString(string value)
        {
            if (value == "null")
            {
                return value;
            }
            int index = value.IndexOf(',');
            if (index == -1)
            {
                return $"new MarshalAsAttribute({value})";
            }
            string head = value.Substring(0, index);
            string tail = value.Substring(index + 2);
            return $"new MarshalAsAttribute({head}) {{ {tail} }}";
        }

        public string ToRuntimeString()
        {
            StringBuilder sb = new($@"new MarshalAsAttribute?[]
        {{
");
            foreach (var kv in attributes)
            {
                _ = sb.AppendLine($"            {GetMarshalAsAttributeString(kv.Value)},");
            }
            _ = sb.Append("        }");
            return sb.ToString();
        }
    }
}
