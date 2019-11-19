using Newtonsoft.Json;

namespace AutoNitro
{
    public class Config
    {
        [JsonProperty("auth_token")]
        public string Token { get; set; }
    }
}
