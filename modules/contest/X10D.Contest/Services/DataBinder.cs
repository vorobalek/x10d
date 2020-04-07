using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Runtime.Serialization;
using X10D.Contest.Models.Abstractions;
using X10D.Infrastructure;

namespace X10D.Contest.Services
{
    internal sealed class DataBinder : ServicePrototype<IDataBinder>, IDataBinder
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;
        IDataBinderCache Cache { get; }
        ILogger Logger { get; }
        ISchemaProvider SchemaProvider { get; }
        public DataBinder(IDataBinderCache cache, ILogger<IDataBinder> logger, ISchemaProvider schemaProvider)
        {
            Cache = cache;
            Logger = logger;
            SchemaProvider = schemaProvider;
        }
        public TModel Bind<TModel>(NpgsqlDataReader reader) where TModel : class
        {
            var type = typeof(TModel);
            var schema = Cache[type];
            if (schema == null)
            {
                Logger.LogWarning($"Схема для типа {type.Name} будет построена впервые.");
                schema = SchemaProvider.Build(type);
            }
            var model = Bind(reader, schema);
            if (model is TModel tModel)
            {
                Cache.SaveSchema(schema);
                return tModel;
            }
            throw new InvalidCastException($"Схема для типа {type.Name} не найдена или некорректна.");
        }
        private object Bind(NpgsqlDataReader reader, ISchema schema)
        {
            var model = FormatterServices.GetUninitializedObject(schema.Type);
            foreach (var field in schema.Fields)
            {
                var value = reader[field.Alias];
                schema.Type.GetProperty(field.Property).SetValue(model, value is DBNull ? null : value);
            }
            return model;
        }
    }
}
