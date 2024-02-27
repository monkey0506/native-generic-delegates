using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Monkeymoto.Generators.NativeGenericDelegates.Generator
{
    internal readonly struct InterceptorDescriptor
    {
        public readonly string ClassName;
        public readonly string FirstArgument;
        public readonly string InterceptsMethod;
        public readonly string InterfaceName;
        public readonly int MethodHash;
        public readonly IReadOnlyList<string> OpenReferenceAttributes;
        public readonly string Parameters;
        public readonly string SourceText;
        public readonly IReadOnlyList<string> TypeArguments;
        public readonly string TypeParameters;

        public InterceptorDescriptor(in ClassDescriptor.DescriptorArgs descriptorArgs)
        {
            string closedReferenceAttributes =
                string.Join($"{Constants.NewLine}        ", GetAttributes(descriptorArgs.References));
            string interfaceTypeParameters = GetTypeParameters(descriptorArgs.InterfaceSymbol.Arity, 0);
            string marshalParamsAsParam = descriptorArgs.InvokeParameterCount == 0 ?
                "" :
                $"MarshalAsAttribute[] marshalParamsAs,{Constants.NewLine}            ";
            string marshalReturnAsParam = descriptorArgs.IsAction switch
            {
                true => "",
                _ => $@"MarshalAsAttribute marshalReturnAs,{Constants.NewLine}            ",
            };
            ClassName = descriptorArgs.ClassName;
            FirstArgument = descriptorArgs.FirstArgument;
            InterfaceName = $"{descriptorArgs.InterfaceName}{interfaceTypeParameters}";
            InterceptsMethod = descriptorArgs.Method.Name;
            OpenReferenceAttributes = GetAttributes(descriptorArgs.References, closedTypes: false);
            TypeArguments = descriptorArgs.InterfaceSymbol.TypeArguments.Select(x => x.ToDisplayString()).ToImmutableList();
            TypeParameters = GetTypeParameters(descriptorArgs.InterfaceSymbol.Arity, descriptorArgs.Method.Arity);
            MethodHash = Hash.Combine
            (
                descriptorArgs.InterfaceSymbol.Name,
                descriptorArgs.InterfaceSymbol.Arity,
                descriptorArgs.Method.Name,
                descriptorArgs.ArgumentInfo
            );
            Parameters = GetParameters(descriptorArgs.FirstParameter, marshalReturnAsParam, marshalParamsAsParam);
            SourceText = GetSourceText(in descriptorArgs, closedReferenceAttributes, TypeParameters, Parameters);
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

        private static string GetParameters(string param, string marshalReturnAsParam, string marshalParamsAsParam)
        {
            return
                $"{param},{Constants.NewLine}            {marshalReturnAsParam}{marshalParamsAsParam}CallingConvention callingConvention";
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
                $"<{string.Join(", ", Enumerable.Range(1, arity).Select(x => $"X{x}"))}>";
        }

        private static string GetSourceText
        (
            in ClassDescriptor.DescriptorArgs descriptorArgs,
            string attributes,
            string typeParameters,
            string parameters
        )
        {
            bool isClosedNode = descriptorArgs.References[0].IsSyntaxReferenceClosedTypeOrMethod;
            if (!isClosedNode)
            {
                return "";
            }
            attributes = $"        {attributes}";
            return
     $@"{attributes}[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static {descriptorArgs.InterfaceFullName} {descriptorArgs.Method.Name}{typeParameters}
        (
            {parameters}
        )
        {{
            return new {descriptorArgs.ClassName}({descriptorArgs.FirstArgument});
        }}";
        }
    }
}
