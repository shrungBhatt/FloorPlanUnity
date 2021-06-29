using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Lines
    {
        [JsonProperty("LINE")]
        public List<Line> Line { get; set; }
    }

    public class Line
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@path")]
        public string Path { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@angle")]
        public string Angle { get; set; }

        [JsonProperty("@distance")]
        public string Distance { get; set; }
    }
}
