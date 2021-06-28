using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class Report
    {
        [JsonProperty("@claimId")]
        public string ClaimId { get; set; }

        [JsonProperty("@reportId")]
        public string ReportId { get; set; }
    }
}
