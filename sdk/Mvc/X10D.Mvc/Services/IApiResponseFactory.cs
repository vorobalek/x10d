using System;
using System.Collections.Generic;

namespace X10D.Mvc.Services
{
    public interface IApiResponseFactory
    {
        public Type GetFormatType(string format);

        IReadOnlyList<string> Formats { get; }
    }
}