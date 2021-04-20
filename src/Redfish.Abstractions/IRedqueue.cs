using System;
using System.Threading.Tasks;

namespace Redfish
{
    public interface IRedqueue
    {
        Task Publish<T>(string channel, T message);
        Task Subscribe<T>(string channel, Action<T> handler);
        Task Unsubscribe(string channel);
    }
}