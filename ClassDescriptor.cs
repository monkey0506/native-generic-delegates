using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct ClassDescriptor
    {
        public readonly InterceptorDescriptor Interceptor;
        public readonly string SourceText;

        public ClassDescriptor
        (
            InterfaceDescriptor interfaceDescriptor,
            MethodDescriptor methodDescriptor,
            ArgumentInfo argumentInfo,
            IReadOnlyList<MethodReference> references
        )
        {
            var descriptorArgs = new DescriptorArgs
            (
                in interfaceDescriptor,
                in methodDescriptor,
                in argumentInfo,
                references
            );
            Interceptor = new InterceptorDescriptor(in descriptorArgs);
            SourceText = GetSourceText(in descriptorArgs);
        }

        private static string GetFromDelegateConstructor(in DescriptorArgs descriptorArgs)
        {
            var name = descriptorArgs.ClassName;
            var param = descriptorArgs.FirstParameter;
            var arg = descriptorArgs.FirstArgument;
            return
     $@"internal {name}({param})
        {{
            ArgumentNullException.ThrowIfNull({arg});
            handler = (Handler)Delegate.CreateDelegate(typeof(Handler), {arg}.Target, {arg}.Method);
            functionPtr = Marshal.GetFunctionPointerForDelegate(handler);
        }}";
        }

        private static string GetFromFunctionPointerConstructor(string name)
        {
            return
     $@"internal {name}(nint functionPtr)
        {{
            if (functionPtr == nint.Zero)
            {{
                throw new ArgumentNullException(nameof(functionPtr));
            }}
            handler = Marshal.GetDelegateForFunctionPointer<Handler>(functionPtr);
            this.functionPtr = functionPtr;
        }}";
        }

        private static string GetInvokeParameters(in DescriptorArgs descriptorArgs)
        {
            var invokeParameterCount = descriptorArgs.Interface.InvokeParameterCount;
            if (invokeParameterCount == 0)
            {
                return "()";
            }
            var marshalParamsAs = descriptorArgs.ArgumentInfo.MarshalInfo.MarshalParamsAs;
            var typeArguments = descriptorArgs.Interface.TypeArguments;
            var sb = new StringBuilder($"{Constants.NewLine}        ({Constants.NewLine}            ");
            for (int i = 0, j = 1; i < invokeParameterCount; ++i, ++j)
            {
                if (marshalParamsAs?[i] is not null)
                {
                    _ = sb.Append($"[MarshalAs({marshalParamsAs[i]})]{Constants.NewLine}            ");
                }
                _ = sb.Append($@"{typeArguments[i]} t{(invokeParameterCount == 1 ? "" : j.ToString())}");
                if (j < invokeParameterCount)
                {
                    _ = sb.Append($",{Constants.NewLine}            ");
                }
                else
                {
                    _ = sb.AppendLine();
                }
            }
            _ = sb.Append("        )");
            return sb.ToString();
        }

        private string GetSourceText(in DescriptorArgs descriptorArgs)
        {
            CallingConvention callingConvention = descriptorArgs.ArgumentInfo.CallingConvention;
            string constructor = descriptorArgs.Method.IsFromFunctionPointer ?
                GetFromFunctionPointerConstructor(descriptorArgs.ClassName) :
                GetFromDelegateConstructor(in descriptorArgs);
            string interfaceFullName = descriptorArgs.Interface.FullName;
            int invokeParameterCount = descriptorArgs.Interface.InvokeParameterCount;
            string invokeParameters = GetInvokeParameters(in descriptorArgs);
            string className = descriptorArgs.ClassName;
            string returnMarshalAsAttribute = descriptorArgs.ArgumentInfo.MarshalInfo.MarshalReturnAs is null ?
                "" :
                $"[return: MarshalAs({descriptorArgs.ArgumentInfo.MarshalInfo.MarshalReturnAs})]{Constants.NewLine}        ";
            string? returnKeyword;
            string? returnType;
            switch (descriptorArgs.Interface.IsAction)
            {
                case true:
                    returnKeyword = "";
                    returnType = "void";
                    break;
                default:
                    returnKeyword = "return ";
                    returnType = descriptorArgs.Interface.TypeArguments.Last();
                    break;
            }
            return
 $@"file sealed class {className} : {interfaceFullName}
    {{
        private readonly Handler handler;
        private readonly nint functionPtr;

        [UnmanagedFunctionPointer(CallingConvention.{callingConvention})]
        {returnMarshalAsAttribute}public delegate {returnType} Handler{invokeParameters};

        {constructor}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint GetFunctionPointer()
        {{
            return (nint)functionPtr;
        }}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        {returnMarshalAsAttribute}public {returnType} Invoke{invokeParameters}
        {{
            {returnKeyword}handler({Constants.Arguments[invokeParameterCount]});
        }}{Interceptor.SourceText}
    }}
";
        }
    }
}
