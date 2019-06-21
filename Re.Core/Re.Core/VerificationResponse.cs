using Newtonsoft.Json;

namespace Re.Core
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VerificationResponse
    {
        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
