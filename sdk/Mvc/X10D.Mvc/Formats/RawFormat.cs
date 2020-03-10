using System.Text;
using X10D.Mvc.Attributes;

namespace X10D.Mvc.Formats
{
    [Format("raw")]
    public sealed class RawFormat : IApiResponse
    {
        public bool ok { get; internal set; }

        public object result { get; internal set; }

        public int status_code { get; internal set; }

        public string description { get; internal set; }

        public double? request_time { get; internal set; }

        public StringBuilder Pack()
        {
            var stringBuilder = new StringBuilder();

            foreach (var prop in GetType().GetProperties())
            {
                stringBuilder.AppendLine($"{prop.Name}:\t{prop.GetValue(this)}");
            }

            return stringBuilder;
        }
    }
}
