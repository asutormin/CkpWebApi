using System;

namespace CkpInfrastructure
{
    public static class ArgumentValidator
    {
        public static void ValidateThatArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }

        public static void ValidateThatArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue))
                throw new ArgumentNullException(argumentName);
        }

        public static void ValidateThatArgumentNotEqualZero(int argumentValue, string argumentName)
        {
            if (argumentValue == 0)
                throw new ArgumentException(string.Format("Аргумент {0} не может быть равен 0", argumentName));
        }
    }
}