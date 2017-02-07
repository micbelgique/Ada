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

        public static async Task<Token> GetToken()
        {
            //if token wasn't loadded
            if (!_loadded)
            {
                LoadToken();
                _loadded = true;
            }

            if (_token == null || !_token.CheckValidity())
                await NewToken();
            
            return _token;
        }

        public static async Task NewToken()
        {
            StringBuilder data = new StringBuilder();
            data.Append("username=");
            data.Append(AppConfig.UserName);
            data.Append("&password=");
            data.Append(AppConfig.Password);
            data.Append("&grant_type=password");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(AppConfig.WebUri + "/token", new StringContent(data.ToString()));
            string content = await response.Content.ReadAsStringAsync();
            _token = JsonConvert.DeserializeObject<Token>(content);
            _token.CreatedTime = DateTime.Now;
            await SaveToken();
        }

        private static async Task SaveToken()
        {
            string serialized = JsonConvert.SerializeObject(_token);

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            Debug.WriteLine("DEBUG FOLDER: " + storageFolder.Path);

            StorageFile sampleFile = await storageFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, serialized);
        }

        public static async void LoadToken()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            if (await storageFolder.TryGetItemAsync(FileName) != null)
            {
                StorageFile sampleFile = await storageFolder.GetFileAsync(FileName);
                string textResult = await FileIO.ReadTextAsync(sampleFile);
                _token = JsonConvert.DeserializeObject<Token>(textResult);
            }
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
