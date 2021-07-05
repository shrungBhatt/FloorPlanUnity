using Assets.Scripts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class FloorPlan01
    {
        [JsonProperty("NewRoomId")]
        public string NewRoomId { get; set; }

        [JsonProperty("FloorPlan")]
        public FloorPlan FloorPlan { get; set; }
    }
}
