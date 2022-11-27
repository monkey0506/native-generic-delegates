using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NativeGenericDelegates
{
    /// <summary>
    /// Represents a native generic delegate with no parameters and the given return value.
    /// </summary>
    public interface INativeFunc<TResult>
    {
        /// <inheritdoc cref="Delegate.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="Delegate.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <summary>
        /// Creates a native generic delegate with the same signature as this interface that will invoke the same method (and
        /// target, if any) as the given delegate.
        /// </summary>
        /// <param name="d">The delegate to copy the invocation method and target from.</param>
        /// <param name="callingConvention">
        /// The calling convention of the unmanaged function pointer (see <see
        /// cref="GetFunctionPointer">GetFunctionPointer</see>).
        /// </param>
        /// <returns>The new delegate instance.</returns>
        public static INativeFunc<TResult> FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <summary>
        /// Creates a native generic delegate with the same signature as this interface that will invoke the given method with
        /// the given target.
        /// </summary>
        /// <param name="target">
        /// The class instance on which the <paramref name="method"/> will be invoked. Should be <see langword="null"/> if
        /// <paramref name="method"/> is a <see langword="static"/> method.
        /// </param>
        /// <param name="method">The method that will be invoked by this delegate.</param>
        /// <inheritdoc cref="FromDelegate" path="//param[@name='callingConvention']|//returns"/>
        public static INativeFunc<TResult> FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <summary>
        /// Creates a native generic delegate with the same signature as this interface that will invoke an unmanaged function
        /// pointer.
        /// </summary>
        /// <param name="functionPtr">The unmanaged function pointer that will be invoked by this delegate.</param>
        /// <inheritdoc cref="FromDelegate" path="//param[@name='callingConvention']|//returns"/>
        /// <param name="marshalReturnAs"></param>
        public static INativeFunc<TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            NativeGenericDelegateInfo info =
                new(new(typeof(TResult), marshalReturnAs), callingConvention, ParameterWithMarshalAsCollection.Empty);
            return (INativeFunc<TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<TResult> FromFunctionPointer<UResult>
            (
                delegate* unmanaged[Cdecl]<UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<TResult> FromFunctionPointer<UResult>
            (
                delegate* unmanaged[Stdcall]<UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<TResult> FromFunctionPointer<UResult>
            (
                delegate* unmanaged[Thiscall]<UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs);
        }

        /// <inheritdoc cref="FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?)"
        /// path="//summary|//param[@name='functionPtr']|//returns"/>
        private static INativeFunc<TResult> FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs);
        }

        /// <inheritdoc cref="Marshal.GetFunctionPointerForDelegate(Delegate)"/>
        /// <remarks>
        /// You must keep a managed reference to this delegate for the lifetime of the unmanaged function pointer.
        /// </remarks>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke();

        /// <summary>
        /// Creates a <see cref="Func{TResult}"/> with the same <see cref="Target">Target</see> and <see
        /// cref="Method">Method</see> as this delegate.
        /// </summary>
        /// <returns>The new delegate.</returns>
        public Func<TResult> ToFunc()
        {
            return (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), Target, Method);
        }
    }

    /// <summary>
    /// Represents a native generic delegate with the given parameter types and no return value.
    /// </summary>
    public interface INativeFunc<T, TResult>
    {
        /// <inheritdoc cref="INativeFunc{TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{TResult}.FromDelegate"/>
        public static INativeFunc<T, TResult> FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{TResult}.FromMethod"/>
        public static INativeFunc<T, TResult> FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{TResult}.FromFunctionPointer(nint, CallingConvention)"/>
        /// <param name="marshalParamAs">
        /// An optional array of <see cref="MarshalAsAttribute">MarshalAsAttribute</see>s that will be applied to each parameter
        /// of the managed delegate. If supplied, the array length and order must match the signature of this interface.
        /// </param>
        public static INativeFunc<T, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T>(marshalParamAs)
                );
            return (INativeFunc<T, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T, TResult> FromFunctionPointer<U, UResult>
            (
                delegate* unmanaged[Cdecl]<U, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T, TResult> FromFunctionPointer<U, UResult>
            (
                delegate* unmanaged[Stdcall]<U, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T, TResult> FromFunctionPointer<U, UResult>
            (
                delegate* unmanaged[Thiscall]<U, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{TResult}.FromUnsafeFunctionPointer"/>
        /// <inheritdoc cref="FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"
        /// path="//param[@name='marshalParamAs']"/>
        private static INativeFunc<T, TResult> FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T t);

        /// <inheritdoc cref="INativeFunc{TResult}.ToFunc"/>
        public Func<T, TResult> ToFunc()
        {
            return (Func<T, TResult>)Delegate.CreateDelegate(typeof(Func<T, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, TResult>
            FromFunctionPointer<U1, U2, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, TResult>
            FromFunctionPointer<U1, U2, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, TResult>
            FromFunctionPointer<U1, U2, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, TResult> ToFunc()
        {
            return (Func<T1, T2, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, TResult>
            FromFunctionPointer<U1, U2, U3, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, TResult>
            FromFunctionPointer<U1, U2, U3, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, TResult>
            FromFunctionPointer<U1, U2, U3, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, TResult>
            FromFunctionPointer<U1, U2, U3, U4, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, TResult>
            FromFunctionPointer<U1, U2, U3, U4, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, TResult>
            FromFunctionPointer<U1, U2, U3, U4, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, TResult>)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, TResult>)
                Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc
        /// cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, TResult>)
                Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc
        /// cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6, T7>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, TResult>)
                Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc
        /// cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6, T7, T8>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)
                Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc
        /// cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9>(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)
                Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc
        /// cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc
        /// cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, UResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13);

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke
            (
                T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14
            );

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke
            (
                T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14,
                T15 t15
            );

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeFunc{T, TResult}"/>
    public interface INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
    {
        /// <inheritdoc cref="INativeFunc{T, TResult}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeFunc{T, TResult}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromDelegate"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromMethod"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(new(method.ReturnParameter), callingConvention, new(method));
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    new(typeof(TResult), marshalReturnAs),
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16
                    >(marshalParamAs)
                );
            return (INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, UResult>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, UResult>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, UResult>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, UResult>
                    functionPtr,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.FromUnsafeFunctionPointer"/>
        private static INativeFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute? marshalReturnAs = null,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalReturnAs, marshalParamAs);
        }

        /// <inheritdoc cref="INativeFunc{T, TResult}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public TResult Invoke
            (
                T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14,
                T15 t15, T16 t16
            );

        /// <inheritdoc cref="INativeFunc{T, TResult}.ToFunc"/>
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> ToFunc()
        {
            return (Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)
                Delegate.CreateDelegate
                (
                    typeof(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>),
                    Target,
                    Method
                );
        }
    }
}
