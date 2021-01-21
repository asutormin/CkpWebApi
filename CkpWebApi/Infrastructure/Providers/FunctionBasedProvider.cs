using CkpWebApi.Infrastructure.Providers.Interfaces;
using System;

namespace CkpWebApi.Infrastructure.Providers
{
    public sealed class FunctionBasedProvider<T> : IProvider<T>
    {
        private readonly Func<T> _function;

        public FunctionBasedProvider(Func<T> function)
        {
            ArgumentValidator.ValidateThatArgumentNotNull(function, "function");
            _function = function;
        }

        public T Get()
        {
            return _function();
        }
    }
}
