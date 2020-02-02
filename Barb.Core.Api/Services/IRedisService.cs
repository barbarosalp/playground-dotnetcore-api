using System;
using StackExchange.Redis;

namespace Barb.Core.Api.Services
{
    public interface IRedisService
    {
        void Execute(Action<IDatabase> action);
    }
}