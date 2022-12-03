# NativeGenericDelegates

__NativeGenericDelegates__ is a C# project designed to provide [Delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate?view=net-7.0)
`class` types that are _[generic](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics)_
and can be used from _native_ code with _[platform invoke](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)_
(P/Invoke). There is a historical caveat to .NET generics in that they cannot
be used with P/Invoke. [Marshal.GetFunctionPointerForDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getfunctionpointerfordelegate?view=net-7.0#system-runtime-interopservices-marshal-getfunctionpointerfordelegate(system-delegate))
in particular will throw an `ArgumentException` if passed a generic delegate
type. This means that you are often left with no option but to create your own
delegate types as-needed, and there is little to no room for code reusability.

This project was inspired by a [StackOverflow question](https://stackoverflow.com/questions/26699394/c-sharp-getdelegateforfunctionpointer-with-generic-delegate)
for which the correct solution was, in fact, to use the `DllImportAttribute`.
However, there are still scenarios where it may be desirable to utilize generic
delegates with P/Invoke and this project aims to facilitate those use cases.

This solution involves a lot of boilerplate code and may not be a good fit for
every project, particularly if you are only using a small number of delegates.

## What this project does

__IMPORTANT NOTE: This project relies on dynamic code using types from the
[System.Reflection.Emit](https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/emitting-dynamic-methods-and-assemblies)
namespace. These types are not supported on all .NET platforms.__

This project provides a set of `interface`s that mirror the `System.Action` and
`System.Func` delegate types with the following features:

* Generic interface (e.g., `INativeAction<int>`, `INativeFunc<double, bool>`, etc.)
* Full interoperability with APIs consuming `Delegate` objects (via cast to `Delegate`)
* Passing delegate to unmanaged code (via `GetFunctionPointer`)
* Construct delegates from managed and unmanaged functions with common interface
* Non-dynamic invocation from managed code (via `Invoke`, no calls to `Delegate.DynamicInvoke`)
* Define unmanaged function pointer calling convention
* Optionally define marshaling behavior for return value and parameters
* Convert to `System.Action` and `System.Func` delegate types (`ToAction` and `ToFunc`)

## How to use this project

__You just DO WHAT THE FUCK YOU WANT TO.__

As per the [license terms](COPYING.md), you are free to use this
project however you see fit, but the following contains an overview of the
public API that the code exposes:

### INativeAction and INativeFunc
```C#
INativeAction<T..16>
INativeFunc<T..16, TResult>
```

There are 17 variations each of the `INativeAction` and `INativeFunc`
interfaces that mirror `System.Action` and `System.Func` respectively.
Similarly, `INativeAction` represents a method with no return value (`void`
return type) and `INativeFunc` takes the return type as the final generic
type parameter (`TResult`). For brevity, the shorthand `<T..16>`/`<T..16,
TResult>` will be used to denote all 17 variations of the interface.

#### Converting to and from Delegate

Every instance of `INativeAction<T..16>` and `INativeFunc<T..16, TResult>` that
is exposed by the methods and properties below are *actual* instances of a
`class` that is derived from `System.MulticastDelegate` (which in turn is
derived from `System.Delegate`). Any APIs using these base classes can accept
`INativeAction` or `INativeFunc` instances, but this requires an explicit cast
(as implicit conversions to or from `interface`s are forbidden):

```C#
private void Foo(int i) { }
private void Bar(Delegate d) { }

var foo = INativeAction<int>.FromDelegate(Foo, CallingConvention.Cdecl);
Bar((Delegate)foo);
```

The conversion in the other direction can also be done with an explicit cast,
and conversions in both directions can use the `is` or `as` operators:

```C#
if (d is INativeAction<int> action)
{
	action.Invoke(42); // avoid expensive call to DynamicInvoke
}
INativeAction<int>? maybeAction = d as INativeAction<int>;
INativeAction<int> invalidCastExceptionIfWrong = (INativeAction<int>)d;
```

#### Custom marshaling

The methods below permit you to specify custom marshaling behavior for the
native generic delegates that you create. The default behavior when
constructing these delegates using [FromDelegate](#fromdelegate) or
[FromMethod](#frommethod) is to copy the marshaling behavior from the method.
In those method's `marshalReturnAs` and `marshalParamAs` parameters, a `null`
value will be used to represent this default behavior. If you explicitly want
the delegate to have no custom marshaling behavior defined, you may use either
`INativeAction.NoCustomMarshaling` or `INativeFunc.NoCustomMarshaling`.

```C#
static MarshalAsAttribute INativeAction.NoCustomMarshaling
static MarshalAsAttribute INativeFunc.NoCustomMarshaling
```

(_NOTE: `INativeFunc` is a `static` class to mirror `INativeAction` when
accessing this property, which is only accessible through the parameterless
`INativeAction` interface or the static `INativeFunc` class. The two references
are equivalent._)

This does __not__ turn off custom marshaling of the underlying methods that
your native generic delegates will represent. This only affects the marshaling
behavior defined for the delegate type itself.

When using [FromFunctionPointer](#fromfunctionpointer), values of `null` and
`NoCustomMarshaling` are the same, as there is no managed method to copy
marshaling behaviors from.

#### FromDelegate
```C#
static INativeAction<T..16> INativeAction<T..16>.FromDelegate(Delegate d, CallingConvention callingConvention, [optional] MarshalAsAttribute?[]? marshalParamAs)
static INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromDelegate(Delegate d, CallingConvention callingConvention, [optional] MarshalAsAttribute? marshalReturnAs, [optional] MarshalAsAttribute?[]? marshalParamAs)
```

Creates a native generic delegate with the same signature as the interface that
will invoke the same method (and target, if any) as the given delegate `d`.

`d` is the delegate from which the invocation method and target will be copied.

`callingConvention` is the calling convention of the unmanaged function pointer
returned by [GetFunctionPointer](#getfunctionpointer).

`marshalReturnAs` is an optional `MarshalAsAttribute` that controls the
marshaling behavior of the managed delegate return value, if any.
`INativeAction<T..16>` omits this parameter as there is no return value. If
this parameter is `null`, the marshaling behavior for the new delegate's return
value will be copied from the marshaling of the managed method (`d.Method`;
**this is the default**). To specify that the new delegate should have no
explicit marshaling, you may pass [NoCustomMarshaling](#custom-marshaling). Any
custom marshaling of the underlying method's return value will still be
preserved, but not represented in the new delegate type.

`marshalParamAs` is an optional array of `MarshalAsAttribute`s that control the
marshaling behavior of the managed delegate parameters. Delegates that accept
no parameters (`INativeAction` and `INativeFunc<TResult>`) omit this parameter.
The length and order of the array must match the function signature. If this
parameter is `null`, the marshaling behavior for the new delegate's parameters
will be copied from the marshaling of the managed method's (`d.Method`'s)
parameters (**this is the default**). To specify that a parameter in the new
delegate type should have no explicit marshaling, you may pass
[NoCustomMarshaling](#custom-marshaling). Any custom marshaling of the
underlying method's parameters will still be preserved, but not represented in
the new delegate type.

_Returns:_ The new delegate instance.

_Example:_

```C#
public static void OnNativeEvent(int eventID)
{
	Console.WriteLine($"Native event {eventID} raised.");
}

var nativeEventHandler = INativeAction<int>.FromDelegate(OnNativeEvent, CallingConvention.Cdecl);
NativeMethods.SetNativeEventCallback(nativeEventHandler.GetFunctionPointer());
```

_See also:_
[FromFunctionPointer](#fromfunctionpointer),
[FromMethod](#frommethod),
[GetFunctionPointer](#getfunctionpointer),
[Invoke](#invoke),
[Method](#method),
[Target](#target)

#### FromFunctionPointer

```C#
static INativeAction INativeAction.FromFunctionPointer(nint functionPtr, CallingConvention callingConvention)
static INativeAction<T..16> INativeAction<T..16>.FromFunctionPointer(nint functionPtr, CallingConvention callingConvention, [optional] MarshalAsAttribute?[]? marshalParamAs)
static unsafe INativeAction INativeAction.FromFunctionPointer(delegate* unmanaged[CALL_CONV]<void> functionPtr)
static unsafe INativeAction<T..16> INativeAction<T..16>.FromFunctionPointer(delegate* unmanaged[CALL_CONV]<T..16, void> functionPtr, [optional] MarshalAsAttribute?[]? marshalParamAs)
static unsafe INativeAction<T..16> INativeAction<T..16>.FromFunctionPointer<U..16>(delegate* unmanaged[CALL_CONV]<U..16, void> functionPtr, [optional] MarshalAsAttribute?[]? marshalParamAs)
static INativeFunc<TResult> INativeFunc<TResult>.FromFunctionPointer(nint functionPtr, CallingConvention callingConvention, [optional] MarshalAsAttribute? marshalReturnAs)
static INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromFunctionPointer(nint functionPtr, CallingConvention callingConvention, [optional] MarshalAsAttribute? marshalReturnAs, [optional] MarshalAsAttribute?[]? marshalParamAs)
static unsafe INativeFunc<TResult> INativeFunc<TResult>.FromFunctionPointer(delegate* unmanaged[CALL_CONV]<TResult> functionPtr, [optional] MarshalAsAttribute? marshalReturnAs)
static unsafe INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromFunctionPointer(delegate* unmanaged[CALL_CONV]<T..16, TResult> functionPtr, [optional] MarshalAsAttribute? marshalReturnAs, [optional] MarshalAsAttribute?[]? marshalParamAs)
static unsafe INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromFunctionPointer<U..16, UResult>(delegate* unmanaged[CALL_CONV]<U..16, UResult> functionPtr, [optional] MarshalAsAttribute? marshalReturnAs, [optional] MarshalAsAttribute?[]? marshalParamAs)
```

*Methods indicated as `unsafe` require the `/unsafe` compiler switch.*

***CALL_CONV** is one of `Cdecl`, `Stdcall`, or `Thiscall`. `Fastcall` is not supported.*

Creates a native generic delegate with the same signature as the interface that
will invoke an unmanaged function pointer.

The overloads that accept type parameters for the unmanaged function pointer
`<U..16>`/`<U..16, UResult>` permit you to change the managed delegate
parameter types from the native function's parameter types (`<T..16>`/`<T..T16,
TResult>` refers to the __managed__ types and `<U..16>`/`<U..16, UResult>`
refers to the __unmanaged__ types). If you do change any of the parameter
types, you must use `marshalParamAs` to define the correct marshaling behavior.

`functionPtr` is the unmanaged function pointer that will be invoked by the
delegate.

`callingConvention` is the calling convention of the unmanaged function pointer
returned by [GetFunctionPointer](#getfunctionpointer). The overloads that take
an unmanaged function pointer omit this parameter, as it is inferred from
`CALL_CONV` instead.

`marshalReturnAs` is an optional `MarshalAsAttribute` that controls the
marshaling behavior of the managed delegate return value, if any.
`INativeAction<T..16>` omits this parameter as there is no return value. If no
custom marshaling behavior is needed for the return value, this should be
`null` (**this is the default**).

`marshalParamAs` is an optional array of `MarshalAsAttribute`s that control the
marshaling behavior of the managed delegate parameters. Delegates that accept
no parameters (`INativeAction` and `INativeFunc<TResult>`) omit this parameter.
The length and order of the array must match the function signature. If no
custom marshaling behavior is needed for a parameter, the corresponding index
in the array should be `null`. If no custom marshaling is needed for any
parameters, the array may be `null` (**this is the default**).

_Returns:_ The new delegate instance.

_Example:_

```C#
nint pMyDllFunc = NativeMethods.GetProcAddress(myDllHandle, "MyDllFunc"); // get some function pointer from native code
var myDllFuncHandler = INativeFunc<int, string>.FromFunctionPointer
(
	functionPtr: pMyDllFunc,
	callingConvention: CallingConvention.StdCall,
	marshalReturnAs: new MarshalAsAttribute(UnmanagedType.LPUTF8Str)
);
Console.WriteLine(myDllFuncHandler.Invoke(42));
```

_Where possible, `DllImportAttribute` or `LibraryImportAttribute` are likely a
better option to import a method from an unmanaged library. This method may be
useful if an unmanaged library exposes method handles that are not exported or
are expensive to load, or if you need to invoke the method from managed code
while maintaining a pointer to the method._

_See also:_
[FromDelegate](#fromdelegate),
[FromMethod](#frommethod)
[GetFunctionPointer](#getfunctionpointer),
[Invoke](#invoke)

#### FromMethod
```C#
static INativeAction<T..16> INativeAction<T..16>.FromMethod(object? target, MethodInfo method, CallingConvention callingConvention, [optional] MarshalAsAttribute?[]? marshalParamAs)
static INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromMethod(object? target, MethodInfo method, CallingConvention callingConvention, [optional] MarshalAsAttribute? marshalReturnAs, [optional] MarshalAsAttribute?[]? marshalParamAs)
```

Creates a native generic delegate with the same signature as the interface that
will invoke the given method `method` with the given target `target`.

Marshaling behavior matches any `MarshalAsAttribute`s that are applied to the
parameters and return value of `method`.

`target` is the class instance on which the `method` will be invoked. Should be
`null` if `method` is a `static` method.

`method` is the method that will be invoked by this delegate.

`callingConvention` is the calling convention of the unmanaged function pointer
returned by [GetFunctionPointer](#getfunctionpointer).

`marshalReturnAs` is an optional `MarshalAsAttribute` that controls the
marshaling behavior of the managed delegate return value, if any.
`INativeAction<T..16>` omits this parameter as there is no return value. If
this parameter is `null`, the marshaling behavior for the new delegate's return
value will be copied from the marshaling of the managed `method` (**this is the
default**). To specify that the new delegate should have no explicit
marshaling, you may pass [NoCustomMarshaling](#custom-marshaling). Any custom
marshaling of the underlying method's return value will still be preserved, but
not represented in the new delegate type.

`marshalParamAs` is an optional array of `MarshalAsAttribute`s that control the
marshaling behavior of the managed delegate parameters. Delegates that accept
no parameters (`INativeAction` and `INativeFunc<TResult>`) omit this parameter.
The length and order of the array must match the function signature. If this
parameter is `null`, the marshaling behavior for the new delegate's parameters
will be copied from the marshaling of the managed `method`'s parameters (**this
is the default**). To specify that a parameter in the new delegate type should
have no explicit marshaling, you may pass
[NoCustomMarshaling](#custom-marshaling). Any custom marshaling of the
underlying method's parameters will still be preserved, but not represented in
the new delegate type.

_Returns:_ The new delegate instance.

_Example:_

```C#
public static void OnNativeEvent(int eventID)
{
	Console.WriteLine($"Native event {eventID} raised.");
}

var nativeEventHandler = INativeAction<int>.FromMethod
(
	target: null,
	method: typeof(Program).GetMethod(nameof(OnNativeEvent))!,
	callingConvention: CallingConvention.Cdecl
);
NativeMethods.SetNativeEventCallback(nativeEventHandler.GetFunctionPointer());
```

_See also:_
[FromDelegate](#fromdelegate)
[FromFunctionPointer](#fromfunctionpointer),
[GetFunctionPointer](#getfunctionpointer),
[Invoke](#invoke),
[Method](#method),
[Target](#target)

#### GetFunctionPointer
```C#
nint INativeAction<T..16>.GetFunctionPointer()
nint INativeFunc<T..16, TResult>.GetFunctionPointer()
```

Converts the delegate into a function pointer that is callable from unmanaged
code.

You must keep a managed reference to the delegate for the lifetime of the
unmanaged function pointer.

_Returns:_ A value that can be passed to unmanaged code, which, in turn, can
use it to call the underlying managed delegate.

_Example:_

```C#
public static void OnNativeEvent(int eventID)
{
	Console.WriteLine($"Native event {eventID} raised.");
}

var nativeEventHandler = INativeAction<int>.FromMethod
(
	target: null,
	method: typeof(Program).GetMethod(nameof(OnNativeEvent))!,
	callingConvention: CallingConvention.Cdecl
);
NativeMethods.SetNativeEventCallback(nativeEventHandler.GetFunctionPointer());
```

#### Invoke
```C#
void INativeAction<T..16>.Invoke(T..16 t..16);
TResult INativeFunc<T..16, TResult>.Invoke(T..16 t..16)
```

Invokes the managed or unmanaged method that the delegate represents with the
given parameters.

_Note:_ This does __not__ call `Delegate.DynamicInvoke` and does not incur the
performance penalty associated with that method.

_Returns:_ Nothing (`INativeAction<T..16>`) or `TResult`
(`INativeFunc<T..16, TResult>`).

_Example:_

```C#
var printPoint = INativeAction<int, int>.FromDelegate
(
	(int x, int y) => Console.WriteLine($"Point {{ X = {x}, Y = {y} }}"),
	CallingConvention.Cdecl
);
printPoint.Invoke(420, 69);
```

_See also:_
[FromDelegate](#fromdelegate)
[FromFunctionPointer](#fromfunctionpointer),
[FromMethod](#frommethod),
[GetFunctionPointer](#getfunctionpointer),
[Method](#method),
[Target](#target)

#### ToAction and ToFunc
```C#
Action<T..16> INativeAction<T..16>.ToAction()
Func<T..16, TResult> INativeFunc<T..16, TResult>.ToFunc()
```

Creates an `System.Action` or `System.Func` with the same [Target](#target) and
[Method](#method) as the delegate.

_Note:_ The returned delegate is __not__ one that implements the
`INativeAction` or `INativeFunc` interfaces. You can convert back to an
equivalent delegate using the [FromDelegate](#fromdelegate) method.

_Returns:_ The requested delegate.

_Example:_

```C#
public static void InvokeAction(Action<int, int> action, int x, int y)
{
	action(x, y);
}

var printPoint = INativeAction<int, int>.FromDelegate
(
	(int x, int y) => Console.WriteLine($"Point {{ X = {x}, Y = {y} }}"),
	CallingConvention.Cdecl
);
InvokeAction(printPoint.ToAction(), 420, 69);
```

_See also:_
[GetFunctionPointer](#getfunctionpointer),
[Method](#method),
[Target](#target)

#### Method
```C#
MethodInfo INativeAction<T..16>.Method { get; }
MethodInfo INativeFunc<T..16, TResult>.Method { get; }
```

Gets the method that is represented by this delegate.

_Example:_

```C#
public static void Foo() { }

var foo = INativeAction.FromDelegate(Foo, CallingConvention.Cdecl);
Console.WriteLine($"{nameof(foo)} represents the method {foo.Method.Name}");
```

_See also:_
[FromDelegate](#fromdelegate),
[FromMethod](#frommethod)
[Target](#target)

#### Target
```C#
object? INativeAction<T..16>.Target { get; }
object? INativeFunc<T..16, TResult>.Target { get; }
```

Gets the class instance on which the current delegate invokes the instance
method, or `null` if this delegate represents a `static` method.

_Returns:_ The object on which the current delegate invokes the instance
method, if the delegate represents an instance method; `null` if the delegate
represents a static method.

_Example:_

```C#
public class Foo
{
	public void Bar(int i) { }
}

Foo foo = new();
var bar = INativeAction<int>.FromDelegate(foo.Bar, CallingConvention.Cdecl);
Console.WriteLine($"{nameof(bar)} target is {nameof(foo)}? {object.ReferenceEquals(bar.Target, foo)}"); // true
```

### Technical implementation detail

The "magic" behind the scenes here is that we use types from
`System.Reflection.Emit` to create a `class` type that inherits from
`System.MulticastDelegate`. It is explicitly disallowed to inherit from
`MulitcastDelegate` at compile-time, but types emitted dynamically at
runtime are permitted to do so. This emitted class is then also permitted to
permitted to implement an `interface` (while compile-time delegates cannot).

In the `INativeAction<T..16>` and `INativeFunc<T..16, TResult>` interfaces, the
only member that does __not__ have a default implementation (and thus must be
implemented by the class) is the `Invoke` method. Because the runtime class
type we define has this method, we can cast our runtime `Delegate`-derived
class to and from the interface that it implements.

See [DelegateFactory.cs](DelegateFactory.cs) for the implementation.
