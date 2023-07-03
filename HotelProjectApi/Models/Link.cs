using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HotelProjectApi.Models
{
    public class Link
    {
        public const string GetMethod = "GET";

        public static Link To(string routeName, object routeValues = null)
            => new()
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = null
            };

        [JsonPropertyOrder(-4)]
        public string Href { get; set; }

        [JsonPropertyOrder(-3)]
        [JsonPropertyName("rel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Relations { get; set; }

        [JsonPropertyOrder(-2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(GetMethod)]
        public string Method { get; set; }

        [JsonIgnore]
        public string RouteName { get; set; }

        [JsonIgnore]
        public object RouteValues { get; set; }
    }
}
