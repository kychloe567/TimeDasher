﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        const string binId = "6277cf50019db4679697a695";

        static HttpClient client = new HttpClient();

        public static void Run()
        {
            client.BaseAddress = new Uri("https://api.jsonbin.io/v3/b/");
            client.DefaultRequestHeaders.Accept.Clear();

            // Tells the server to send data in JSON format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Master-Key", masterkey);

            //DeleteAll();
        }

        public static void DeleteAll()
        {
            List<LeaderboardScore> helperList = new List<LeaderboardScore>()
            {
                new LeaderboardScore() { Date = DateTime.Now, Name="Teszt", Seconds = 1}
            };

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.PutAsJsonAsync("https://api.jsonbin.io/v3/b/" + binId, helperList).Result;
            }
            catch { return; }
        }

        public static Dictionary<string, List<LeaderboardScore>> GetAll()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.GetAsync(binId).Result;
            }
            catch
            {
                return new Dictionary<string, List<LeaderboardScore>>();
            }

            Dictionary<string, List<LeaderboardScore>> scores = new Dictionary<string, List<LeaderboardScore>>();

            if (response.IsSuccessStatusCode)
            {
                JObject parsed = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                IList<JToken> tokened = parsed["record"].ToList();

                foreach (JToken item in tokened)
                {
                    LeaderboardScore one = item.ToObject<LeaderboardScore>();

                    if (one.SceneTitle is null || one.SceneTitle == "")
                        continue;

                    //TODO Custom scenes
                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\" + one.SceneTitle + ".json"))
                        continue;

                    if (!scores.ContainsKey(one.SceneTitle))
                        scores[one.SceneTitle] = new List<LeaderboardScore>();

                    scores[one.SceneTitle].Add(one);
                }
            }

            return scores;
        }

        public static Dictionary<string, LeaderboardScore> GetAll(string username)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.GetAsync(binId).Result;
            }
            catch
            {
                return new Dictionary<string, LeaderboardScore>();
            }

            Dictionary<string, List<LeaderboardScore>> scores = new Dictionary<string, List<LeaderboardScore>>();

            if (response.IsSuccessStatusCode)
            {
                JObject parsed = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                IList<JToken> tokened = parsed["record"].ToList();

                foreach (JToken item in tokened)
                {
                    LeaderboardScore one = item.ToObject<LeaderboardScore>();

                    if (one.SceneTitle is null || one.SceneTitle == "")
                        continue;

                    if (one.Name != username)
                        continue;

                    //TODO Custom scenes
                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\Scenes\\" + one.SceneTitle + ".json"))
                        continue;

                    if (!scores.ContainsKey(one.SceneTitle))
                        scores[one.SceneTitle] = new List<LeaderboardScore>();

                    scores[one.SceneTitle].Add(one);
                }
            }

            Dictionary<string, LeaderboardScore> bestScores = new Dictionary<string, LeaderboardScore>();
            foreach(KeyValuePair<string, List<LeaderboardScore>> scenescores in scores)
            {
                LeaderboardScore scenescore = scenescores.Value.OrderBy(x => x.Seconds).First();
                bestScores.Add(scenescore.SceneTitle, scenescore);
            }

            return bestScores;
        }

        public static void AddNew(LeaderboardScore itemToAdd)
        {
            List<LeaderboardScore> helperList = new List<LeaderboardScore>();
            foreach(List<LeaderboardScore> ls in GetAll().Values)
            {
                helperList.AddRange(ls);
            }
            
            helperList.Add(itemToAdd);

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = client.PutAsJsonAsync("https://api.jsonbin.io/v3/b/" + binId, helperList).Result;
            }
            catch { return; }
        }
    }
}
