using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Structures
    {
        [JsonProperty("@northorientation")]
        public string NorthOrientation { get; set; }

        [JsonProperty("@interiorWallThickness")]
        public string InteriorWallThickness { get; set; }

        [JsonProperty("@exteriorWallThickness")]
        public string ExteriorWallThickness { get; set; }

        [JsonProperty("ROOF")]
        public Roof Roof { get; set; }
    }
}
