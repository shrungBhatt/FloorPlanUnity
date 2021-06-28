using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Location
    {
        [JsonProperty("@address")]
        public string Address { get; set; }

        [JsonProperty("@city")]
        public string City { get; set; }

        [JsonProperty("@lat")]
        public string Latitude { get; set; }

        [JsonProperty("@long")]
        public string Longitude { get; set; }

        [JsonProperty("@postal")]
        public string Postal { get; set; }

        [JsonProperty("@state")]
        public string State { get; set; }
    }
}
