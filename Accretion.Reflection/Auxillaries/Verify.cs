using System;
using System.ComponentModel;

namespace Accretion.Reflection
{
    internal static class Verify
    {
        public static void IsNotNull(object parameter, string parameterName)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void IsNotDefault<T>(T parameter, string parameterName)
        {
            if (Equals(parameter, default(T)))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void IsValidEnum<TEnum>(TEnum enumeration, string parameterName) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), enumeration))
            {
                throw new InvalidEnumArgumentException(parameterName, Convert.ToInt32(enumeration), typeof(TEnum));
            }
        }

        public static void IsTrue(bool condition, string message, string parameterName)
        {
            if (!condition)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new ArgumentException(message);
            }
        }
    }
}
