using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using X10D.Mvc.Attributes;

namespace X10D.Mvc.Formats
{
    [Format("json", "application/json")]
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public sealed class JsonFormat : IApiResponse
    {
        [JsonRequired]
        [JsonProperty(Order = 0)]
        public bool ok { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 1024, ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public object result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 2048)]
        public int status_code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 4096)]
        public string description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = int.MaxValue)]
        public double? request_time { get; set; }
    }
}
