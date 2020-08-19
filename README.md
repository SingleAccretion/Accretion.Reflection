## Accretion.Reflection

A utility library that supports the usage of reflection in .NET Standard 2.0 compliant execution environments with dynamic code generation enabled.

[Is available](https://www.nuget.org/packages/Accretion.Reflection/) in a package form on Nuget.

## Emitting shims for methods with default parameters

A common scenario in reflection is the discovery and invocation of third-party methods (e. g. constructors in DI systems). However, this only works seamlessly when dealing with regular methods that do not have optional parameters, and it is sometimes a requirement to support those. Some of the complexities include:
- `MethodInfo.DefaultValue` is a very tricky API:
  - It returns whatever is in the metadata (`.param [i]` in IL). This means that a parameter of type `Int32` can easily have a default value of `Byte` if some CLI compiler decides to optimize the assembly size this way.
  - It may return a value defined in a custom attribute (Roslyn compiler uses `DecimalConstantAttribute` and `DateTimeConstantAttribute` to encode constants not directly supported by the CLI).
  - It has a complex relationship with enums where `TEnum` is returned typed as `TEnum` while `TEnum?` is returned as the underlying type.
- Passing default paramters to the desired method means using `System.Reflection.Emit` which is an advanced API with many pitfalls and the requirement of IL knowledge (for this reason, `Accretion.Reflection` uses [`GroboIL`](https://github.com/skbkontur/gremit), a wrapper around `ILGenerator` that features stack validation and many other niceties). Different types and values require distinctly different intialization techniques.
- In general, there are many moving parts: `nullref` being the equivalent of `default` (because Roslyn), `IntPtr` and `UIntPtr` being `nint` and `nuint`, pointers, nullable types, `ByRef`s and `ByRefLike`s, strings - it is very easy to forget something and end up with an implementation that works in simple case but falls apart in more complex ones.

`DynamicMethod Accretion.Reflection.Emit.Shim.Create(MethodInfo target, MethodInfo source, Emitter emitter)` is meant to address the above problems by hiding the complexity behind a relatively strightforward interface. Here, `target` is a method which you would eventually turn into a delegate - the shim itself, `source` is a method with default parameters and `emitter` is a delegate-like class that emits the parameters. The default implementation is the `Emitter.Default` singleton, which emit the loads to the `target`'s parameters when a `source`'s parameter has no default value and loads `parameter.DefaultValue` on the stack otherwise. For example, if you were to invoke a factory method that has some trailing default parameters:
````C#
using Accretion.Reflection.Emit;

public class Class 
{
    private Class(int x) => X = x;

    public int X { get; }

    public static Class Create(int x = 10) => new Class(x);
}

public static Func<T> CreateFactory<T>()
{
    var sourceFactory = typeof(T).GetMethod("Create");
    var target = typeof(Func<T>).GetMethod("Invoke");
    var shim = Shim.Create(target, sourceFactory, Emitter.Default);
    
    return (Func<T>)shim.CreateDelegate(typeof(Func<T>));
}

var factory = CreateFactory<Class>();
var newClass = factory.Invoke(); 
Console.WriteLine(newClass.X); // Prints "10"
````
While the default emitter should work for all C# current code, it is not suitable for methods with required parameters mixed in with __non-trailing__ optional ones. For the purposes of suporting custom encodings and other possible customizations, `Emitter.EmitParameterLoad` is `virtual` - you can create your own emitter and, if needed, reuse all the complex logic in the base class with `base.EmitParameterLoad`.
