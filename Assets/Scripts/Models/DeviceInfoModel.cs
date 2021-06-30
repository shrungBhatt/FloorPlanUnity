using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class DeviceInfoModel
    {
        [JsonProperty("screenWidth")]
        public int ScreenWidth { get; set; }

        [JsonProperty("screenHeight")]
        public int ScreenHeight { get; set; }

        [JsonProperty("zoomFactor")]
        public int ZoomFactor { get; set; }
    }
}
