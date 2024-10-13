using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed partial class MarshalInfo : IEquatable<MarshalInfo>
    {
        private readonly int hashCode;

        public IReadOnlyList<string?>? MarshalParamsAs { get; }
        public string? MarshalReturnAs { get; }
        public CallingConvention? StaticCallingConvention { get; }

        public static bool operator ==(MarshalInfo? left, MarshalInfo? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(MarshalInfo? left, MarshalInfo? right) => !(left == right);

        private static IFieldReferenceOperation? GetCallingConventionOperation
        (
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            IPropertySymbol? property = null,
            Compilation? compilation = null
        )
        {
            var callingConventionArg = invocationExpression.ArgumentList.Arguments
                .Select(x => semanticModel.GetOperation(x, cancellationToken) as IArgumentOperation)
                .Where(static x => (x is not null) && (x.Parameter!.Name == "callingConvention"))
                .FirstOrDefault();
            return callingConventionArg is not null ?
                callingConventionArg.Value as IFieldReferenceOperation :
                GetFieldReferenceOperation(property, compilation, cancellationToken);
        }

        private static IFieldReferenceOperation? GetFieldReferenceOperation
        (
            IPropertySymbol? property,
            Compilation? compilation,
            CancellationToken cancellationToken
        ) => GetOperation<IFieldReferenceOperation>(property, compilation, cancellationToken);

        public static MarshalInfo GetMarshalInfo
        (
            INamedTypeSymbol? marshaller,
            InterfaceDescriptor interfaceDescriptor,
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken
        )
        {
            if (marshaller is null)
            {
                return new(invocationExpression, semanticModel, cancellationToken);
            }
            var compilation = semanticModel.Compilation;
            var marshallerInterface = compilation.GetTypeByMetadataName(Constants.IMarshallerMetadataName)!;
            var properties = marshaller.GetMembers()
                .OfType<IPropertySymbol>()
                .Where
                (
                    x => x.ExplicitInterfaceImplementations.FirstOrDefault() is IPropertySymbol prop &&
                        SymbolEqualityComparer.Default.Equals
                        (
                            prop.ContainingType.OriginalDefinition,
                            marshallerInterface
                        )
                );
            IPropertySymbol? callingConventionProperty = null;
            IPropertySymbol? marshalMapProperty = null;
            IPropertySymbol? marshalParamsAsProperty = null;
            IPropertySymbol? marshalReturnAsProperty = null;
            foreach (var property in properties)
            {
                var name = property.Name.Substring(property.Name.LastIndexOf('.') + 1);
                switch (name)
                {
                    case "CallingConvention":
                        callingConventionProperty = property;
                        break;
                    case "MarshalMap":
                        marshalMapProperty = property;
                        break;
                    case "MarshalParamsAs":
                        marshalParamsAsProperty = property;
                        break;
                    case "MarshalReturnAs":
                        marshalReturnAsProperty = property;
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            var callingConventionOp = GetCallingConventionOperation
            (
                invocationExpression,
                semanticModel,
                cancellationToken,
                callingConventionProperty,
                compilation
            );
            var marshalMapOp = GetObjectCreationOperation(marshalMapProperty, compilation, cancellationToken);
            var marshalParamsAsOp =
                GetObjectCreationOperation(marshalParamsAsProperty, compilation, cancellationToken);
            var marshalReturnAsOp =
                GetObjectCreationOperation(marshalReturnAsProperty, compilation, cancellationToken);
            return new
            (
                interfaceDescriptor,
                callingConventionOp,
                marshalMapOp,
                marshalParamsAsOp,
                marshalReturnAsOp
            );
        }

        private static IObjectCreationOperation? GetObjectCreationOperation
        (
            IPropertySymbol? property,
            Compilation compilation,
            CancellationToken cancellationToken
        ) => GetOperation<IObjectCreationOperation>(property, compilation, cancellationToken);

        private static T? GetOperation<T>
        (
            IPropertySymbol? property,
            Compilation? compilation,
            CancellationToken cancellationToken
        )
            where T : class, IOperation
        {
            if ((property is null) || (compilation is null))
            {
                return null;
            }
            var node = property.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken);
            var semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
            return node.DescendantNodesAndSelf()
                .Select(x => semanticModel.GetOperation(x, cancellationToken))
                .Where(static x => x is not null)
                .Select(static x => x.DescendantsAndSelf().OfType<T>().FirstOrDefault())
                .FirstOrDefault();
        }

        private static CallingConvention? GetStaticCallingConvention(IFieldReferenceOperation? callingConventionOp)
        {
            var field = callingConventionOp?.Field;
            if (field is not null && SymbolEqualityComparer.Default.Equals(field.ContainingType, field.Type) &&
                Enum.TryParse(field?.Name, false, out CallingConvention callingConvention))
            {
                return callingConvention;
            }
            return null;
        }

        private MarshalInfo
        (
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken
        )
        {
            var operation =
                GetCallingConventionOperation(invocationExpression, semanticModel, cancellationToken);
            StaticCallingConvention = GetStaticCallingConvention(operation);
            hashCode = Hash.Combine
            (
                MarshalParamsAs,
                MarshalReturnAs,
                StaticCallingConvention
            );
        }

        private MarshalInfo
        (
            InterfaceDescriptor interfaceDescriptor,
            IFieldReferenceOperation? callingConventionOp,
            IObjectCreationOperation? marshalMapCreation,
            IObjectCreationOperation? marshalParamsAsCreation,
            IObjectCreationOperation? marshalReturnAsCreation
        )
        {
            StaticCallingConvention = GetStaticCallingConvention(callingConventionOp);
            MarshalParamsAs =
                Parser.GetMarshalParamsAs(marshalParamsAsCreation, interfaceDescriptor.InvokeParameterCount);
            MarshalReturnAs = Parser.GetMarshalReturnAs(marshalReturnAsCreation);
            var marshalMap = MarshalMap.Parse(marshalMapCreation);
            if (marshalMap is not null)
            {
                var marshalParamsList = new List<string?>(interfaceDescriptor.InvokeParameterCount);
                if (MarshalParamsAs is not null)
                {
                    marshalParamsList.AddRange(MarshalParamsAs);
                }
                while (marshalParamsList.Count < interfaceDescriptor.InvokeParameterCount)
                {
                    marshalParamsList.Add(null);
                }
                bool dirty = false;
                for (int i = 0; i < interfaceDescriptor.InvokeParameterCount; ++i)
                {
                    if ((marshalParamsList[i] is null) &&
                        marshalMap.TryGetValue(interfaceDescriptor.TypeArguments[i], out var marshalParamAs))
                    {
                        marshalParamsList[i] = marshalParamAs;
                        dirty = true;
                    }
                }
                if (dirty)
                {
                    MarshalParamsAs = marshalParamsList.AsReadOnly();
                }
                if ((MarshalReturnAs is null) && !interfaceDescriptor.IsAction &&
                    marshalMap.TryGetValue(interfaceDescriptor.ReturnType, out var marshalReturnAs))
                {
                    MarshalReturnAs = marshalReturnAs;
                }
            }
            hashCode = Hash.Combine
            (
                MarshalParamsAs,
                MarshalReturnAs,
                StaticCallingConvention
            );
        }

        public override bool Equals(object? obj) => obj is MarshalInfo other && Equals(other);
        public bool Equals(MarshalInfo? other) =>
            (other is not null) &&
            (MarshalParamsAs?.SequenceEqual(other.MarshalParamsAs) ?? other.MarshalParamsAs is null) &&
            (MarshalReturnAs == other.MarshalReturnAs) && (StaticCallingConvention == other.StaticCallingConvention);
        public override int GetHashCode() => hashCode;
    }
}
