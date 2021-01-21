using CkpWebApi.Infrastructure.Providers.Interfaces;
using System;

namespace CkpWebApi.Infrastructure.Providers
{
    public sealed class FunctionBasedKeyedProvider<TParam, TResult> : IKeyedProvider<TParam, TResult>
    {
        private readonly Func<TParam, TResult> _function;

        public FunctionBasedKeyedProvider(Func<TParam, TResult> function)
        {
            ArgumentValidator.ValidateThatArgumentNotNull(function, "function");
            _function = function;
        }

        public TResult GetByValue(TParam value)
        {
            return _function(value);
        }
    }
}
