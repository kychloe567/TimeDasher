using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Logic;
using SZGUIFeleves.Models;

namespace SZGUIFeleves.Controller
{
    internal class LeaderboardController
    {
        // READ allowed ONLY with this key
        const string masterkey = "$2b$10$s/e8i//Enfog.N2UHxFVDenE2MJtNujAkmzy.qAXTo0rqcwMgo6jW";
        const string binId = "625dc6cf80883c3054e31eb2";

        static HttpClient client = new HttpClient();

        public static void Run()
        {
            client.BaseAddress = new Uri("https://api.jsonbin.io/v3/b/");
            client.DefaultRequestHeaders.Accept.Clear();

            // Tells the server to send data in JSON format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Master-Key", masterkey);
        }

        public static IList<LeaderboardScore> GetAll(string path)
        {
            HttpResponseMessage response = client.GetAsync(path).Result;
            IList<LeaderboardScore> output = new List<LeaderboardScore>();

            if (response.IsSuccessStatusCode)
            {
                JObject parsed = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                IList<JToken> tokened = parsed["record"]["records"].Children().ToList();

                foreach (JToken item in tokened)
                {
                    LeaderboardScore one = item.ToObject<LeaderboardScore>();
                    output.Add(one);
                }
            }
            return output;
        }

        public static void AddNew(LeaderboardScore itemToAdd)
        {
            IList<LeaderboardScore> helperList = GetAll(binId);
            helperList.Add(itemToAdd);

            HttpResponseMessage response = client.PutAsJsonAsync("https://api.jsonbin.io/v3/b/" + binId, helperList).Result;
        }
    }
}
