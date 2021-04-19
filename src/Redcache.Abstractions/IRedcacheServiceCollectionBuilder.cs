using Microsoft.Extensions.DependencyInjection;

namespace Redcache
{
    public interface IRedcacheServiceCollectionBuilder
    {
        public IServiceCollection Services { get; }
    }
}