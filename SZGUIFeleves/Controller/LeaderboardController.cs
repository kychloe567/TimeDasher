using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SZGUIFeleves.Logic;

namespace SZGUIFeleves.Controller
{
    internal class LeaderboardController
    {
        // READ allowed ONLY with this key
        const string accesskey = "$2b$10$Z.CNtFQIJdeIz2JImbT4sue8jU/Nptr/LUNzdcRti37iOtTaB83A2";
        const string binId = "625dc6cf80883c3054e31eb2";

        static HttpClient client = new HttpClient();

        static void Run()
        {
            client.BaseAddress = new Uri("https://api.jsonbin.io/v3/b/");
            client.DefaultRequestHeaders.Accept.Clear();

            // Tells the server to send data in JSON format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Access-Key", accesskey);
        }

        static string GetAll(string path)
        {
            string output = string.Empty;
            HttpResponseMessage response = client.GetAsync(path).Result;

            if (response.IsSuccessStatusCode)
            {
                output = response.Content.ReadAsStringAsync().Result;
            }
            return output;
        }
    }
}
