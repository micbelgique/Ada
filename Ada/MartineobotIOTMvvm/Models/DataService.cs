using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MartineobotBridge;
using Newtonsoft.Json;
using TokenManagerService = MartineobotIOTMvvm.Models.AuthenticationServices.TokenManagerService;

namespace MartineobotIOTMvvm.Models
{
    public class DataService : Interfaces.IDataService
    {

#if DEBUG
        public static readonly string ApiBasePath = AppConfig.WebUri + "/api/person";
#else
        public static readonly string ApiBasePath = AppConfig.WebUri+"/api/person";
#endif

        public static readonly string PersonsRecognitionQuery = "recognizepersons";

        public static readonly string PersonUpdateInformationQuery = "updatepersoninformation";

        private HttpClient _httpClient;

        public DataService()
        {
            _httpClient = new HttpClient(); 
        }

        public async Task<PersonDto[]> RecognizePersonsAsync(Stream picture)
        {
            using (var streamContent = new StreamContent(picture))
            using (var formData = new MultipartFormDataContent())
            {
                // Creates uri from configuration
                var uri = new Uri($"{ApiBasePath}/{PersonsRecognitionQuery}");

                // Adds content to multipart form data 
                formData.Add(streamContent, "file", $"{Guid.NewGuid()}.jpg");

                // Get authorization token 
                string token = (await TokenManagerService.GetToken()).AccessToken;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage resp = await _httpClient.PostAsync(uri, formData);

                if (resp.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<PersonDto[]>(await resp.Content.ReadAsStringAsync());

                var error = JsonConvert.DeserializeObject<WebServiceError>(await resp.Content.ReadAsStringAsync());
                Debug.WriteLine($"WebServiceError : {error.HttpStatus} - {error.ErrorCode} : {error.ErrorMessage}");
                return null;
            }
        }

        public async Task UpdatePersonInformation(PersonUpdateDto personUpdateDto)
        {
            // Creates uri from configuration 
            var uri = new Uri($"{ApiBasePath}/{PersonUpdateInformationQuery}");
            Debug.WriteLine("Debug: " + uri);

            // Get authorization token 
            string token = (await TokenManagerService.GetToken()).AccessToken;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(personUpdateDto), Encoding.UTF8, "application/json");

            HttpResponseMessage resp = await _httpClient.PostAsync(uri, content);

            if (!resp.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<WebServiceError>(await resp.Content.ReadAsStringAsync());
                Debug.WriteLine($"WebServiceError : {error.HttpStatus} - {error.ErrorCode} : {error.ErrorMessage}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _httpClient == null) return;

            _httpClient.Dispose();
            _httpClient = null;
        }
    }
}
