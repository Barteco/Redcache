using Microsoft.Extensions.DependencyInjection;

namespace Redfish
{
    public interface IRedfishServiceCollectionBuilder
    {
        public IServiceCollection Services { get; }
    }
}