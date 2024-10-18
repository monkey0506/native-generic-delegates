using System;

namespace Monkeymoto.NativeGenericDelegates
{
    internal sealed class InterceptorDescriptor : IEquatable<InterceptorDescriptor>
    {
        private readonly int hashCode;

        public string Constraints { get; }
        public string InterfaceFullName { get; }
        public string Parameters { get; }
        public string TypeParameters { get; }

        public static bool operator ==(InterceptorDescriptor? left, InterceptorDescriptor? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(InterceptorDescriptor? left, InterceptorDescriptor? right) => !(left == right);

        public InterceptorDescriptor(MethodDescriptor method, string parameters)
        {
            Parameters = parameters;
            int interfaceArity = method.ContainingInterface.BaseInterface.Arity;
            if (method.ContainingInterface.IsUnmanaged)
            {
                Constraints = Constants.InterceptorUnmanagedTypeConstraints[interfaceArity];
                TypeParameters = Constants.InterceptorUnmanagedTypeParameters[interfaceArity];
            }
            else
            {
                Constraints = Constants.InterceptorTypeConstraints[interfaceArity];
                TypeParameters = Constants.InterceptorTypeParameters[interfaceArity];
            }
            if (interfaceArity != 0)
            {
                InterfaceFullName = $"{method.ContainingInterface.Name}<{TypeParameters}>";
                TypeParameters = method.Arity != 0 ?
                    $"<{TypeParameters}, XMarshaller>" :
                    $"<{TypeParameters}>";
            }
            else
            {
                InterfaceFullName = method.ContainingInterface.Name;
                if (method.Arity != 0)
                {
                    TypeParameters = "<XMarshaller>";
                }
            }
            hashCode = Hash.Combine(Constraints, InterfaceFullName, Parameters, TypeParameters);
        }

        public override bool Equals(object? obj) => obj is InterceptorDescriptor other && Equals(other);
        public bool Equals(InterceptorDescriptor? other) => (other is not null) &&
            (Constraints == other.Constraints) && (InterfaceFullName == other.InterfaceFullName) &&
            (Parameters == other.Parameters) && (TypeParameters == other.TypeParameters);
        public override int GetHashCode() => hashCode;
    }
}
