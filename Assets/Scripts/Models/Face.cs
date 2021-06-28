using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Face
    {
        [JsonProperty("@area")]
        public string Area { get; set; }

        [JsonProperty("@areaname")]
        public string AreaName { get; set; }

        [JsonProperty("@children")]
        public string Children { get; set; }

        [JsonProperty("@designator")]
        public string Designator { get; set; }

        [JsonProperty("@floor")]
        public string Floor { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@measuretype")]
        public string MeasureType { get; set; }

        [JsonProperty("@mode")]
        public string Mode { get; set; }

        [JsonProperty("POLYGON")]
        public Polygon Polygon { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@height")]
        public string Height { get; set; }

        [JsonProperty("@floorindex")]
        public string FloorIndex { get; set; }
    }
}
