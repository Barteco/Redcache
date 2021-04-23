using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redfish
{
    public interface IRedcache
    {
        Task<bool> Exists(string key);
        Task<Optional<T>> Get<T>(string key);
        Task<T> GetOrSet<T>(string key, Func<T> setter, DateTime absoluteExpiration);
        Task<T> GetOrSet<T>(string key, Func<T> setter, DateTimeOffset absoluteExpiration);
        Task<T> GetOrSet<T>(string key, Func<T> setter, TimeSpan? slidingExpiration = null);
        Task<List<T>> GetList<T>(string key, Range? range = null);
        Task AppendList<T>(string key, T value);
        Task AppendList<T>(string key, T[] values);
        Task Set<T>(string key, T value, DateTime absoluteExpiration);
        Task Set<T>(string key, T value, DateTimeOffset absoluteExpiration);
        Task Set<T>(string key, T value, TimeSpan? slidingExpiration = null);
        Task Delete(string key);
        Task DeleteMultiple(string[] keys);
        Task DeleteNamespace(string @namespace);
        Task FlushAll();
    }
}