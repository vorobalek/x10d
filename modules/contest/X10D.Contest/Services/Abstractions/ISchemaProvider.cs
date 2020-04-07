using System;
using X10D.Contest.Models.Abstractions;

namespace X10D.Contest.Services
{
    internal interface ISchemaProvider
    {
        ISchema Build(Type type);
    }
}