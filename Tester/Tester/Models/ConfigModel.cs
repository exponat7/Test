using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tester.Models
{
    class ConfigModel
    {
        public int? MinDelay { get; set; }
        public int? MaxDelay { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? RequestsCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Port { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxWaitForAnswerTimeoutMs { get; set; }
    }
}
