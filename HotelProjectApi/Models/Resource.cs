using System.Text.Json.Serialization;

namespace HotelProjectApi.Models
{
    public abstract class Resource : Link
    {
        [JsonPropertyOrder(-2)]
        [JsonIgnore]
        public Link Self { get; set; }
    }
}
