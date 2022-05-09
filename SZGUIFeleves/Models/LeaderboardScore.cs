using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SZGUIFeleves.Models
{
    public class LeaderboardScore
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("seconds")]
        public double Seconds { get; set; }

        [JsonProperty("scenetitle")]
        public string SceneTitle { get; set; }

        public override string ToString()
        {
            return Name + " - " + Date.ToString("d") + " - " + TimeSpan.FromSeconds(Seconds).ToString(@"mm\:ss\.fff");
        }

        [JsonIgnore]
        public string Timespan
        {
            get
            {
                return TimeSpan.FromSeconds(Seconds).ToString(@"mm\:ss\.fff");
            }
        }
    }
}
