using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class EagleViewExport
    {
        [JsonProperty("LOCATION")]
        public Location Location { get; set; }

        [JsonProperty("REPORT")]
        public Report Report { get; set; }

        [JsonProperty("STRUCTURES")]
        public Structures Structures { get; set; }

        [JsonProperty("VERSION")]
        public Version Version { get; set; }
    }
}
