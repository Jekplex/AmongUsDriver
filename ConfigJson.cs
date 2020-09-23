using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongUsDriver
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("token2")]
        public string Token2 { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
