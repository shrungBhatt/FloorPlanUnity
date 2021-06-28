using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Points
    {
        [JsonProperty("POINT")]
        public List<Point> Point { get; set; }
    }

    public class Point
    {
        [JsonProperty("@data")]
        public string Data { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }
    }
}
