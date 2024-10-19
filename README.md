# Monkeymoto.NativeGenericDelegates

__NativeGenericDelegates__ is a C# incremental source generator designed to
provide
[delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate)-_like_
types that are
_[generic](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics)_
and can be used from _native_ code with
_[platform invoke](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)_
(P/Invoke). There is a historical caveat to .NET generics in that they cannot
be used with P/Invoke.
[Marshal.GetFunctionPointerForDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getfunctionpointerfordelegate#system-runtime-interopservices-marshal-getfunctionpointerfordelegate(system-delegate))
in particular will throw an `ArgumentException` if passed a generic delegate
type. This means that you are often left with no option but to create your own
delegate types as-needed, and there is little to no room for code reusability.

This project was inspired by a
[StackOverflow question](https://stackoverflow.com/questions/26699394/c-sharp-getdelegateforfunctionpointer-with-generic-delegate)
for which the correct solution was, in fact, to use
[DllImportAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute).
However, there are still scenarios where it may be desirable to utilize generic
delegates with P/Invoke and this project aims to facilitate those use cases.

## What this project does

_Note: Previous versions of this project relied on dynamic code using types
from the
[System.Reflection.Emit](https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/emitting-dynamic-methods-and-assemblies)
namespace. The current version does __not__ use dynamic code, and __can__ be
used with
[Native AOT deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)._

This project provides a set of `interface`s that mirror the `System.Action` and
`System.Func` delegate types with the following features:

* Generic interface (e.g., `INativeAction<int>`, `INativeFunc<double, bool>`,
  etc.)
* Passing delegate to unmanaged code (via
  [`GetFunctionPointer`](#getfunctionpointer-method))
* Construct delegates from managed and unmanaged functions with common
  interface
* Non-dynamic invocation from managed code (via [`Invoke`](#invoke-method), no
  calls to `Delegate.DynamicInvoke`)
* Define unmanaged function pointer calling convention
* Optionally [define marshaling behavior](#imarshallertself-interface) for
  return value and parameters
* Convert to `System.Action` and `System.Func` delegate types (via [`ToAction`
  and `ToFunc`](#toaction-and-tofunc-methods))

## How to use this project

__You just DO WHAT THE FUCK YOU WANT TO.__

As per the [license terms](COPYING.md), you are free to use this
project however you see fit, but the following contains an overview of the
public API that the code exposes:

### `INativeAction`, `INativeFunc`, `IUnmanagedAction`, and `IUnmanagedFunc` interfaces

```C#
internal interface INativeAction
internal interface INativeAction<...>
internal interface INativeFunc<...>
internal unsafe interface IUnmanagedAction
internal unsafe interface IUnmanagedAction<...>
internal unsafe interface IUnmanagedFunc<...>
```

This project provides interfaces with each of these names with a range of type
arguments. These interfaces mirror `System.Action` and `System.Func` in their
type arguments. The `-Action` interfaces do not return a value, and the `-Func`
interfaces do return a value. When discussing these interfaces, "return type"
and "arguments" are in reference to the `Invoke` interface method.

#### `INativeAction` and `INativeFunc` interfaces

`INativeAction` and `INativeFunc` take the same type arguments as the
matching `System.Action` and `System.Func`, respectively.

That is, `INativeAction` has a `void` return type and takes no arguments,
`INativeAction<T>` has a `void` return type and takes one argument of type `T`,
`INativeFunc<TResult>` takes no arguments and has a return type of `TResult`,
and so forth.

#### `IUnmanagedAction` and `IUnmanagedFunc` interfaces

These interfaces __require__ the _/unsafe_ compiler switch __and__ the constant
__UNSAFE__ to be defined. These interfaces _will not compile_ without both of
these settings.

`IUnmanagedAction` and `IUnmanagedFunc` take _twice as many_ type arguments as
the matching `System.Action` and `System.Func`, respectively.

The type arguments are prefixed with a `T` or a `U`, which denotes the meaning
of the type argument.

`T`-prefixed type arguments represent the _managed_ type arguments. These
would be the same type arguments that you would use with `System.Action` or
`System.Func`, and match the parameters and/or return type of the `Invoke`
method.

`U`-prefixed type arguments represent the _unmanaged_ type arguments. These
would be the types of the arguments which you would use to invoke the method
_in native code_. The `U`-prefixed type arguments have the _unmanaged_ type
constraint.

The `IUnmanagedAction` and `IUnmanagedFunc` interfaces implement the
`INativeAction` and `INativeFunc` interfaces, respectively, with the
`T`-prefixed type arguments only. This can be used for type-erasure if hiding
the unmanaged type argument list from the signature is desirable.

A native generic delegate instance created using the `INativeAction` or
`INativeFunc` interfaces will _not_ implement the `IUnmanagedAction` or
`IUnmanagedFunc` interfaces.

### `IMarshaller<TSelf>` interface

```C#
internal interface IMarshaller<TSelf> where TSelf : IMarshaller<TSelf>
```

This interface provides `static virtual` properties which, when explicitly
implemented by a user-defined type are used to define the custom marshalling
behaviors. The default for each property is `null`, which will not implement
any custom marshalling.

The values defined by these properties __must__ be parsable at compile-time in
order for appropriate source code to be generated for your native generic
delegate wrappers. Any of these values which are not specified must follow the
same parsing rules as `System.Attribute` arguments, or they will be ignored.
Namely, you cannot reference fields, properties, or methods except as otherwise
noted below.

#### `CallingConvention` property (`IMarshaller<TSelf>`)

```C#
protected static virtual CallingConvention? CallingConvention { get; }
```

_This is the static property of the `IMarshaller<TSelf>` interface. For the
native generic delegate instance property, see [`CallingConvention`
property](#callingconvention-property)._

Defines the unmanaged calling convention.

If specified, this value must be `null` or one of the enumerated values defined
in `System.Runtime.InteropServices.CallingConvention` (e.g.,
`CallingConvention.Cdecl` or `CallingConvention.StdCall`). Any other value
(such as a reference to another field, property, or the result of a method
call) will be ignored.

This property is only used when the calling convention parameter of the factory
method (`FromAction`, `FromFunc`, or `FromFunctionPointer`) is omitted. When
that parameter is supplied, this property will be ignored for that invocation.

This property is always ignored when invoking the `FromFunctionPointer` methods
of an `IUnmanagedAction` or `IUnmanagedFunc` with an unmanaged function
pointer.

#### `MarshalMap` property

```C#
protected static virtual MarshalMap? MarshalMap { get; }
```

Defines the [MarshalMap](#marshalmap-class).

If specified, this value must be `null` or a `new()` expression with a
collection initializer. Each element in the collection initializer must be a
`KeyValuePair<Type, MarshalAsAttribute>`. The key must be a `typeof`
expression, and the value must be a `new()` expression with an optional
initializer.

Failure to parse any elements in the map will result in the entire map being
discarded.

#### `MarshalParamsAs` property

```C#
protected static virtual MarshalAsAttribute?[]? MarshalParamsAs { get; }
```

Defines the `MarshalAsAttribute` that will annotate each parameter of the
`Invoke` method.

If specified, this value must be `null` or a `new()` expression with a
collection initializer. Each element in the collection initializer must be a
`new()` expression with an optional initializer.

Failure to parse any elements in the collection will result in the entire
collection being discarded.

This property is ignored if `Invoke` does not take any parameters.

#### `MarshalReturnAs` property

```C#
protected static virtual MarshalAsAttribute? MarshalReturnAs { get; }
```

Defines the `MarshalAsAttribute` that will annotate the return value of the
`Invoke` method.

If specified, this value must be `null` or a `new()` expression with an
optional initializer.

Failure to parse this value will result in the value being discarded.

This property is ignored if `Invoke` does not return a value.

### `MarshalMap` class

```C#
internal sealed class MarshalMap
```

Represents a mapping between `Type`s and `MarshalAsAttribute`s.

When the
[IMarshaller&lt;TSelf&gt;.MarshalMap property](#marshalmap-property) is
specified, this represents a "default" marshalling behavior for parameters of
the `Invoke` method. For example, if `typeof(string)` is mapped to
`new MarshalAsAttribute(UnmanagedType.LPUTF8Str)`, then any `string` parameter
of the `Invoke` method will, by default, be marshalled as a UTF-8 string.

The mapped values are always overridden by the
[MarshalParamsAs](#marshalparamsas-property) and
[MarshalReturnAs](#marshalreturnas-property) properties (where applicable).

__NOTE:__ This type is provided as a __compile-time only__ construct. The
mapped values are not stored and not available at runtime. While this type does
implement `IEnumerable<T>`, this is only because it is required for collection
initializer support. Attempting to enumerate the map at runtime will cause a
`NotImplementedException` to be thrown.

### `FromAction` and `FromFunc` methods

```C#
// non-generic methods
public static INativeAction FromAction(Action, optional CallingConvention)
public static INativeAction<...> FromAction(Action<...>, optional CallingConvention)
public static INativeFunc<...> FromFunc(Func<...>, optional CallingConvention)
public static unsafe IUnmanagedAction FromAction(Action, optional CallingConvention)
public static unsafe IUnmanagedAction<...> FromAction(Action<...>, optional CallingConvention)
public static unsafe IUnmanagedFunc<...> FromFunc(Func<...>, optional CallingConvention)
// generic methods
public static INativeAction<...> FromAction<TMarshaller>(Action<...>, optional CallingConvention)
public static INativeFunc<...> FromFunc<TMarshaller>(Func<...>, optional CallingConvention)
public static unsafe IUnmanagedAction<...> FromAction<TMarshaller>(Action<...>, optional CallingConvention)
public static unsafe IUnmanagedFunc<...> FromFunc<TMarshaller>(Func<...>, optional CallingConvention)
	where TMarshaller : IMarshaller<TMarshaller>, new()
```

_Note: Using the `IUnmanagedAction` and `IUnmanagedFunc` interfaces require
unsafe compiler options. See [`IUnmanagedAction` and `IUnmanagedFunc`
interfaces](#iunmanagedaction-and-iunmanagedfunc-interfaces) for details._

#### Non-generic methods

Each interface provides a static, _non-generic_ method named either
`FromAction` or `FromFunc` (respective of the containing interface). This
method takes a managed delegate as its first parameter whose type arguments
match those of the interface (for `IUnmanagedAction` and `IUnmanagedFunc`,
these are the `T`-prefixed type arguments). The second argument is an
_optional_ `CallingConvention`, which will default to
`CallingConvention.Winapi`.

#### Generic methods

Except for the `INativeAction` and `IUnmanagedAction` interfaces with no type
arguments, each interface also provides a static, _generic_ method by the same
name (`FromAction` or `FromFunc`) with the same arguments as the non-generic
method.

These generic methods take a single type argument, `TMarshaller`. The
`TMarshaller` type argument is constrained to be a type which implements the
[IMarshaller&lt;TSelf&gt; interface](#imarshallertself-interface) and has the
`new()` constraint. The type supplied for this type argument specifies the
marshalling behaviors for the returned instance. See `IMarshaller<TSelf>` for
details.

#### Return value

Each of these methods return an instance of the interface to which the static
method belongs. This instance can then be used to invoke the native generic
delegate from both managed and unmanaged code. See
[GetFunctionPointer](#getfunctionpointer-method) and [Invoke](#invoke-method)
for details.

### `FromFunctionPointer` methods

```C#
// non-generic methods
public static INativeAction FromFunctionPointer(nint, optional CallingConvention)
public static INativeAction<...> FromFunctionPointer(nint, optional CallingConvention)
public static INativeFunc<...> FromFunctionPointer(nint, optional CallingConvention)
public static unsafe IUnmanagedAction FromFunctionPointer(nint, optional CallingConvention)
public static unsafe IUnmanagedAction<...> FromFunctionPointer(nint, optional CallingConvention)
public static unsafe IUnmanagedFunc<...> FromFunctionPointer(nint, optional CallingConvention)
public static unsafe IUnmanagedAction FromFunctionPointer(delegate* unmanaged[CALL_CONV]<void>)
public static unsafe IUnmanagedAction<...> FromFunctionPointer(delegate* unmanaged[CALL_CONV]<..., void>)
public static unsafe IUnmanagedFunc<...> FromFunctionPointer(delegate* unmanaged[CALL_CONV]<...>)
// generic methods
public static INativeAction<...> FromFunctionPointer<TMarshaller>(nint, optional CallingConvention)
public static INativeFunc<...> FromFunctionPointer<TMarshaller>(nint, optional CallingConvention)
public static unsafe IUnmanagedAction<...> FromFunctionPointer<TMarshaller>(nint, optional CallingConvention)
public static unsafe IUnmanagedFunc<...> FromFunctionPointer<TMarshaller>(nint, optional CallingConvention)
public static unsafe IUnmanagedAction<...> FromFunctionPointer<TMarshaller>(delegate* unmanaged[CALL_CONV]<..., void>)
public static unsafe IUnmanagedFunc<...> FromFunctionPointer<TMarshaller>(delegate* unmanaged[CALL_CONV]<...>)
	where TMarshaller : IMarshaller<TMarshaller>, new()
	// where CALL_CONV is one of:
	//     - Cdecl
	//     - Stdcall
	//     - Thiscall
```

_Note: Using the `IUnmanagedAction` and `IUnmanagedFunc` interfaces require
unsafe compiler options. See [`IUnmanagedAction` and `IUnmanagedFunc`
interfaces](#iunmanagedaction-and-iunmanagedfunc-interfaces) for details._

#### Non-generic methods

Each interface provides a static, _non-generic_ method,
`FromFunctionPointer(nint, CallingConvention)`. This method takes a `nint` as
its first parameter which represents a function pointer to an unmanaged method
(a method in native code, _or_ a method in managed code marked with the
`UnmanagedCallersOnlyAttribute`). The second argument is an _optional_
`CallingConvention`, which will default to `CallingConvention.Winapi`.

The `IUnmanagedAction` and `IUnmanagedFunc` interfaces also each provide three
additional overloads of `FromFunctionPointer` which only take a single
argument: an unmanaged function pointer. The calling convention is part of the
signature of the function pointer, and cannot be overridden. The function
pointer type arguments match the `U`-prefixed interface type arguments, with a
trailing `void` return type for the `IUnmanagedAction` interfaces.

#### Generic methods

Except for the `INativeAction` and `IUnmanagedAction` interfaces with no type
arguments, each interface also provides a static, _generic_ method by the same
name (`FromFunctionPointer`) with the same arguments as the non-generic method
(`IUnmanagedAction` and `IUnmanagedFunc` provide the same _four_ overloads for
the generic method as are provided for the non-generic method).

These generic methods take a single type argument, `TMarshaller`. The
`TMarshaller` type argument is constrained to be a type which implements the
[IMarshaller&lt;TSelf&gt; interface](#imarshallertself-interface) and has the
`new()` constraint. The type supplied for this type argument specifies the
marshalling behaviors for the returned instance. See `IMarshaller<TSelf>` for
details.

_Note: The `TMarshaller` type cannot override the calling convention when
creating a native generic delegate from an unmanaged function pointer (via
the `IUnmanagedAction` or `IUnmanagedFunc` overloads)._

#### Return value

Each of these methods return an instance of the interface to which the static
method belongs. This instance can then be used to invoke the native generic
delegate from both managed and unmanaged code. See
[GetFunctionPointer](#getfunctionpointer-method) and [Invoke](#invoke-method)
for details.

### `CallingConvention` property

```C#
public CallingConvention CallingConvention { get; }
```

_This is the instance property of a native generic delegate instance. For
the `IMarshaller<TSelf>` static property, see [`CallingConvention` property
(`IMarshaller<TSelf>`)](#callingconvention-property-imarshallertself)._

This property returns the calling convention used when invoking the native
generic delegate from native or unmanaged code.

### `GetFunctionPointer` method

```C#
public nint GetFunctionPointer()
```

Each interface provides this method, which returns a function pointer which can
be passed into native code. This function pointer can be safely invoked from
native code for as long as you retain the instance which provided the function
pointer.

To invoke the instance from managed code, see [Invoke](#invoke-method).

__CAUTION: If the managed instance is garbage collected, then the function
pointer returned by this method will no longer be safe to invoke. You must
ensure that this function pointer is not invoked after your managed instance
has been garbage collected.__

### `Invoke` method

```C#
// INativeAction and IUnmanagedAction
public void Invoke()
// INativeAction<...> and IUnmanagedAction<...>
public void Invoke(...)
// INativeFunc<...> and IUnmanagedFunc<...>
public TResult Invoke(...)
```

Each interface provides a method named `Invoke`. The return type and parameters
of this method depend on the interface which was used to create the instance.

For the `INativeAction` and `INativeFunc` interfaces, the return type and
parameters of this method mirror those of `System.Action` and `System.Func`
with the same type parameters as the interface.

For the `IUnmanagedAction` and `IUnmanagedFunc` interfaces, the return type and
parameters of this method mirror those of `System.Action` and `System.Func`
with the `T`-prefixed type parameters of the interface.

To invoke the instance from native code, see
[GetFunctionPointer](#getfunctionpointer-method).

### `ToAction` and `ToFunc` methods

```C#
// INativeAction and IUnmanagedAction
public Action ToAction()
// INativeAction<...> and IUnmanagedAction<...>
public Action<...> ToAction()
// INativeFunc<...> and IUnmanagedFunc<...>
public Func<...> ToFunc()
```

Each interface provides a method named `ToAction` or `ToFunc`, respective to
the interface. These methods return a managed delegate which matches the
signature of the `Invoke` method.

For the `INativeAction` and `INativeFunc` interfaces, the type arguments of the
returned delegate will match the interface type arguments (if any).

For the `IUnmanagedAction` and `IUnmanagedFunc` interfaces, the type arguments
of the returned delegate will match the `T`-prefixed interface type arguments
(if any).

### `AsCdeclPtr`, `AsStdCallPtr`, and `AsThisCallPtr` properties

```C#
public unsafe delegate* unmanaged[Cdecl]<...> AsCdeclPtr { get; }
public unsafe delegate* unmanaged[Stdcall]<...> AsStdCallPtr { get; }
public unsafe delegate* unmanaged[Thiscall]<...> AsThisCallPtr { get; }
```

_Note: Using the `IUnmanagedAction` and `IUnmanagedFunc` interfaces require
unsafe compiler options. See [`IUnmanagedAction` and `IUnmanagedFunc`
interfaces](#iunmanagedaction-and-iunmanagedfunc-interfaces) for details._

These properties are provided _only_ by the `IUnmanagedAction` and
`IUnmanagedFunc` interfaces.

The unmanaged function pointer type arguments match the `U`-prefixed interface
type arguments. For `IUnmanagedAction` interfaces, the `void` return type is
added to the end of the function pointer type argument list.

Each of these properties will respect the
[`CallingConvention`](#callingconvention-property) of the instance, and will
return `null` if you attempt to access the wrong calling convention.

If `CallingConvention` is `CallingConvention.Winapi`, then the value of the
`AsCdeclPtr` and `AsStdCallPtr` properties depends on the current operating
system. If the operating system is Windows, then `AsStdCallPtr` will return a
non-`null` pointer, and `AsCdeclPtr` will return `null`; otherwise, if the
operating system is not Windows, then `AsCdeclPtr` will return a non-`null`
pointer, and `AsStdCallPtr` will return `null`. `AsThisCallPtr` will always
return `null` when `CallingConvention` is anything other than
`CallingConvention.ThisCall`.

When non-`null`, these properties can be used to directly invoke the unmanaged
function pointer from managed code, or they can be passed to native code.

__CAUTION: If the managed instance is garbage collected, then the function
pointers returned by these properties will no longer be safe to invoke. You
must ensure that these function pointers are not invoked after your managed
instance has been garbage collected.__

### Examples

```C#
[UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
private static void Print(nint message)
{
	var str = Marshal.PtrToStringUni(message);
	Console.WriteLine(str);
}

var print = IUnmanagedAction<string, nint>.FromFunctionPointer(&Print);
var str = Marshal.StringToCoTaskMemUni("Hello World!");
print.AsCdeclPtr(str); // prints "Hello World!"
Marshal.FreeCoTaskMem(str);
print.Invoke("Goodbye, Galaxy!"); // prints "Goodbye, Galaxy!"
```

```C#
internal sealed class Utf8Marshaller : IMarshaller<Utf8Marshaller>
{
	static MarshalMap? IMarshaller<Utf8Marshaller>.MarshalMap => new()
	{
		{ typeof(string), new MarshalAsAttribute(UnmanagedType.LPUTF8Str) }
	};
}

[UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
private static void PrintUtf8(nint message)
{
	var str = Marshal.PtrToStringUTF8(message);
	Console.WriteLine(str);
}

var printUtf8 = IUnmanagedAction<string, nint>.FromFunctionPointer<Utf8Marshaller>(&PrintUtf8);
var str = Marshal.StringToCoTaskMemUTF8("Hello UTF-8!");
printUtf8.AsStdCallPtr(str);
Marshal.FreeCoTaskmem(str);
```
