using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System.Web;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using AdaBot.Models;
using System.Text;

namespace AdaBot.Services
{
    public class VisionService
    {
        Activity activity;
        string visionApiKey;
        VisionServiceClient visionClient;
        public VisionService (Activity acti)
        {
            activity = acti;
            visionApiKey = ConfigurationManager.AppSettings["VisionApiKey"];

            //Vision SDK classes
            visionClient = new VisionServiceClient(visionApiKey);
        }

        public async Task<AnalysisResult> CallVisionApiAsync()
        {
            VisualFeature[] visualFeatures = new VisualFeature[] {
                                        VisualFeature.Adult, //recognize adult content
                                        VisualFeature.Categories, //recognize image features
                                        VisualFeature.Description //generate image caption
                                        };
            AnalysisResult analysisResult = null;

            if (activity == null || activity.GetActivityType() != ActivityTypes.Message)
            {
                //add code to handle errors, or non-messaging activities
            }

            //If the user uploaded an image, read it, and send it to the Vision API
            if (activity.Attachments.Any() && activity.Attachments.First().ContentType.Contains("image"))
            {
                //stores image url (parsed from attachment or message)
                string uploadedImageUrl = activity.Attachments.First().ContentUrl; ;
                //uploadedImageUrl = HttpUtility.UrlDecode(uploadedImageUrl.Substring(uploadedImageUrl.IndexOf("file=") + 5));

                using (Stream imageFileStream = GetStreamFromUrl(uploadedImageUrl))
                {
                    try
                    {
                        analysisResult = await visionClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                    }
                    catch (Exception e)
                    {
                        analysisResult = null; //on error, reset analysis result to null
                    }
                }
            }
            //Else, if the user did not upload an image, determine if the message contains a url, and send it to the Vision API

            return analysisResult;

        }
        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        public async Task<string> MakeOCRRequest(Stream imageFilePath)
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", visionApiKey);

            // Request parameters and URI
            string requestParameters = "language=unk&detectOrientation =true";
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.

            byte[] byteData = ReadFully(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                var resultContent = await response.Content.ReadAsStringAsync();
                OCRModel result = JsonConvert.DeserializeObject<OCRModel>(resultContent);
                StringBuilder stringResult = new StringBuilder();

                for(int i = 0 ; i < result.Regions.Count; i++)
                {
                    for (int y = 0; y < result.Regions[i].Lines.Count; y++)
                    {
                        for (int z = 0; z < result.Regions[i].Lines[y].Words.Count; z++)
                        {
                            stringResult.Append(result.Regions[i].Lines[y].Words[z].Text);
                            stringResult.Append(" ");
                        }
                    }
                }

                return stringResult.ToString(); ;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}