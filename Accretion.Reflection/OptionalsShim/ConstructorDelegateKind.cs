namespace Accretion.Reflection.Emit
{
    public enum ConstructorDelegateKind
    {
        /// /// <summary>
        /// This option exposes the constructor as a static method that both allocates and initializes the object.
        /// </summary>
        Factory = 0,
        /// <summary>
        /// This option exposes the constructor as an instance method that initializes the object.
        /// This is the canonical representation of constructors in the Common Type System.
        /// </summary>
        Initializer = 1
    }
}