using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class OpenGenericInterceptors : IEquatable<OpenGenericInterceptors>
    {
        private readonly ImmutableDictionary<Key, ImmutableHashSet<string>> attributes;
        private readonly int hashCode;
        private readonly ImmutableDictionary<Key, ImmutableList<ImplementationClass>> implementationClasses;

        public string SourceText { get; }

        public static bool operator ==(OpenGenericInterceptors? left, OpenGenericInterceptors? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(OpenGenericInterceptors? left, OpenGenericInterceptors? right) =>
            !(left == right);

        private OpenGenericInterceptors
        (
            ImmutableDictionary<Key, ImmutableHashSet<string>> attributes,
            ImmutableDictionary<Key, ImmutableList<ImplementationClass>> implementationClasses
        )
        {
            this.attributes = attributes;
            this.implementationClasses = implementationClasses;
            SourceText = GetSourceText();
            hashCode = SourceText.GetHashCode();
        }

        public override bool Equals(object? obj) => obj is OpenGenericInterceptors other && Equals(other);
        public bool Equals(OpenGenericInterceptors? other) => (other is not null) &&
            (SourceText == other.SourceText);
        public override int GetHashCode() => hashCode;

        private string GetSourceText()
        {
            var sb = new StringBuilder();
            foreach (var kv in implementationClasses)
            {
                Debug.Assert(attributes.ContainsKey(kv.Key));
                var keyMethod = kv.Key.Method;
                var constraints = keyMethod.Interceptor.Constraints;
                var interfaceName = keyMethod.Interceptor.InterfaceFullName;
                var methodHash = kv.Key.GetHashCode();
                var methodName = keyMethod.Name;
                var parameters = keyMethod.Interceptor.Parameters;
                var typeParameters = keyMethod.Interceptor.TypeParameters;
                var unsafeKeyword = keyMethod.UnsafeKeywordSourceText;
                _ = sb.Append(Constants.NewLineIndent1).AppendLine
                (
 $@"file static {unsafeKeyword}class NativeGenericDelegates_{(methodHash < 0 ? $"S{-methodHash}" : $"U{methodHash}")}
    {{"
                );
                foreach (var attribute in attributes[kv.Key])
                {
                    _ = sb.Append("        ").Append(attribute).AppendLine();
                }
                _ = sb.Append("        ").AppendLine
                (
     $@"[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {interfaceName} {methodName}{typeParameters}
        (
            {parameters}
        ){constraints}
        {{"
                );
                for (int i = 0; i < kv.Value.Count; ++i)
                {
                    var value = kv.Value[i];
                    var method = value.Method;
                    var containingInterface = method.ContainingInterface;
                    var marshallerType = value.MarshalInfo.MarshallerType;
                    var typeArguments = containingInterface.TypeArguments;
                    if (typeArguments.Count == 1)
                    {
                        if (marshallerType is null)
                        {
                            _ = sb.Append($"            if (typeof(X) == typeof({typeArguments[0]}))");
                        }
                        else
                        {
                            _ = sb.Append
                            (
                                $"            if ((typeof(X) == typeof({typeArguments[0]})) && (typeof(XMarshaller) " +
                                    $"== {marshallerType}))"
                            );
                        }
                    }
                    else
                    {
                        _ = sb.Append($"            if{Constants.NewLineIndent3}({Constants.NewLine}");
                        for (int j = 0, k = 1; j < typeArguments.Count; ++j, ++k)
                        {
                            var typeArg = containingInterface.IsUnmanaged ?
                                k <= containingInterface.BaseInterface.Arity ?
                                    $"XT{(typeArguments.Count != 2 ? k.ToString() : string.Empty)}" :
                                    $"XU{(typeArguments.Count != 2 ? (k / 2).ToString() : string.Empty)}" :
                                $"X{k}";
                            _ = sb.Append($"                (typeof({typeArg}) == typeof({typeArguments[j]}))");
                            if (k != typeArguments.Count)
                            {
                                _ = sb.AppendLine(" &&");
                            }
                            else if (marshallerType is not null)
                            {
                                _ = sb.AppendLine
                                (
                                    $" &&{Constants.NewLineIndent4}(typeof(XMarshaller) == {marshallerType})"
                                );
                            }
                        }
                        _ = sb.Append($"{Constants.NewLineIndent3})");
                    }
                    _ = sb.Append(Constants.NewLineIndent3);
                    var firstArg = method.FirstArgument;
                    if (!method.IsFromFunctionPointer)
                    {
                        firstArg = $"({containingInterface.Category}" +
                            $"{containingInterface.TypeArgumentList})(object){firstArg}";
                    }
                    if (value.MarshalInfo.StaticCallingConvention is not null)
                    {
                        _ = sb.AppendLine
                        (
         $@"{{
                return ({interfaceName})(object)(new {value.ClassName}({firstArg}));
            }}"
                        );
                    }
                    else
                    {
                        var className = value.ClassName;
                        _ = sb.AppendLine
                        (
         $@"{{
                return callingConvention switch
                {{
                    CallingConvention.Cdecl =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.Cdecl)}({firstArg})),
                    CallingConvention.StdCall =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.StdCall)}({firstArg})),
                    CallingConvention.ThisCall =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.ThisCall)}({firstArg})),
                    CallingConvention.Winapi =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.Winapi)}({firstArg})),
                    _ => throw new NotImplementedException()
                }};
            }}"
                        );
                    }
                }
                _ = sb.Append
                (
                    $"            throw new NotImplementedException();{Constants.NewLineIndent2}}}" +
                        $"{Constants.NewLineIndent1}}}{Constants.NewLine}"
                );
            }
            return sb.ToString();
        }
    }
}
