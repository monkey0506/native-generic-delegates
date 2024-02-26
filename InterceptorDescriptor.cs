using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterceptorDescriptor
    {
        public readonly string ClosedAttributes;
        public readonly string FirstArgument;
        public readonly string InterceptsMethod;
        public readonly string MarshalParamsAsParam;
        public readonly string MarshalReturnAsParam;
        public readonly int MethodHash;
        public readonly IReadOnlyList<string> OpenReferenceAttributes;
        public readonly string Parameters;
        public readonly string SourceText;
        public readonly IReadOnlyList<string> TypeArguments;
        public readonly string TypeParameters;

        public InterceptorDescriptor(in ClassDescriptor.Builder builder, in InterfaceDescriptor interfaceDescriptor)
        {
            ClosedAttributes = string.Join
            (
$@"
        ",
                GetAttributes(builder.References)
            );
            FirstArgument = builder.FirstArgument;
            InterceptsMethod = builder.Method.Name;
            OpenReferenceAttributes = GetAttributes(builder.References, closedTypes: false);
            TypeArguments = GetTypeArguments(in builder);
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
            MethodHash = Hash.Combine(builder.InterfaceSymbol.Name, builder.InterfaceSymbol.Arity, builder.Method.Name, builder.ArgumentInfo);
            Parameters = GetParameters(builder.FirstParameter, MarshalReturnAsParam, MarshalParamsAsParam);
            SourceText = GetSourceText(in builder, in interfaceDescriptor, ClosedAttributes, TypeParameters, Parameters);
        }

        private static IReadOnlyList<string> GetAttributes
        (
            IReadOnlyList<MethodReference> references,
            bool closedTypes = true
        )
        {
            var list = new List<string>(references.Count);
            foreach (var reference in references)
            {
                if (!reference.IsSyntaxReferenceClosedTypeOrMethod)
                {
                    if (closedTypes)
                    {
                        continue;
                    }
                }
                else if (!closedTypes)
                {
                    continue;
                }
                list.Add($"[InterceptsLocation(@\"{reference.FilePath}\", {reference.Line}, {reference.Character})]");
            }
            return list.AsReadOnly();
        }

//        private static string GetAttributes
//        (
//            IReadOnlyList<MethodReference> references,
//            bool closedTypes = true
//        )
//        {
//            var sb = new StringBuilder();
//            foreach (var reference in references)
//            {
//                if (!reference.IsSyntaxReferenceClosedTypeOrMethod)
//                {
//                    if (closedTypes)
//                    {
//                        continue;
//                    }
//                }
//                else if (!closedTypes)
//                {
//                    //throw new System.NotImplementedException($"{reference.Line}: {reference.Method}");
//                    continue;
//                }
//                _ = sb.Append
//                (
//$@"[InterceptsLocation(@""{reference.FilePath}"", {reference.Line}, {reference.Character})]
//        "
//                );
//            }
//            return sb.ToString();
//        }

        private static string GetParameters(string param, string marshalReturnAsParam, string marshalParamsAsParam)
        {
            return
$@"{param},
            {marshalReturnAsParam}{marshalParamsAsParam}CallingConvention callingConvention";
        }

        private static IReadOnlyList<string> GetTypeArguments(in ClassDescriptor.Builder builder)
        {
            return builder.InterfaceSymbol.TypeArguments.Select(x => x.ToDisplayString()).ToImmutableList();
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
            if (!isClosedNode)
            {
                return "";
            }
            return
$@"
        {attributes}[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {interfaceDescriptor.FullName} {builder.Method.Name}{typeParameters}
        (
            {parameters}
        )
        {{
            return new {builder.ClassName}({builder.FirstArgument});
        }}
";
        }
    }
}
