using System;
using X10D.Contest.Models.Abstractions;

namespace X10D.Contest.Services
{
    internal interface IDataBinderCache
    {
        ISchema this[Type type] { get; }
        void SaveSchema(ISchema schema);
    }
}