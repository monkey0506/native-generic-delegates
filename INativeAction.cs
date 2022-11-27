using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NativeGenericDelegates
{
    /// <summary>
    /// Represents a native generic delegate with no parameters and no return value.
    /// </summary>
    public interface INativeAction
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
        public static INativeAction FromDelegate(Delegate d, CallingConvention callingConvention)
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
        public static INativeAction FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <summary>
        /// Creates a native generic delegate with the same signature as this interface that will invoke an unmanaged function
        /// pointer.
        /// </summary>
        /// <param name="functionPtr">The unmanaged function pointer that will be invoked by this delegate.</param>
        /// <inheritdoc cref="FromDelegate" path="//param[@name='callingConvention']|//returns"/>
        /// <param name="marshalParamAs">
        /// An optional array of <see cref="MarshalAsAttribute">MarshalAsAttribute</see>s that will be applied to each parameter
        /// of the managed delegate. If supplied, the array length and order must match the signature of this interface.
        /// </param>
        public static INativeAction FromFunctionPointer(nint functionPtr, CallingConvention callingConvention)
        {
            NativeGenericDelegateInfo info = new(null, callingConvention, ParameterWithMarshalAsCollection.Empty);
            return (INativeAction)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction FromFunctionPointer(delegate* unmanaged[Cdecl]<void> functionPtr)
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction FromFunctionPointer(delegate* unmanaged[Stdcall]<void> functionPtr)
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction FromFunctionPointer(delegate* unmanaged[Thiscall]<void> functionPtr)
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall);
        }

        /// <inheritdoc cref="FromFunctionPointer(nint, CallingConvention)"
        /// path="//summary|//param[@name='functionPtr']|//param[@name='marshalParamAs']|//returns"/>
        /// <remarks>
        /// <para>
        /// The parameter types of the function pointer do not need to match the signature of this interface, but if they do not
        /// match then you must supply <paramref name="marshalParamAs"/> with correct marshaling behavior for those
        /// parameters.
        /// </para><para>
        /// For example, the native method may accept a <c>char*</c> for a string argument in UTF-8 encoding. The managed
        /// delegate can use a <see cref="string">string</see> parameter with a <see
        /// cref="MarshalAsAttribute">MarshalAsAttribute</see> constructed using <see
        /// cref="UnmanagedType.LPUTF8Str">UnmanagedType.LPUTF8Str</see>.
        /// </para>
        /// </remarks>
        private static INativeAction FromUnsafeFunctionPointer(nint functionPtr, CallingConvention callingConvention)
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention);
        }

        /// <inheritdoc cref="Marshal.GetFunctionPointerForDelegate(Delegate)"/>
        /// <remarks>
        /// You must keep a managed reference to this delegate for the lifetime of the unmanaged function pointer.
        /// </remarks>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke();

        /// <summary>
        /// Creates an <see cref="Action"/> with the same <see cref="Target">Target</see> and <see cref="Method">Method</see> as
        /// this delegate.
        /// </summary>
        /// <returns>The new delegate.</returns>
        public Action ToAction()
        {
            return (Action)Delegate.CreateDelegate(typeof(Action), Target, Method);
        }
    }

    /// <summary>
    /// Represents a native generic delegate with the given parameter types and no return value.
    /// </summary>
    public interface INativeAction<T>
    {
        /// <inheritdoc cref="INativeAction.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction.FromDelegate"/>
        public static INativeAction<T> FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction.FromMethod"/>
        public static INativeAction<T> FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction.FromFunctionPointer(nint, CallingConvention)"/>
        /// <param name="marshalParamAs">
        /// An optional array of <see cref="MarshalAsAttribute">MarshalAsAttribute</see>s that will be applied to each parameter
        /// of the managed delegate. If supplied, the array length and order must match the signature of this interface.
        /// </param>
        public static INativeAction<T> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T>(marshalParamAs)
                );
            return (INativeAction<T>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T> FromFunctionPointer<U>
            (
                delegate* unmanaged[Cdecl]<U, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T> FromFunctionPointer<U>
            (
                delegate* unmanaged[Stdcall]<U, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T> FromFunctionPointer<U>
            (
                delegate* unmanaged[Thiscall]<U, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction.FromUnsafeFunctionPointer"/>
        /// <inheritdoc cref="FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"
        /// path="//param[@name='marshalParamAs']"/>
        private static INativeAction<T> FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T t);

        /// <inheritdoc cref="INativeAction.ToAction"/>
        public Action<T> ToAction()
        {
            return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2>(marshalParamAs)
                );
            return (INativeAction<T1, T2>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2>
            FromFunctionPointer<U1, U2>
            (
                delegate* unmanaged[Cdecl]<U1, U2, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2>
            FromFunctionPointer<U1, U2>
            (
                delegate* unmanaged[Stdcall]<U1, U2, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2>
            FromFunctionPointer<U1, U2>
            (
                delegate* unmanaged[Thiscall]<U1, U2, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2> ToAction()
        {
            return (Action<T1, T2>)Delegate.CreateDelegate(typeof(Action<T1, T2>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3>
            FromFunctionPointer<U1, U2, U3>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3>
            FromFunctionPointer<U1, U2, U3>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3>
            FromFunctionPointer<U1, U2, U3>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3> ToAction()
        {
            return (Action<T1, T2, T3>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4>
            FromFunctionPointer<U1, U2, U3, U4>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4>
            FromFunctionPointer<U1, U2, U3, U4>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4>
            FromFunctionPointer<U1, U2, U3, U4>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4> ToAction()
        {
            return (Action<T1, T2, T3, T4>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5>
            FromFunctionPointer<U1, U2, U3, U4, U5>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5>
            FromFunctionPointer<U1, U2, U3, U4, U5>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5>
            FromFunctionPointer<U1, U2, U3, U4, U5>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5>)Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6>)
                Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6, T7>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7>)
                Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6, T7, T8>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8>)
                Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9>(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)
                Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>), Target, Method);
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13);

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke
            (
                T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14
            );

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void> functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke
            (
                T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14,
                T15 t15
            );

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>),
                    Target,
                    Method
                );
        }
    }

    /// <inheritdoc cref="INativeAction{T}"/>
    public interface INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        /// <inheritdoc cref="INativeAction{T}.Method"/>
        public MethodInfo Method => ((Delegate)this).Method;
        /// <inheritdoc cref="INativeAction{T}.Target"/>
        public object? Target => ((Delegate)this).Target;

        /// <inheritdoc cref="INativeAction{T}.FromDelegate"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromDelegate(Delegate d, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(d);
            return FromMethod(d.Target, d.Method, callingConvention);
        }

        /// <inheritdoc cref="INativeAction{T}.FromMethod"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromMethod(object? target, MethodInfo method, CallingConvention callingConvention)
        {
            ArgumentNullException.ThrowIfNull(method);
            NativeGenericDelegateInfo info = new(null, callingConvention, new(method));
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)
                DelegateFactory.GetDelegate(info, target, method);
        }

        /// <inheritdoc cref="INativeAction{T}.FromFunctionPointer(nint, CallingConvention, MarshalAsAttribute?[]?)"/>
        public static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> FromFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            NativeGenericDelegateInfo info = new
                (
                    null,
                    callingConvention,
                    ParameterWithMarshalAsCollection.FromTypes
                    <
                        T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16
                    >(marshalParamAs)
                );
            return (INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)
                DelegateFactory.GetDelegate(info, functionPtr);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromFunctionPointer
            (
                delegate* unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>
            functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromFunctionPointer
            (
                delegate* unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromFunctionPointer
            (
                delegate* unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16>
            (
                delegate* unmanaged[Cdecl]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromUnsafeFunctionPointer((nint)functionPtr, CallingConvention.Cdecl, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16>
            (
                delegate* unmanaged[Stdcall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.StdCall, marshalParamAs);
        }

        /// <inheritdoc cref="FromUnsafeFunctionPointer"/>
        public static unsafe INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromFunctionPointer<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16>
            (
                delegate* unmanaged[Thiscall]<U1, U2, U3, U4, U5, U6, U7, U8, U9, U10, U11, U12, U13, U14, U15, U16, void>
                    functionPtr,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            return FromFunctionPointer((nint)functionPtr, CallingConvention.ThisCall, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.FromUnsafeFunctionPointer"/>
        private static INativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
            FromUnsafeFunctionPointer
            (
                nint functionPtr,
                CallingConvention callingConvention,
                MarshalAsAttribute?[]? marshalParamAs = null
            )
        {
            // this overload is for <inheritdoc/>, which doesn't seem to play nicely with function pointer types
            return FromFunctionPointer(functionPtr, callingConvention, marshalParamAs);
        }

        /// <inheritdoc cref="INativeAction{T}.GetFunctionPointer"/>
        public nint GetFunctionPointer()
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)this);
        }

        public void Invoke
            (
                T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14,
                T15 t15, T16 t16
            );

        /// <inheritdoc cref="INativeAction{T}.ToAction"/>
        public Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ToAction()
        {
            return (Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)
                Delegate.CreateDelegate
                (
                    typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>),
                    Target,
                    Method
                );
        }
    }
}
