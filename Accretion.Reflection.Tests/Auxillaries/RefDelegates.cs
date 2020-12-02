namespace Accretion.Reflection.Tests
{
    internal delegate void RefAction<T>(ref T instance);

    internal delegate T RefFunc<T1, T2, T>(ref T1 instance, T2 firstArgument);
}
