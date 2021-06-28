using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Faces
    {
        [JsonProperty("FACE")]
        public List<Face> Face { get; set; }
    }
}
