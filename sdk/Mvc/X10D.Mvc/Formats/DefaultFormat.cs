using X10D.Infrastructure;
using X10D.Mvc.Attributes;

namespace X10D.Mvc.Formats
{
    [Format(Constants.DefaultFormat, Constants.DefaultFormatContentType)]
    public sealed class DefaultFormat : IApiResponse
    {
        public bool ok { get; set; }

        public object result { get; set; }

        public int status_code { get; set; }

        public string description { get; set; }

        public double? request_time { get; set; }
    }
}
