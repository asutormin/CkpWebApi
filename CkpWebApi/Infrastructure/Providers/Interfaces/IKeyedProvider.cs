﻿namespace CkpWebApi.Infrastructure.Providers.Interfaces
{
    public interface IKeyedProvider<in TParam, out TResult>
    {
        TResult GetByValue(TParam value);
    }
}
