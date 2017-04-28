using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using AdaSDK.Models;

namespace AdaSDK.Services
{
    public class VisionService
    {
        VisionServiceClient visionClient;
      
        public async Task<string> MakeOCRRequest(Stream imageFilePath, string visionApiKey)
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

                for(int i = 0 ; i < result.Regions.Count ; i++)
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