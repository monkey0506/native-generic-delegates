using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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
                var first = kv.Value.First();
                var firstInterface = first.Method.ContainingInterface;
                var typeParameters = firstInterface.IsUnmanaged ?
                    Constants.InterceptorUnmanagedTypeParameters[firstInterface.BaseInterfaceArity] :
                    Constants.InterceptorTypeParameters[firstInterface.Arity];
                var interfaceName = $"{firstInterface.Name}<{typeParameters}>";
                typeParameters = first.Method.Arity != 0 ?
                    $"<{typeParameters}, XMarshaller>" :
                    typeParameters.Length != 0 ?
                        $"<{typeParameters}>" :
                        typeParameters;
                var methodHash = kv.Key.GetHashCode();
                var methodName = first.Method.Name;
                var parameters = first.Method.InterceptorParameters;
                var constraints = firstInterface.IsUnmanaged ?
                    Constants.InterceptorUnmanagedTypeConstraints[firstInterface.BaseInterfaceArity] :
                    Constants.InterceptorTypeConstraints[firstInterface.Arity];
                var unsafeKeyword = first.Method.IsFromUnsafeFunctionPointer ? "unsafe " : string.Empty;
                _ = sb.AppendLine().Append("    ").AppendLine
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
                    var method = kv.Value[i].Method;
                    var typeArguments = method.ContainingInterface.TypeArguments;
                    var marshallerType = kv.Value[i].MarshalInfo.MarshallerType;
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
                                $"            if ((typeof(X) == typeof({typeArguments[0]
                                    })) && (typeof(XMarshaller) == {marshallerType}))"
                            );
                        }
                    }
                    else
                    {
                        _ = sb.Append($"            if{Constants.NewLineIndent3}({Constants.NewLine}");
                        for (int j = 0, k = 1; j < typeArguments.Count; ++j, ++k)
                        {
                            var typeArg = method.ContainingInterface.IsUnmanaged ?
                                k <= method.ContainingInterface.BaseInterfaceArity ?
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
                                _ = sb.AppendLine($" &&{Constants.NewLineIndent4}(typeof(XMarshaller) == {marshallerType})");
                            }
                        }
                        _ = sb.Append($"{Constants.NewLineIndent3})");
                    }
                    _ = sb.Append(Constants.NewLineIndent3);
                    var firstParam = method.FirstParameterName;
                    if (!method.IsFromFunctionPointer)
                    {
                        firstParam = $"({method.ContainingInterface.Category}{method.ContainingInterface.TypeArgumentList})(object){firstParam}";
                    }
                    if (kv.Value[i].MarshalInfo.StaticCallingConvention is not null)
                    {
                        var cast = method.IsFromUnsafeFunctionPointer ? "(nint)" : string.Empty;
                        _ = sb.AppendLine
                        (
         $@"{{
                return ({interfaceName})(object)(new {kv.Value[i].ClassName}({cast}{firstParam}));
            }}"
                        );
                    }
                    else
                    {
                        var className = kv.Value[i].ClassName;
                        _ = sb.AppendLine
                        (
         $@"{{
                return callingConvention switch
                {{
                    CallingConvention.Cdecl =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.Cdecl)}({firstParam})),
                    CallingConvention.StdCall =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.StdCall)}({firstParam})),
                    CallingConvention.ThisCall =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.ThisCall)}({firstParam})),
                    CallingConvention.Winapi =>
                        ({interfaceName})(object)(new {className}_{nameof(CallingConvention.Winapi)}({firstParam})),
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
