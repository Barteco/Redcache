using Redfish.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redfish.Decorators
{
    internal class RedcacheLoggerService : IRedcache
    {
        private readonly IRedcache _redqueue;
        private readonly IRedfishLogger _logger;

        public RedcacheLoggerService(IRedcache redcache, IRedfishLogger logger)
        {
            _redqueue = redcache;
            _logger = logger;
        }

        public async Task<bool> Exists(string key)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "Exists",
                ["Key"] = key
            });

            try
            {
                return await _redqueue.Exists(key).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task<Optional<T>> Get<T>(string key)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "Get",
                ["Key"] = key
            });

            try
            {
                return await _redqueue.Get<T>(key).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task<T> GetOrSet<T>(string key, Func<T> setter, TimeSpan? expiry = null)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "GetSet",
                ["Key"] = key,
                ["Expiry"] = expiry
            });

            try
            {
                return await _redqueue.GetOrSet(key, setter, expiry).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task<List<T>> GetList<T>(string key, Range? range = null)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "GetList",
                ["Key"] = key,
                ["Range"] = range
            });

            try
            {
                return await _redqueue.GetList<T>(key, range).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task AppendList<T>(string key, T value)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "AppendList",
                ["Key"] = key,
                ["Value"] = value
            });

            try
            {
                await _redqueue.AppendList(key, value).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task AppendList<T>(string key, T[] values)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "AppendList",
                ["Key"] = key,
                ["Values"] = values
            });

            try
            {
                await _redqueue.AppendList(key, values).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "Set",
                ["Key"] = key,
                ["Value"] = value,
                ["Expiry"] = expiry
            });

            try
            {
                await _redqueue.Set(key, value, expiry).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task Delete(string key)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "Delete",
                ["Key"] = key
            });

            try
            {
                await _redqueue.Delete(key).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task DeleteMultiple(string[] keys)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "DeleteMultiple",
                ["Keys"] = keys
            });

            try
            {
                await _redqueue.DeleteMultiple(keys).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task DeleteNamespace(string @namespace)
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "DeleteNamespace",
                ["Namespace"] = @namespace
            });

            try
            {
                await _redqueue.DeleteNamespace(@namespace).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }

        public async Task FlushAll()
        {
            using var scope = _logger.BeginScope(new()
            {
                ["Action"] = "FlushAll"
            });

            try
            {
                await _redqueue.FlushAll().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                scope.Catch(exception);
                throw;
            }
        }
    }
}