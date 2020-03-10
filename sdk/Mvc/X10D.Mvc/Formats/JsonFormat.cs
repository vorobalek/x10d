using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using X10D.Mvc.Attributes;

namespace X10D.Mvc.Formats
{
    [Format("json")]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public sealed class JsonFormat : IApiResponse
    {
        [JsonRequired]
        [JsonProperty(Order = 0)]
        public bool ok { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 1024, ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public object result { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 2048)]
        public int status_code { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 4096)]
        public string description { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = int.MaxValue)]
        public double? request_time { get; internal set; }

        public StringBuilder Pack()
        {
            return new StringBuilder(JsonConvert.SerializeObject(this));
        }
    }
}
