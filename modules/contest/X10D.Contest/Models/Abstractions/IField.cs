using System;

namespace X10D.Contest.Models.Abstractions
{
    internal interface IField
    {
        string Alias { get; }
        Type Type { get; }
        string Property { get; }
    }
}