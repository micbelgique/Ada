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

    }
}