using System.Runtime.InteropServices;

namespace NativeGenericDelegates
{
    /// <summary>
    /// Represents a native generic delegate signature and marshaling behaviors.
    /// </summary>
    /// <param name="ReturnParameter">The method return parameter type and marshaling behavior.</param>
    /// <param name="CallingConvention">The unmanaged function calling convention.</param>
    /// <param name="Parameters">The method parameter types and marshaling behaviors.</param>
    internal sealed record NativeGenericDelegateInfo
        (
            ParameterWithMarshalAs? ReturnParameter,
            CallingConvention CallingConvention,
            ParameterWithMarshalAsCollection Parameters
        )
    {
        /// <summary>
        /// Gets the return parameter type.
        /// </summary>
        /// <remarks>
        /// If <see cref="ReturnParameter">ReturnParameter</see> is <see langword="null"/>, this returns <see
        /// langword="typeof"/>(<see langword="void"/>).
        /// </remarks>
        public Type ReturnType => ReturnParameter?.ParameterType ?? typeof(void);
    }
}
