using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class FloorPlan
    {
        [JsonProperty("EAGLEVIEW_EXPORT")]
        public EagleViewExport EagleViewExport { get; set; }
    }
}
