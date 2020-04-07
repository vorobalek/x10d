using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using X10D.Contest.Models.Abstractions;
using X10D.Infrastructure;

namespace X10D.Contest.Services
{
    internal sealed class DataBinderCache : ServicePrototype<IDataBinderCache>, IDataBinderCache
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
        private ConcurrentDictionary<Type, ISchema> Cache { get; } = new ConcurrentDictionary<Type, ISchema>();
        public ISchema this[Type type]
        {
            get
            {
                if (Cache.ContainsKey(type) && Cache.TryGetValue(type, out var schema))
                {
                    return schema;
                }
                else
                {
                    return null;
                }
            }
        }
        public void SaveSchema(ISchema schema)
        {
            if (!Cache.ContainsKey(schema.Type))
            {
                Cache.TryAdd(schema.Type, schema);
            }
        }
    }
}
