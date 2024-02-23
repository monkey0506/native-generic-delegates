using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly partial struct ClassDescriptor
    {
        public readonly CallingConvention CallingConvention;
        public readonly string Constructor;
        public readonly InterceptorDescriptor Interceptor;
        public readonly InterfaceDescriptor Interface;
        public readonly int InvokeParameterCount;
        public readonly string InvokeParameters;
        public readonly string Name;
        public readonly string ReturnKeyword;
        public readonly string ReturnMarshalAsAttribute;
        public readonly string ReturnType;

        public ClassDescriptor(in Builder builder)
        {
            CallingConvention = builder.ArgumentInfo.CallingConvention;
            Constructor = builder.IsFromFunctionPointer ?
                GetFromFunctionPointerConstructor(builder.ClassName) :
                GetFromDelegateConstructor(in builder);
            Interface = new InterfaceDescriptor(in builder);
            Interceptor = new InterceptorDescriptor(in builder, in Interface);
            InvokeParameterCount = builder.InvokeParameterCount;
            InvokeParameters = GetInvokeParameters(in builder);
            Name = builder.ClassName;
            var marshalReturnAs = builder.ArgumentInfo.MarshalInfo.MarshalReturnAs;
            ReturnMarshalAsAttribute = marshalReturnAs is null ?
                "" :
$@"[return: MarshalAs({marshalReturnAs})]
        ";
            switch (builder.IsAction)
            {
                case true:
                    ReturnKeyword = "";
                    ReturnType = "void";
                    break;
                default:
                    ReturnKeyword = "return ";
                    ReturnType = builder.InterfaceSymbol.TypeArguments.Last().ToDisplayString();
                    break;
            }
        }

        private static string GetFromDelegateConstructor(in Builder builder)
        {
            var name = builder.ClassName;
            var param = builder.FirstParameter;
            var arg = builder.FirstArgument;
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

        private static string GetInvokeParameters(in Builder builder)
        {
            if (builder.InvokeParameterCount == 0)
            {
                return "()";
            }
            var invokeParameterCount = builder.InvokeParameterCount;
            var marshalParamsAs = builder.ArgumentInfo.MarshalInfo.MarshalParamsAs;
            var typeArguments = builder.InterfaceSymbol.TypeArguments;
            var sb = new StringBuilder
            (
$@"
        (
            "
            );
            for (int i = 0, j = 1; i < invokeParameterCount; ++i, ++j)
            {
                if (marshalParamsAs?[i] is not null)
                {
                    _ = sb.Append
                    (
$@"[MarshalAs({marshalParamsAs[i]})]
            "
                    );
                }
                _ = sb.Append($@"{typeArguments[i].ToDisplayString()} t{(invokeParameterCount == 1 ? "" : j.ToString())}");
                if (j < invokeParameterCount)
                {
                    _ = sb.Append
                    (
$@",
            "
                    );
                }
                else
                {
                    _ = sb.AppendLine();
                }
            }
            _ = sb.Append
            (
$@"        )"
            );
            return sb.ToString();
        }

        public string GetSourceText()
        {
            return
$@"    file sealed class {Name} : {Interface.FullName}
    {{
        private readonly Handler handler;
        private readonly nint functionPtr;

        [UnmanagedFunctionPointer(CallingConvention.{CallingConvention})]
        {ReturnMarshalAsAttribute}public delegate {ReturnType} Handler{InvokeParameters};

        {Constructor}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nint GetFunctionPointer()
        {{
            return (nint)functionPtr;
        }}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        {ReturnMarshalAsAttribute}public {ReturnType} Invoke{InvokeParameters}
        {{
            {ReturnKeyword}handler({Constants.Arguments[InvokeParameterCount]});
        }}
{Interceptor.SourceText}    }}
";
        }
    }
}
