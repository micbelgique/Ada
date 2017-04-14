using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace AdaW10.Models.AuthenticationServices
{
    public static class TokenManagerService
    {
        private static Token _token;
        private const string FileName = "token.txt";
        private static bool _loadded; 

        public static async Task<string> NewToken()
        {
            StringBuilder data = new StringBuilder();
            data.Append("username=");
            data.Append("martineobot");
            data.Append("&password=");
            data.Append("Passw0rd");
            data.Append("&grant_type=password");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("http://adawebapp.azurewebsites.net/token", new StringContent(data.ToString()));
            string content = await response.Content.ReadAsStringAsync();
            _token = JsonConvert.DeserializeObject<Token>(content);
            _token.CreatedTime = DateTime.Now;
            return _token.AccessToken;
        }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string Type { get; set; }

        [JsonProperty("expires_in")]
        public int Duration { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool CheckValidity()
        {
            DateTime date = DateTime.Now;
            var diffInSeconds = (date - CreatedTime).TotalSeconds;
            return !(diffInSeconds >= Duration);
        }
    }
}
