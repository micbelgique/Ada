using AdaBot.Models;
using AdaSDK;
using AdaSDK.Models;
using AdaSDK.Services;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdaBot.Services
{
    public class StringConstructor
    {

        public async Task PictureAnalyseAsync(Activity activity)
        {
            VisionServiceClient visionClient;

            string visionApiKey;
            visionApiKey = ConfigurationManager.AppSettings["VisionApiKey"];

            visionClient = new VisionServiceClient(visionApiKey);

            StringBuilder reply = new StringBuilder();

            //If the user uploaded an image, read it, and send it to the Vision API
            if (activity.Attachments.Any() && activity.Attachments.First().ContentType.Contains("image"))
            {
                //stores image url (parsed from attachment or message)
                string uploadedImageUrl = activity.Attachments.First().ContentUrl;
                StringConstructor stringConstructor = new StringConstructor();

                using (Stream imageFileStream = GetStreamFromUrl(uploadedImageUrl))
                {
                    StringConstructorSDK client = new StringConstructorSDK() { WebAppUrl = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }" };
                    reply.Append(await client.PictureAnalyseAsync(visionApiKey, imageFileStream));
                }
            }

            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            string[] stringReturn = reply.ToString().Split('|');

            for (int i=0; i<stringReturn.Length; i++)
            {
                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(stringReturn[i].ToString()));
            }

            //await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(reply.ToString()));
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