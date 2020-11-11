using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmongUsDriver
{
    public struct ConfigJson
    {
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("token")]
        public string Token { get; private set; }
    }
}
