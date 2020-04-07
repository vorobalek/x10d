using System;
using System.Collections.Generic;
using System.Text;

namespace X10D.Contest.Models.Abstractions
{
    internal interface ISchema
    {
        Type Type { get; }
        IEnumerable<IField> Fields { get; }
    }
}
