using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using X10D.Contest.Attributes;
using X10D.Contest.Models.Abstractions;
using X10D.Infrastructure;

namespace X10D.Contest.Services
{
    internal sealed class SchemaProvider : ServicePrototype<ISchemaProvider>, ISchemaProvider
    {
        public override ServiceLifetime ServiceLifetime => ServiceLifetime.Transient;

        private class Schema : ISchema
        {
            public Type Type { get; set; }
            public IEnumerable<IField> Fields { get; set; }
        }

        private class Field : IField
        {
            public string Alias { get; set; }

            public Type Type { get; set; }

            public string Property { get; set; }
        }

        public ISchema Build(Type type)
        {
            var schema = new Schema
            {
                Type = type
            };

            var tuples = type
                .GetProperties()
                .Select(property => new ValueTuple<PropertyInfo, FieldAttribute>(property, property.GetCustomAttribute<FieldAttribute>()))
                .Where(tuple => tuple.Item2 != null)
                .ToArray();

            var fields = new List<IField>(tuples.Length);

            foreach (var tuple in tuples)
            {
                fields.Add(new Field
                {
                    Alias = tuple.Item2.Name,
                    Type = tuple.Item1.PropertyType,
                    Property = tuple.Item1.Name
                });
            }

            schema.Fields = fields;

            return schema;
        }
    }
}
