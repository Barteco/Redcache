using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redfish
{
    public interface IRedcache
    {
        Task<bool> Exists(string key);
        Task<Optional<T>> Get<T>(string key, TimeSpan? expiry = null);
        Task<T> Get<T>(string key, Func<T> setter, TimeSpan? expiry = null);
        Task<List<T>> GetList<T>(string key, Range? range = null);
        Task AppendList<T>(string key, T value);
        Task AppendList<T>(string key, T[] values);
        Task Set<T>(string key, T value, TimeSpan? expiry = null);
        Task Delete(string key);
        Task DeleteMultiple(string[] keys);
        Task DeleteNamespace(string @namespace);
        Task FlushAll();
    }
}