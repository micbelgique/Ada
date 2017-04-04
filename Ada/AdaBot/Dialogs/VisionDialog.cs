using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using Microsoft.ProjectOxford.Vision;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class VisionDialog
    {
        VisionServiceClient _visionClient = new VisionServiceClient(ConfigurationManager.AppSettings["VisionApiKey"]);
        Activity _activity;

        public VisionDialog (Activity acti) : base()
        {
            _activity = acti;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);

            return Task.CompletedTask;
        }

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var idUser = context.Activity.From.Id;

            string[] search = new string[2] { "Description", "Tags" };

            try
            {
                var result = await _visionClient.AnalyzeImageAsync(GetStreamFromUrl(_activity.Attachments[0].ContentUrl), search);
                //return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
                await context.PostAsync(result.Description.ToString());
                context.Wait(MessageReceived);
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                //await connector.Conversations.ReplyToActivityAsync(reply);

            }
            catch (Microsoft.ProjectOxford.Vision.ClientException clientException)
            {
                //Trace Ecrit en mode debug ET release
                Debug.WriteLine(clientException.Error.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
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