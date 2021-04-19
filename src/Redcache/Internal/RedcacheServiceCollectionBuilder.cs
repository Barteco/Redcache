using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;

namespace Redcache.Internal
{
    class RedcacheServiceCollectionBuilder : IRedcacheServiceCollectionBuilder
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        public RedcacheServiceCollectionBuilder(IServiceCollection services)
        {
            Services = (services ?? throw new ArgumentNullException(nameof(services)));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }
    }
}