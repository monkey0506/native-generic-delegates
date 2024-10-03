using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class ImplementationClass : IEquatable<ImplementationClass>
    {
        private readonly int hashCode;

        public ClassID ID { get; }
        public string ClassName { get; }
        public ClosedGenericInterceptor? Interceptor { get; }
        public DelegateMarshalling Marshalling { get; }
        public MethodDescriptor Method { get; }
        public string SourceText { get; }

        public ImplementationClass
        (
            OpenGenericInterceptors.Builder openGenericInterceptorsBuilder,
            MethodDescriptor method,
            bool isInterfaceOrMethodOpenGeneric,
            DelegateMarshalling marshalling,
            IReadOnlyList<MethodReference> methodReferences
        )
        {
            var category = method.ContainingInterface.Category;
            ID = new(method, marshalling);
            ClassName = $"Native{category}_{ID}";
            Marshalling = marshalling;
            Method = method;
            if (!isInterfaceOrMethodOpenGeneric)
            {
                Interceptor = new ClosedGenericInterceptor(method, this, methodReferences);
            }
            else
            {
                openGenericInterceptorsBuilder.Add(this, methodReferences);
            }
            SourceText = GetSourceText();
            hashCode = SourceText.GetHashCode();
        }

        public override bool Equals(object? obj) => obj is ImplementationClass other && Equals(other);
        public bool Equals(ImplementationClass? other) => (other is not null) && (SourceText == other.SourceText);
        public override int GetHashCode() => hashCode;

        private string GetFromDelegateConstructor()
        {
            var firstParam = Method.FirstParameterName;
            return
     $@"internal {ClassName}({Method.FirstParameterType} {firstParam})
        {{
            ArgumentNullException.ThrowIfNull({firstParam});
            handler = (Handler)Delegate.CreateDelegate(typeof(Handler), {firstParam}.Target, {firstParam}.Method);
            functionPtr = Marshal.GetFunctionPointerForDelegate(handler);
        }}";
        }

        private string GetFromFunctionPointerConstructor()
        {
            return
     $@"internal {ClassName}(nint functionPtr)
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
            var marshalParamsAs = Marshalling.MarshalParamsAs;
            var typeArguments = Method.ContainingInterface.TypeArguments;
            var sb = new StringBuilder($"{Constants.NewLineIndent2}({Constants.NewLineIndent3}");
            for (int i = 0, j = 1; i < invokeParameterCount; ++i, ++j)
            {
                if (marshalParamsAs?[i] is not null)
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
            var callingConvention = Marshalling.CallingConvention;
            var constructor = Method.IsFromFunctionPointer ?
                GetFromFunctionPointerConstructor() :
                GetFromDelegateConstructor();
            var interfaceFullName = Method.ContainingInterface.FullName;
            var invokeParameterCount = Method.ContainingInterface.InvokeParameterCount;
            var invokeParameters = GetInvokeParameters();
            var returnMarshalAsAttribute = Marshalling.MarshalReturnAs is not null ?
                $"[return: MarshalAs({Marshalling.MarshalReturnAs})]{Constants.NewLineIndent2}" :
                string.Empty;
            string? returnKeyword;
            string? returnType;
            switch (Method.ContainingInterface.IsAction)
            {
                case true:
                    returnKeyword = string.Empty;
                    returnType = "void";
                    break;
                default:
                    returnKeyword = "return ";
                    returnType = Method.ContainingInterface.TypeArguments.Last();
                    break;
            }
            return
 $@"file sealed class {ClassName} : {interfaceFullName}
    {{
        private readonly Handler handler;
        private readonly nint functionPtr;
        
        [UnmanagedFunctionPointer(CallingConvention.{callingConvention})]
        {returnMarshalAsAttribute}public delegate {returnType} Handler{invokeParameters};
        
        {constructor}
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint GetFunctionPointer() => functionPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        {returnMarshalAsAttribute}public {returnType} Invoke{invokeParameters}
        {{
            {returnKeyword}handler({Constants.Arguments[invokeParameterCount]});
        }}{Interceptor?.SourceText ?? string.Empty}
    }}
    ";
        }
    }
}
