using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeGenericDelegates
{
    /// <summary>
    /// Represents a collection of native generic delegate method parameters with optional marshaling behavior.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This collection does not include the return parameter (if any). See <see
    /// cref="NativeGenericDelegateInfo.ReturnParameter">NativeGenericDelegateInfo.ReturnParameter</see> to access the return
    /// parameter.
    /// </para>
    /// </remarks>
    internal sealed class ParameterWithMarshalAsCollection :
        IEquatable<ParameterWithMarshalAsCollection>,
        IEqualityOperators<ParameterWithMarshalAsCollection, ParameterWithMarshalAsCollection, bool>,
        IReadOnlyList<ParameterWithMarshalAs>
    {
        public static readonly ParameterWithMarshalAsCollection Empty = new();

        private readonly ParameterWithMarshalAs[] parameters;

        public static bool operator ==(ParameterWithMarshalAsCollection? left, ParameterWithMarshalAsCollection? right) =>
            left?.Equals(right) ?? right is null;
        public static bool operator !=(ParameterWithMarshalAsCollection? left, ParameterWithMarshalAsCollection? right) =>
            !(left == right);

        #region FromTypes generic overloads
        /// <summary>
        /// Creates a collection from the given generic type parameters with optional marshaling behavior.
        /// </summary>
        /// <param name="marshalParamAs">
        /// Optionally defines the marshaling behavior for the parameters. The array length and order must match the generic type
        /// parameters.
        /// </param>
        /// <returns>The new parameter collection.</returns>
        public static ParameterWithMarshalAsCollection FromTypes<T>(MarshalAsAttribute?[]? marshalParamAs = null) =>
            FromTypes(marshalParamAs, typeof(T));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2>(MarshalAsAttribute?[]? marshalParamAs = null) =>
            FromTypes(marshalParamAs, typeof(T1), typeof(T2));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3>(MarshalAsAttribute?[]? marshalParamAs = null) =>
            FromTypes(marshalParamAs, typeof(T1), typeof(T2), typeof(T3));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4>
            (
                MarshalAsAttribute?[]? marshalParamAs = null
            ) => FromTypes(marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5>
            (
                MarshalAsAttribute?[]? marshalParamAs = null
            ) => FromTypes(marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes(marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes(marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection
            FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15)
            );
        /// <inheritdoc cref="FromTypes{T}(MarshalAsAttribute?[]?)"/>
        public static ParameterWithMarshalAsCollection
            FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            (
                MarshalAsAttribute?[]? marshalParamAs
            ) => FromTypes
            (
                marshalParamAs, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8),
                typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16)
            );
        #endregion FromTypes generic overloads

        /// <summary>
        /// Creates a collection from the given parameter types with optional marshaling behavior.
        /// </summary>
        /// <param name="marshalParamAs">
        /// Optionally defines the marshaling behavior for the parameters. The array length and order must match the <paramref
        /// name="parameterTypes"/> array.
        /// </param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>The new parameter collection.</returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="parameterTypes"/> and <paramref name="marshalParamAs"/> arrays are of different lengths.
        /// </exception>
        private static ParameterWithMarshalAsCollection FromTypes
            (
                MarshalAsAttribute?[]? marshalParamAs,
                params Type[] parameterTypes
            )
        {
            if (marshalParamAs is null)
            {
                marshalParamAs = new MarshalAsAttribute[parameterTypes.Length];
            }
            else if (marshalParamAs.Length != parameterTypes.Length)
            {
                throw new ArgumentException
                    (
                        $"{nameof(MarshalAsAttribute)} array must be of same length as parameter type array.",
                        nameof(marshalParamAs)
                    );
            }
            var parameters = new ParameterWithMarshalAs[parameterTypes.Length];
            for (int i = 0; i < parameterTypes.Length; ++i)
            {
                parameters[i] = new(parameterTypes[i], marshalParamAs[i]);
            }
            return new(parameters);
        }

        /// <summary>
        /// Creates a collection from the given parameters.
        /// </summary>
        /// <param name="parameters">The native generic delegate parameters with optional marshaling behavior.</param>
        public ParameterWithMarshalAsCollection(params ParameterWithMarshalAs[] parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);
            for (int i = 0; i < parameters.Length; ++i)
            {
                ArgumentNullException.ThrowIfNull(parameters[i]);
            }
            this.parameters = parameters;
        }

        /// <summary>
        /// Creates a collection from the given method.
        /// </summary>
        /// <param name="method">The method to copy the signature and marshaling behavior from.</param>
        public ParameterWithMarshalAsCollection(MethodInfo method, MarshalAsAttribute?[]? marshalParamAs = null)
        {
            ArgumentNullException.ThrowIfNull(method);
            var parameterInfos = method.GetParameters();
            parameters = parameterInfos.Length != 0 ?
                new ParameterWithMarshalAs[parameterInfos.Length] :
                Array.Empty<ParameterWithMarshalAs>();
            bool overrideMarshaling = marshalParamAs is not null;
            if (overrideMarshaling && (marshalParamAs!.Length != parameters.Length))
            {
                throw new ArgumentException
                    (
                        $"{nameof(MarshalAsAttribute)} array must be of same length as method parameter list.",
                        nameof(marshalParamAs)
                    );
            }
            for (int i = 0; i < parameterInfos.Length; ++i)
            {
                parameters[i] = new
                    (
                        parameterInfos[i].ParameterType,
                        overrideMarshaling ?
                            marshalParamAs![i] :
                            (MarshalAsAttribute?)parameterInfos[i].GetCustomAttribute(typeof(MarshalAsAttribute))
                    );
            }
        }

        /// <summary>
        /// Creates a collection from the given delegate's method.
        /// </summary>
        /// <param name="d">The delegate to copy the method signature and marshaling behavior from.</param>
        public ParameterWithMarshalAsCollection(Delegate d, MarshalAsAttribute?[]? marshalParamAs = null) :
            this(d?.Method!, marshalParamAs)
        { }

        /// <summary>
        /// Gets the parameter at the given index.
        /// </summary>
        /// <param name="index">The index of the requested parameter.</param>
        /// <returns>The requested parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero or greater than or equal to <see cref="Count">Count</see>.
        /// </exception>
        public ParameterWithMarshalAs this[int index] => parameters[index];

        /// <summary>
        /// Gets the number of parameters in this collection.
        /// </summary>
        public int Count => parameters.Length;

        public override bool Equals(object? obj)
        {
            return obj is ParameterWithMarshalAsCollection other && Equals(other);
        }

        public bool Equals(ParameterWithMarshalAsCollection? other)
        {
            return other is not null && parameters.SequenceEqual(other.parameters);
        }

        public IEnumerator<ParameterWithMarshalAs> GetEnumerator()
        {
            return ((IEnumerable<ParameterWithMarshalAs>)parameters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            int hash = parameters.Length;
            foreach (var parameter in parameters)
            {
                hash = HashCode.Combine(hash, parameter);
            }
            return hash;
        }

        public override string ToString()
        {
            if (parameters.Length == 0)
            {
                return $"{nameof(ParameterWithMarshalAsCollection)} {{ }}";
            }
            StringBuilder sb = new(nameof(ParameterWithMarshalAsCollection));
            sb.Append($"{Environment.NewLine}{{{Environment.NewLine}");
            for (int i = 0; i < parameters.Length; ++i)
            {
                sb.Append($"    {parameters[i]}");
                if ((i + 1) != parameters.Length)
                {
                    sb.Append($",{Environment.NewLine}");
                }
                else
                {
                    sb.Append(Environment.NewLine);
                }
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}
