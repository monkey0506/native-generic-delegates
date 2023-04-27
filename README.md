# NativeGenericDelegates

__NativeGenericDelegates__ is a C# project designed to provide
[delegate](https://learn.microsoft.com/en-us/dotnet/api/system.delegate?view=net-7.0)-like `class` types that are
_[generic](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics)_ and can be used from _native_ code with
_[platform invoke](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)_ (P/Invoke). There is a historical
caveat to .NET generics in that they cannot
be used with P/Invoke. [Marshal.GetFunctionPointerForDelegate](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getfunctionpointerfordelegate?view=net-7.0#system-runtime-interopservices-marshal-getfunctionpointerfordelegate(system-delegate))
in particular will throw an `ArgumentException` if passed a generic delegate
type. This means that you are often left with no option but to create your own
delegate types as-needed, and there is little to no room for code reusability.

This project was inspired by a
[StackOverflow question](https://stackoverflow.com/questions/26699394/c-sharp-getdelegateforfunctionpointer-with-generic-delegate)
for which the correct solution was, in fact, to use the `DllImportAttribute`. However, there are still scenarios where it may be
desirable to utilize generic delegates with P/Invoke and this project aims to facilitate those use cases.

This solution uses an [incremental generator](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)
which creates a lot of boilerplate code. This may not be a good fit for every project, particularly if you are only using a small
number of delegates.

## What this project does

This project provides a set of `interface`s that mirror the `System.Action` and `System.Func` delegate types with the following
features:

* Generic interface (e.g., `INativeAction<int>`, `INativeFunc<double, bool>`, etc.)
* Convert to `System.Action` and `System.Func` delegate types (`ToAction` and `ToFunc`)
* Full interoperability with APIs consuming `Delegate` objects (via intermediary call to `ToAction` or `ToFunc`)
* Passing delegate to unmanaged code (via `GetFunctionPointer`)
* Construct delegates from managed and unmanaged functions with common interface
* Non-dynamic invocation from managed code (via `Invoke`, no calls to `Delegate.DynamicInvoke`)
* Define unmanaged function pointer calling convention
* Optionally define marshaling behavior for return value and parameters
* [Native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/) deployment available (__new__ from v2.0.0)

## About this version

__v2.0.0__ is a complete rewrite of the _NativeGenericDelegates_ project. The previous version (v1.0.0) aimed to provide
dynamically instanciated `System.Delegate` objects that implemented one of the provided interfaces, but relied on classes from
`System.Reflection.Emit`, which is incompatible with some .NET platforms (those using native AOT deployment).

This version instead aims to create classes that implement the provided interfaces at compile-time, using an incremental
generator. As such, this version introduces __breaking changes__ in the API. ___Importantly___, the generated class objects are
__NOT__ instances of the `System.Delegate` or `System.MulticastDelegate` types, and `/unsafe` is __required__.

## How to use this project

__You just DO WHAT THE FUCK YOU WANT TO.__

As per the [license terms](COPYING.md), you are free to use this project however you see fit, but the following contains an
overview of the public API that the code exposes.

__IMPORTANT:__ This version __requires__ the `/unsafe` compile switch. A future version may not require unsafe code.

### INativeAction and INativeFunc
```C#
INativeAction<T..16>
INativeFunc<T..16, TResult>
```

There are 17 variations each of the `INativeAction` and `INativeFunc` interfaces that mirror `System.Action` and `System.Func`
respectively. Similarly, `INativeAction` represents a method with no return value (`void` return type) and `INativeFunc` takes
the return type as the final generic type parameter (`TResult`). For brevity, the shorthand `<T..16>`/`<T..16, TResult>` will be
used to denote all 17 variations of the interface.

#### Converting to and from Delegate

___BREAKING CHANGE:__ Unlike v1.0.0, the instances of `INativeAction<T..16>` and `INativeFunc<T..16, TResult>` exposed by the API
methods below are __NOT__ instances of `System.Delegate` or `System.MulticastDelegate`. Using the `as` or `is` operators to
compare an instance with a delegate will __always fail__._

Instances of `INativeAction<T..16>` and `INativeFunc<T..16, TResult>` are compatible with APIs that accept `System.Delegate`
objects, but this requires an explicit call to `ToAction` or `ToFunc` (respectively).

```C#
private void Foo(int i) { }
private void Bar(Delegate d) { }

var foo = INativeAction<int>.FromAction(Foo);
Bar(foo.ToAction());
```

The conversion in the other direction depends on a call to `FromAction` or `FromFunc`:

```C#
INativeAction<int>? nativeAction;
if (d is Action<int> action)
{
	nativeAction = INativeAction<int>.FromAction(action);
}
else
{
	Action<int> action = (Action<int>)Delegate.CreateDelegate(typeof(Action<int>), d.Target, d.Method);
	nativeAction = INativeAction<int>.FromAction(action);
}
SetNativeCallback(nativeAction.GetFunctionPointer());
```

#### Custom marshaling

___BREAKING CHANGE:__ Unlike v1.0.0, there is no way to implicitly copy the custom marshaling behavior from the provided managed
delegate. As such, `INativeAction.NoCustomMarshaling` and `INativeFunc.NoCustomMarshaling` have been removed._

___BREAKING CHANGE:__ The `marshalReturnAs` and `marshalParamsAs` parameters have restrictions on their use that must be observed
to correctly generate the custom marshaling behaviors. See below._

The methods below permit you to specify custom marshaling behavior for the native generic delegates that you create. The default
behavior when constructing these delegates using `FromAction` or `FromFunc` is to leave the marshaling behavior undefined by the
generated delegate. This is the same behavior that you would get if you created your own delegate type and did not supply any
`MarshalAs` attributes on the return value or the parameters. If the managed method represented by this delegate has explicit
marshaling behavior defined, that behavior will still be preserved.

When constructing a native generic delegate using `FromFunctionPointer`, custom marshaling behavior must be defined if you have
[non-blittable](https://learn.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types) arguments, or if
your managed and unmanaged parameters are not of the same type.

The `marshalReturnAs` and `marshalParamsAs` parameters may be `null`, which will be used to represent the default behavior.
Otherwise, these parameters are restricted in how they must be used. You may use inline `new` expression syntax, or you may
reference a __readonly__ field with inline initialization.

__IMPORTANT:__ If you use a field to define custom marshaling behavior, it __must__ be initialized inline. Assignment in a
constructor will __NOT__ be detected by the source generator and will not be reflected in the generated delegate type. This is
by-design and will not be changed in future versions.

```C#
private static readonly MarshalAsAttribute[] nativeCallbackMarshaling = new[] { new MarshalAsAttribute(UnmanagedType.LPWStr) }; // okay, inline initialization of readonly field

static ThisClass()
{
	nativeCallbackMarshaling = new[] { new MarshalAsAttribute(UnmanagedType.LPUTF8Str) }; // NOT DETECTED BY GENERATOR, THIS VALUE IS IGNORED
}

var nativeAction = INativeAction<string>.FromFunctionPointer<nint>(GetNativeCallback(), new[] { new MarshalAsAttribute(UnmanagedType.LPWStr) }); // okay, inline new expression
var nativeAction2 = INativeAction<string>.FromFunctionPointer<nint>(GetNativeCallback(), nativeCallbackMarshaling); // okay, reference to readonly field - ONLY REFLECTS VALUE FROM INLINE INITIALIZATION

var localMarshaling = new[] { new MarshalAsAttribute(UnmanagedType.LPWStr) };
var nativeAction3 = INativeAction<string>.FromFunctionPointer<nint>(GetNativeCallback(), localMarshaling); // compile-time error: must use inline new expression, null expression, or readonly field reference
```

#### FromAction and FromFunc
```C#
static INativeAction INativeAction.FromAction(Action action, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
static INativeAction<T..16> INativeAction<T..16>.FromAction(Action<T..16> action, [optional] MarshalAsAttribute?[]? marshalParamsAs = null, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
static INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromFunc(Func<T..16> func, [optional] MarshalAsAttribute? marshalReturnAs = null, [optional] MarshalAsAttribute?[]? marshalParamsAs = null, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
```

___BREAKING CHANGE:__ In v1.0.0, these methods were represented as `FromDelegate` and `FromMethod`. It is no longer supported to
create a native generic delegate directly from a `System.Delegate` or `System.Reflection.MethodInfo`._

Creates a native generic delegate with the same signature as the interface that will invoke the same method (and target, if any)
as the given delegate.

`action`/`func` is the delegate from which the invocation method and target will be copied.

`marshalReturnAs` is an optional `MarshalAsAttribute` that defines the custom marshaling behavior of the managed delegate return
value, if any. `FromAction` omits this parameter as there is no return value.

`marshalParamsAs` is an optional array of `MarshalAsAttribute?` that defines the custom marshaling behavior of the managed
delegate parameters, if any. `INativeAction.FromAction` omits this parameter as the delegate does not accept any parameters. If
this array is shorter than the number of parameters, the remaining entries are assumed as `null`.

`callingConvention` is the calling convention of the unmanaged function pointer returned by
[GetFunctionPointer](#getfunctionpointer). If calling convention is `CallingConvention.Winapi`, then the default platform calling
convention is used instead: `CallingConvention.StdCall` on Windows and `CallingConvention.Cdecl` on all other platforms.

If any `MarshalAsAttribute?` is `null` (for `marshalReturnAs`, any of the entries in the `marshalParamsAs` array, or the
`marshalParamsAs` array itself), then no custom marshaling behavior is applied for the respective parameter or return value. Any
marshaling behavior defined by the managed method that this native generic delegate represents will still be preserved, but the
marshaling behavior will not be defined in the generated managed delegate type.

_Returns:_ An object representing a managed delegate with the requested signature and marshaling behavior, which implements the
native generic delegate interface.

_Example:_

```C#
public static void OnNativeEvent(int eventID)
{
	Console.WriteLine($"Native event {eventID} raised.");
}

var nativeEventHandler = INativeAction<int>.FromAction(OnNativeEvent);
SetNativeEventCallback(nativeEventHandler.GetFunctionPointer());
```

_See also:_
[FromFunctionPointer](#fromfunctionpointer),
[GetFunctionPointer](#getfunctionpointer),
[Invoke](#invoke),
[ToAction](#toaction-and-tofunc),
[ToFunc](#toaction-and-tofunc)

#### FromFunctionPointer
```C#
static INativeAction INativeAction.FromFunctionPointer(nint functionPtr, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
static INativeAction<T..16> INativeAction<T..16>.FromFunctionPointer(nint functionPtr, [optional] MarshalAsAttribute?[]? marshalParamsAs = null, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
static INativeAction<T..16> INativeAction<T..16>.FromFunctionPointer<U..16>(nint functionPtr, MarshalAsAttribute[] marshalParamsAs, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
static INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromFunctionPointer(nint functionPtr, [optional] MarshalAsAttribute? marshalReturnAs = null, [optional] MarshalAsAttribute?[]? marshalParamsAs = null, CallingConvention callingConvention = CallingConvention.Winapi)
static INativeFunc<T..16, TResult> INativeFunc<T..16, TResult>.FromFunctionPointer<U..16, UResult>(nint functionPtr, MarshalAsAttribute marshalReturnAs, MarshalAsAttribute[] marshalParamsAs, [optional] CallingConvention callingConvention = CallingConvention.Winapi)
```

___BREAKING CHANGE:__ In v1.0.0, these methods accepted unmanaged function pointers as the first argument, which is no longer
supported._

Creates a native generic delegate with the same signature as the interface that will invoke an unmanaged function pointer.

The overloads that accept a second set of type parameters (`<U..16>`/`<U..16, UResult>`) permit you to specify both the _managed_
delegate parameter types (`<T..16>`/`<T..16, TResult>`) and the _unmanaged_ parameter types (`<U..16>`/`<U..16, UResult>`). For
example, you might use `INativeAction<string>.FromFunctionPointer<nint>` to describe a native generic delegate that accepts a
`System.String` parameter in managed code, and a `const char*` (`nint`) parameter in unmanaged code. The custom marshaling
behavior for these overloads is not optional.

`functionPtr` is the unmanaged function pointer that will be invoked by the delegate.

`marshalReturnAs` is an optional `MarshalAsAttribute` that defines the custom marshaling behavior of the managed delegate return
value, if any. `FromAction` omits this parameter as there is no return value.

`marshalParamsAs` is an optional array of `MarshalAsAttribute?` that defines the custom marshaling behavior of the managed
delegate parameters, if any. `INativeAction.FromAction` omits this parameter as the delegate does not accept any parameters. If
this array is shorter than the number of parameters, the remaining entries are assumed as `null`.

`callingConvention` is the calling convention of the unmanaged function pointer returned by
[GetFunctionPointer](#getfunctionpointer). If calling convention is `CallingConvention.Winapi`, then the default platform calling
convention is used instead: `CallingConvention.StdCall` on Windows and `CallingConvention.Cdecl` on all other platforms.

_Returns:_ An object representing a managed delegate with the requested signature and marshaling behavior, which implements the
native generic delegate interface.

_Example:_

```C#
nint pMyDllFunc = GetProcAddress(myDllHandle, "MyDllFunc"); // get some function pointer from native code
var myDllFuncHandler = INativeFunc<int, string>.FromFunctionPointer<int, nint>
(
	pMyDllFunc,
	new MarshalAsAttribute(UnmanagedType.LPUTF8Str),
	new[] { new MarshalAsAttribute(UnmanagedType.I4) }
);
Console.WriteLine(myDllFuncHandler.Invoke(42));
```

_Where possible, `DllImportAttribute` or `LibraryImportAttribute` are likely a better option to import a method from an unmanaged
library. This method may be useful if an unmanaged library exposes method handles that are not exported or are expensive to load,
or if you need to invoke the method from managed code while maintaining an unmanaged pointer to the method._

_See also:_
[FromAction](#fromaction-and-fromfunc),
[FromFunc](#fromaction-and-fromfunc),
[GetFunctionPointer](#getfunctionpointer),
[Invoke](#invoke),
[ToAction](#toaction-and-tofunc),
[ToFunc](#toaction-and-tofunc)

#### GetFunctionPointer
```C#
nint INativeAction<T..16>.GetFunctionPointer()
nint INativeFunc<T..16>.GetFunctionPointer()
```

Converts the native generic delegate into a function pointer that is callable from unmanaged code.

You must keep a managed reference to the native generic delegate instance for the lifetime of the unmanaged function pointer.

_Returns:_ A value that can be passed to unmanaged code, which, in turn, can use it to call the underlying managed delegate.

_Example:_

```C#
public static void OnNativeEvent(int eventID)
{
	Console.WriteLine($"Native event {eventID} raised.");
}

var nativeEventHandler = INativeAction<int>.FromAction(OnNativeEvent);
SetNativeEventCallback(nativeEventHandler.GetFunctionPointer());
```

_See also:_
[FromAction](#fromaction-and-fromfunc),
[FromFunc](#fromaction-and-fromfunc),
[FromFunctionPointer](#fromfunctionpointer),
[Invoke](#invoke),
[ToAction](#toaction-and-tofunc),
[ToFunc](#toaction-and-tofunc)

#### Invoke
```C#
void INativeAction<T..16>.Invoke(T..16 t..16)
TResult INativeFunc<T..16, TResult>.Invoke(T..16 t..16)
```

Invokes the managed or unmanaged method that this native generic delegate represents with the given parameters.

_Note:_ This does __not__ call `Delegate.DynamicInvoke` and does not incur the performance penalty associated with that method.

_Returns:_ Nothing (`INativeAction<T..16>`) or `TResult` (`INativeFunc<T..16, TResult>`).

_Example:_

```C#
var printPoint = INativeAction<int, int>.FromAction
(
	(int x, int y) => Console.WriteLine($"Point {{ X = {x}, Y = {y} }}")
);
printPoint.Invoke(420, 69);
```

_See also:_
[FromAction](#fromaction-and-fromfunc),
[FromFunc](#fromaction-and-fromfunc),
[FromFunctionPointer](#fromfunctionpointer),
[GetFunctionPointer](#getfunctionpointer),
[ToAction](#toaction-and-tofunc),
[ToFunc](#toaction-and-tofunc)

#### ToAction and ToFunc
```C#
Action<T..16> INativeAction<T..16>.ToAction()
Func<T..16, TResult> INativeFunc<T..16, TResult>.ToFunc()
```

Creates a `System.Action` or `System.Func` with the same target and method as the native generic delegate.

_Note:_ The returned delegate does __NOT__ implement the native generic delegate interface. You can convert back to an equivalent
native generic delegate using [FromAction](#fromaction-and-fromfunc) or [FromFunc](#fromaction-and-fromfunc).

_Returns:_ The requested delegate.

_Example:_

```C#
public static void InvokeAction(Action<int, int> action, int x, int y)
{
	action(x, y);
}

var printPoint = INativeAction<int, int>.FromAction
(
	(int x, int y) => Console.WriteLine($"Point {{ X = {x}, Y = {y} }}")
);
InvokeAction(printPoint.ToAction(), 420, 69);
```

_See also:_
[FromAction](#fromaction-and-fromfunc),
[FromFunc](#fromaction-and-fromfunc),
[FromFunctionPointer](#fromfunctionpointer),
[GetFunctionPointer](#getfunctionpointer),
[Invoke](#invoke)
