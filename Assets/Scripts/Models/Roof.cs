using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Roof
    {
        [JsonProperty("FACES")]
        public Faces Faces { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("LINES")]
        public Lines Lines { get; set; }

        [JsonProperty("POINTS")]
        public Points Points { get; set; }

    }
}
