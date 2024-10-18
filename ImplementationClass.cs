using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClass : IEquatable<ImplementationClass>
    {
        private readonly int hashCode;

        public ClassID ID { get; }
        public string ClassName { get; }
        public ClosedGenericInterceptor? Interceptor { get; }
        public MarshalInfo MarshalInfo { get; }
        public MethodDescriptor Method { get; }
        public string SourceText { get; }

        public ImplementationClass
        (
            OpenGenericInterceptors.Builder openGenericInterceptorsBuilder,
            MethodDescriptor method,
            bool isInterfaceOrMethodOpenGeneric,
            MarshalInfo marshalInfo,
            int invocationArgumentCount,
            InterceptedLocation location,
            IReadOnlyList<MethodReference> methodReferences
        )
        {
            var category = method.ContainingInterface.Category;
            ID = new(method, invocationArgumentCount, marshalInfo, !isInterfaceOrMethodOpenGeneric);
            ClassName = $"Native{category}_{ID}";
            MarshalInfo = marshalInfo;
            Method = method;
            if (!isInterfaceOrMethodOpenGeneric)
            {
                Interceptor = new ClosedGenericInterceptor(method, this, methodReferences);
            }
            else
            {
                openGenericInterceptorsBuilder.Add(this, location, methodReferences);
            }
            SourceText = GetSourceText();
            hashCode = SourceText.GetHashCode();
        }

        public override bool Equals(object? obj) => obj is ImplementationClass other && Equals(other);
        public bool Equals(ImplementationClass? other) => (other is not null) && (SourceText == other.SourceText);
        public override int GetHashCode() => hashCode;

        private string GetFromDelegateConstructor(string classSuffix)
        {
            var firstParam = Method.FirstParameterName;
            return
     $@"internal {ClassName}{classSuffix}({Method.FirstParameterType} {firstParam})
        {{
            ArgumentNullException.ThrowIfNull({firstParam});
            handler = (Handler)Delegate.CreateDelegate(typeof(Handler), {firstParam}.Target, {firstParam}.Method);
            functionPtr = Marshal.GetFunctionPointerForDelegate(handler);
        }}";
        }

        private string GetFromFunctionPointerConstructor(string classSuffix)
        {
            return
     $@"internal {ClassName}{classSuffix}(nint functionPtr)
        {{
            if (functionPtr == nint.Zero)
            {{
                throw new ArgumentNullException(nameof(functionPtr));
            }}
            handler = Marshal.GetDelegateForFunctionPointer<Handler>(functionPtr);
            this.functionPtr = functionPtr;
        }}";
        }

        private string GetInvokeParameters()
        {
            var invokeParameterCount = Method.ContainingInterface.InvokeParameterCount;
            if (invokeParameterCount == 0)
            {
                return "()";
            }
            var marshalParamsAs = MarshalInfo.MarshalParamsAs ?? [];
            var typeArguments = Method.ContainingInterface.TypeArguments;
            var sb = new StringBuilder($"{Constants.NewLineIndent2}({Constants.NewLineIndent3}");
            for (int i = 0, j = 1; i < invokeParameterCount; ++i, ++j)
            {
                if ((i < marshalParamsAs.Count) && (marshalParamsAs[i] is not null))
                {
                    _ = sb.Append($"[MarshalAs({marshalParamsAs[i]})]{Constants.NewLineIndent3}");
                }
                _ = sb.Append($"{typeArguments[i]} t{(invokeParameterCount == 1 ? string.Empty : j.ToString())}");
                if (j < invokeParameterCount)
                {
                    _ = sb.Append($",{Constants.NewLineIndent3}");
                }
                else
                {
                    _ = sb.AppendLine();
                }
            }
            _ = sb.Append("        )");
            return sb.ToString();
        }

        private string GetSourceText()
        {
            if (MarshalInfo.StaticCallingConvention is not null)
            {
                return GetSourceText(MarshalInfo.StaticCallingConvention.Value);
            }
            var interceptor = Interceptor?.SourceText;
            if (interceptor is not null)
            {
                interceptor =
 $@"
    file static class {ClassName}
    {{{interceptor}
    }}";
            }
            return
 $@"{GetSourceText(CallingConvention.Cdecl, $"_{nameof(CallingConvention.Cdecl)}")}
    {GetSourceText(CallingConvention.StdCall, $"_{nameof(CallingConvention.StdCall)}")}
    {GetSourceText(CallingConvention.ThisCall, $"_{nameof(CallingConvention.ThisCall)}")}
    {GetSourceText(CallingConvention.Winapi, $"_{nameof(CallingConvention.Winapi)}")}{interceptor ?? string.Empty}";
        }

        private string GetSourceText(CallingConvention callingConvention, string? classSuffix = null)
        {
            classSuffix ??= string.Empty;
            var constructor = Method.IsFromFunctionPointer ?
                GetFromFunctionPointerConstructor(classSuffix) :
                GetFromDelegateConstructor(classSuffix);
            var interfaceFullName = Method.ContainingInterface.FullName;
            string? baseInterfaceFullName;
            string? unmanagedProperties;
            if (Method.ContainingInterface.IsUnmanaged)
            {
                var typeArguments = Method.ContainingInterface.TypeArguments;
                var baseTypeArguments = typeArguments.Take(typeArguments.Count / 2);
                var baseTypeArgumentList = $"<{string.Join(", ", baseTypeArguments)}>";
                baseInterfaceFullName =
                    $"{Method.ContainingInterface.Name.Replace("Unmanaged", "Native")}{baseTypeArgumentList}";
                var unmanagedTypeArgumentList = Method.ContainingInterface.UnmanagedTypeArgumentList;
                unmanagedProperties =
$@"
        
#if UNSAFE
        public unsafe delegate* unmanaged[Cdecl]{unmanagedTypeArgumentList} AsCdeclPtr
        {{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => {GetPointerSourceText(callingConvention, CallingConvention.Cdecl, unmanagedTypeArgumentList)};
        }}
        
        public unsafe delegate* unmanaged[Stdcall]{unmanagedTypeArgumentList} AsStdCallPtr
        {{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => {GetPointerSourceText(callingConvention, CallingConvention.StdCall, unmanagedTypeArgumentList)};
        }}
        
        public unsafe delegate* unmanaged[Thiscall]{unmanagedTypeArgumentList} AsThisCallPtr
        {{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => {GetPointerSourceText(callingConvention, CallingConvention.ThisCall, unmanagedTypeArgumentList)};
        }}
#endif // UNSAFE";
            }
            else
            {
                baseInterfaceFullName = interfaceFullName;
                unmanagedProperties = string.Empty;
            }
            var invokeParameterCount = Method.ContainingInterface.InvokeParameterCount;
            var invokeParameters = GetInvokeParameters();
            var returnMarshalAsAttribute = MarshalInfo.MarshalReturnAs is not null ?
                $"[return: MarshalAs({MarshalInfo.MarshalReturnAs})]{Constants.NewLineIndent2}" :
                string.Empty;
            var returnKeyword = Method.ContainingInterface.ReturnKeyword;
            var returnType = Method.ContainingInterface.ReturnType;
            var interceptor = MarshalInfo.StaticCallingConvention is not null ?
                Interceptor?.SourceText ?? string.Empty :
                string.Empty;
            if (interceptor != string.Empty)
            {
                interceptor = $"{Constants.NewLineIndent2}{interceptor}";
            }
            return
 $@"file sealed class {ClassName}{classSuffix} : {interfaceFullName}
    {{
        private readonly Handler handler;
        private readonly nint functionPtr;
        
        public CallingConvention CallingConvention => CallingConvention.{callingConvention};
        
        [UnmanagedFunctionPointer(CallingConvention.{callingConvention})]
        {returnMarshalAsAttribute}public delegate {returnType} Handler{invokeParameters};{unmanagedProperties}
        
        {constructor}
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint GetFunctionPointer() => functionPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        {returnMarshalAsAttribute}public {returnType} Invoke{invokeParameters}
        {{
            {returnKeyword}handler({Constants.Arguments[invokeParameterCount]});
        }}
        
        object {baseInterfaceFullName}.Target => handler.Target;
        MethodInfo {baseInterfaceFullName}.Method => handler.Method;{interceptor}
    }}
    ";
        }

        private static string GetPointerSourceText
        (
            CallingConvention actual,
            CallingConvention expected,
            string typeArgumentList
        )
        {
            if (actual == CallingConvention.Winapi)
            {
                return expected switch
                {
                    CallingConvention.Cdecl =>
                        $"NativeGenericDelegates.PlatformDefaultCallingConvention == CallingConvention.Cdecl ?" +
                            Constants.NewLineIndent4 +
                            GetPointerSourceText(CallingConvention.Cdecl, expected, typeArgumentList) +
                            $" : {Constants.NewLineIndent4}null",
                    CallingConvention.StdCall =>
                        $"NativeGenericDelegates.PlatformDefaultCallingConvention == CallingConvention.StdCall ?" +
                        Constants.NewLineIndent4 +
                        GetPointerSourceText(CallingConvention.StdCall, expected, typeArgumentList) +
                        $" : {Constants.NewLineIndent4}null",
                    _ => "null",
                };
            }
            if (actual != expected)
            {
                return "null";
            }
            var callingConvention = actual switch
            {
                CallingConvention.Cdecl => "Cdecl",
                CallingConvention.StdCall => "Stdcall",
                CallingConvention.ThisCall => "Thiscall",
                _ => throw new UnreachableException()
            };
            return $"(delegate* unmanaged[{callingConvention}]{typeArgumentList})functionPtr";
        }
    }
}
