using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AdaSDK;
using Newtonsoft.Json;
using TokenManagerService = AdaW10.Models.AuthenticationServices.TokenManagerService;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using AdaSDK.Models;
using System.Collections.Generic;

namespace AdaBot.Models
{
    public class DataService
    {
        public static readonly string ApiBasePath = $"{ConfigurationManager.AppSettings["WebAppUrl"]}/api/person";
        public static readonly string PersonsRecognitionQuery = "recognizepersonsPicture";
        public static readonly string EmotionPicture = "EmotionPicture";

        private HttpClient _httpClient;

        public DataService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<FullPersonDto[]> recognizepersonsPictureAsync(Stream picture)
        {
            using (var streamContent = new StreamContent(picture))
            using (var formData = new MultipartFormDataContent())
            {
                // Creates uri from configuration
                var uri = new Uri($"{ApiBasePath}/{PersonsRecognitionQuery}");

                // Adds content to multipart form data 
                formData.Add(streamContent, "file", $"{Guid.NewGuid()}.jpg");

                var resp = await _httpClient.PostAsync(uri, formData);

                if (resp.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<FullPersonDto[]>(await resp.Content.ReadAsStringAsync());
                }

                var error = JsonConvert.DeserializeObject<WebServiceError>(await resp.Content.ReadAsStringAsync());
                Debug.WriteLine($"WebServiceError : {error.HttpStatus} - {error.ErrorCode} : {error.ErrorMessage}");
                return null;
            }
        }

        public async Task<List<EmotionDto>> recognizeEmotion(Stream picture)
        {
            using (var streamContent = new StreamContent(picture))
            using (var formData = new MultipartFormDataContent())
            {
                // Creates uri from configuration
                var uri = new Uri($"{ApiBasePath}/{EmotionPicture}");

                // Adds content to multipart form data 
                formData.Add(streamContent, "file", $"{Guid.NewGuid()}.jpg");

                var resp = await _httpClient.PostAsync(uri, formData);

                //List<EmotionDto> result = null;

                var test = await resp.Content.ReadAsStringAsync();

                if (resp.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<EmotionDto>>(await resp.Content.ReadAsStringAsync());   
                }

                var error = JsonConvert.DeserializeObject<WebServiceError>(await resp.Content.ReadAsStringAsync());
                Debug.WriteLine($"WebServiceError : {error.HttpStatus} - {error.ErrorCode} : {error.ErrorMessage}");
                return null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _httpClient == null) return;

            _httpClient.Dispose();
            _httpClient = null;
        }
    }
}
