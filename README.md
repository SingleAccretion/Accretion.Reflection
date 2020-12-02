## Accretion.Reflection

A utility library that supports the usage of reflection in .NET Standard 2.0 compliant execution environments with dynamic code generation enabled.

[Is available](https://www.nuget.org/packages/Accretion.Reflection/) in a package form on Nuget.

## Emitting shims for methods with default parameters

A common scenario in reflection is the discovery and invocation of third-party methods (e. g. constructors in DI systems). However, this only works seamlessly when dealing with regular methods that do not have optional parameters, and it is sometimes a requirement to support those. Some of the complexities include:
- `MethodInfo.DefaultValue` is a very tricky API:
  - It returns whatever is in the metadata (`.param [i]` in IL). This means that a parameter of type `Int32` can easily have a default value of type `Byte` if some CLI compiler decides to optimize the assembly size this way.
  - It may return a value defined in a custom attribute (Roslyn compiler uses `DecimalConstantAttribute` and `DateTimeConstantAttribute` to encode constants not directly supported by the CLI).
  - It has a complex relationship with enums where `TEnum` is returned typed as `TEnum` while `TEnum?` is returned as the underlying type.
- Passing default paramters to the desired method means using `System.Reflection.Emit` which is an advanced API with many pitfalls and the requirement of IL knowledge. Different types and values require distinctly different intialization techniques.
- In general, there are many moving parts: `nullref` being the equivalent of `default` (because Roslyn), `IntPtr` and `UIntPtr` being `nint` and `nuint`, pointers, nullable types, `ByRef`s and `ByRefLike`s, `Decimal`s, `DateTime`s, `String`s - it is very easy to forget something and end up with an implementation that works in simple case but falls apart in more complex ones.

`DynamicMethod Accretion.Reflection.Emit.Shim.Create<TTarget>(MethodInfo source)` is meant to address the above problems by hiding the complexity behind a relatively strightforward interface. Here, `TTarget` is the type the delegate - the shim itself, `source` is a method with optional parameters. For example, if you were to invoke a factory method that has some optional parameters:
````C#
using Accretion.Reflection.Emit;

public class Class 
{
    public Class(int x = 20) => X = x;
    public int X { get; }
    public static Class Create(int x = 10) => new Class(x);
}

public static Func<T> CreateFactory<T>()
{
    var sourceFactory = typeof(T).GetMethod("Create");    
    return Shim.Create<Func<T>>(sourceFactory);
}

var factory = CreateFactory<Class>();
Console.WriteLine(factory().X); // Prints "10"
````
This of course works for constructors as well. The library offers two types of shims for these: `Factory` and `Initializer`. The first mimics the semantic of construtors in languages like `C#`, where they are essentially static functions that both allocate and initialize the type:
````C#
var ctor = typeof(Class).GetConstructors().First();
var factory = Shim.Create<Func<Class>>(ctor, ConstructorDelegateKind.Factory);
Console.WriteLine(factory().X); // Prints "20"
````
The second type of shim, `Initializer`, is meant for advanced use cases, where you already have the instance allocated and want to just initialize it:
````C#
var ctor = typeof(Class).GetConstructors().First();
var rawInstance = RuntimeHelpers.GetUninitializedObject(typeof(Class));
var initializer = Shim.Create<Action<Class>>(ctor, ConstructorDelegateKind.Factory);
initializer(rawInstance);
Console.WriteLine(rawInstance.X); // Prints "20"

public struct Struct
{
    public Struct(int x = 30) => X = x;
    public int X { get; }
}

public delegate void RefAction<T>(ref T);

var ctor = typeof(Struct).GetConstructors().First();
var rawInstance = default(Struct);
var initializer = Shim.Create<RefAction<Struct>>(ctor, ConstructorDelegateKind.Factory);
initializer(ref rawInstance);
Console.WriteLine(rawInstance.X); // Prints "30"
````
Notably, shims for instance methods (like `Initializer` constructors) are static - this is to reduce complexity and improve performance, as creating new shims for new instances would be prohibitively expensive. For structs, the first parameter - `this` - has to be passed by reference.

Finally, the library offers a method for consumers who are already doing their own dynamic code emission and are only interested in the handling of default parameters - `Shim.EmitOptionalParameterLoad(ILGenerator il, ParameterInfo parameter)`.
