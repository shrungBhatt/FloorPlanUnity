using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Polygon
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@orientation")]
        public string Orientation { get; set; }

        [JsonProperty("@path")]
        public string Path { get; set; }

        [JsonProperty("@pitch")]
        public string Pitch { get; set; }

        [JsonProperty("@size")]
        public string Size { get; set; }

        [JsonProperty("@unroundedsize")]
        public string UnroundedSize { get; set; }
    }
}
