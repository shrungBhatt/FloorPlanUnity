using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Version
    {
        [JsonProperty("@coplanarity")]
        public string Coplanarity { get; set; }

        [JsonProperty("@dormers")]
        public string Dormers { get; set; }

        [JsonProperty("@precision")]
        public string Precision { get; set; }

        [JsonProperty("@precisionUnits")]
        public string PrecisionUnits { get; set; }

        [JsonProperty("@sourceVersion")]
        public string SourceVersion { get; set; }

        [JsonProperty("@targetVersion")]
        public string TargetVersion { get; set; }

        [JsonProperty("@triangulation")]
        public string Triangulation { get; set; }
    }
}
