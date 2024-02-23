using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterceptorDescriptor
    {
        public readonly string Attributes;
        public readonly string MarshalParamsAsParam;
        public readonly string MarshalReturnAsParam;
        public readonly string Parameters;
        public readonly string SourceText;
        public readonly string TypeParameters;

        public InterceptorDescriptor(in ClassDescriptor.Builder builder, in InterfaceDescriptor interfaceDescriptor)
        {
            Attributes = GetAttributes(builder.References);
            TypeParameters = GetTypeParameters(builder.InterfaceSymbol.Arity, builder.Method.Arity);
            MarshalParamsAsParam = builder.InvokeParameterCount == 0 ?
                "" :
$@"MarshalAsAttribute[] marshalParamsAs,
            ";
            MarshalReturnAsParam = builder.IsAction switch
            {
                true => "",
                _ =>
$@"MarshalAsAttribute marshalReturnAs,
            ",
            };
            Parameters = GetParameters(builder.FirstParameter, MarshalReturnAsParam, MarshalParamsAsParam);
            SourceText = GetSourceText(in builder, in interfaceDescriptor, Attributes, TypeParameters, Parameters);
        }

        private static string GetAttributes(IReadOnlyList<MethodReference> references)
        {
            var sb = new StringBuilder();
            foreach (var reference in references)
            {
                _ = sb.Append
                (
$@"[InterceptsLocation(@""{reference.FilePath}"", {reference.Line}, {reference.Character})]
        "
                );
            }
            return sb.ToString();
        }

        private static string GetParameters(string param, string marshalReturnAsParam, string marshalParamsAsParam)
        {
            return
$@"{param},
            {marshalReturnAsParam}{marshalParamsAsParam}CallingConvention callingConvention";
        }

        private static string GetTypeParameters(int interfaceArity, int methodArity)
        {
            if (interfaceArity == 0)
            {
                return "";
            }
            int arity = interfaceArity + methodArity;
            return arity == 1 ?
                "<X>" :
                $"<{string.Join(", ", Enumerable.Range(1, interfaceArity).Select(x => $"X{x}"))}>";
        }

        private static string GetSourceText
        (
            in ClassDescriptor.Builder builder,
            in InterfaceDescriptor interfaceDescriptor,
            string attributes,
            string typeParameters,
            string parameters
        )
        {
            bool isClosedNode = builder.References[0].IsSyntaxReferenceClosedTypeOrMethod;
            var returnType = isClosedNode ? interfaceDescriptor.FullName : interfaceDescriptor.OriginalName.Replace('T', 'X');
            var impl = isClosedNode ?
                $"return new {builder.ClassName}({builder.FirstArgument});" :
                "throw new NotImplementedException();";
            return
$@"
        {attributes}[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {returnType} {builder.Method.Name}{typeParameters}
        (
            {parameters}
        )
        {{
            {impl}
        }}
";
        }
    }
}
