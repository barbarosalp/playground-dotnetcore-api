using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Barb.Core.Api.Services
{
    public interface IRedisService
    {
        void Execute(Action<IDatabase> action);
        Task ExecuteAsync(Func<IDatabase, Task> action);
    }
}