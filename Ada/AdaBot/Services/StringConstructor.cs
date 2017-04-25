using AdaBot.Models;
using AdaSDK;
using AdaSDK.Models;
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

            DataService dataService = new DataService();

            VisionService visionService = new VisionService(activity);
            VisualFeature[] visualFeatures = new VisualFeature[] {
                                        VisualFeature.Adult, //recognize adult content
                                        VisualFeature.Categories, //recognize image features
                                        VisualFeature.Description //generate image caption
                                        };
            AnalysisResult analysisResult = null;
            StringBuilder reply = new StringBuilder();

            GoogleTranslatorService translator = new GoogleTranslatorService();
            //If the user uploaded an image, read it, and send it to the Vision API
            if (activity.Attachments.Any() && activity.Attachments.First().ContentType.Contains("image"))
            {
                //stores image url (parsed from attachment or message)
                string uploadedImageUrl = activity.Attachments.First().ContentUrl;
                string OCR;
                StringConstructor stringConstructor = new StringConstructor();
                using (Stream imageFileStream = GetStreamFromUrl(uploadedImageUrl))
                {
                    analysisResult = await visionClient.AnalyzeImageAsync(imageFileStream, visualFeatures);

                    imageFileStream.Seek(0, SeekOrigin.Begin);

                    OCR = await visionService.MakeOCRRequest(imageFileStream);
                    reply.Append(translator.TranslateText(analysisResult.Description.Captions[0].Text.ToString(), "en|fr") + ". ");

                    if (analysisResult.Description.Tags.Contains("person"))
                    {
                        imageFileStream.Seek(0, SeekOrigin.Begin);

                        FullPersonDto[] persons = await dataService.recognizepersonsPictureAsync(imageFileStream);

                        if (persons != null)
                        {
                            reply.Append("je vois : " + persons.Count() + " personne(s) sur la photo.");


                            foreach (FullPersonDto result in persons)
                            {
                                reply.Append(stringConstructor.DescriptionPersonImage(result));
                            }
                        }
                    }

                    reply.Append(" Et il me semble qu'il y a du texte sur la photo, le voici : ");
                    reply.Append(OCR);
                }
            }

            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(reply.ToString()));
        }

        public StringBuilder DescriptionPersonImage(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (person.FirstName != null)
            {
                reply.Append(" Je connais cette personne c'est " + person.FirstName + ". ");
            }
            else
            {
                reply.Append(" Je ne connais malheureusement pas cette personne. ");
            }

            reply.Append(DescriptionGender(person));

            if (Convert.ToInt32(person.Glasses) != 0)
            {
                reply.Append(DescriptionGlasses(person));
            }

            if (Convert.ToString(person.Gender) == "male")
            {
                reply.Append(DescriptionEmotion(person));
            }
            else if(Convert.ToString(person.Gender) == "female")
            {
                reply.Append(DescriptionEmotionFemale(person));
            }


            reply.Append(".");
            return reply;
        }

        public StringBuilder DescriptionGender(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (Convert.ToString(person.Gender) == "male")
            {
                reply.Append("C'est un homme");

                if(person.Beard >= 0.5 && person.Mustache >= 0.5)
                {
                    reply.Append(" barbu et moustachu");
                }
                else if(person.Beard >= 0.5)
                {
                    reply.Append(" barbu");
                }
                else if(person.Mustache >= 0.5)
                {
                    reply.Append(" moustachu");
                }

                reply.Append(" d'environ " + (int)person.Age + " ans ");
            }
            else
            {
                reply.Append("C'est une femme d'environ " + (int)person.Age + " ans ");
            }

            return reply;
        }

        public StringBuilder DescriptionGlasses(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (Convert.ToInt32(person.Glasses) == 1)
            {
                reply.Append("qui porte des lunettes de soleil ");
            }
            else if (Convert.ToInt32(person.Glasses) == 2)
            {
                reply.Append("qui porte des lunettes ");
            }
            else
            {
                reply.Append("qui porte des lunettes de piscine ");
            }

            return reply;
        }
        public StringBuilder DescriptionEmotion(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if(person.Happiness >= 0.75)
            {
                reply.Append("et qui est heureux");
            }
            else if (person.Neutral >= 0.75)
            {
                reply.Append("et qui est neutre");
            }
            else if(person.Sadness >= 0.75)
            {
                reply.Append("et qui est triste");
            }
            else if(person.Surprise >= 0.75)
            {
                reply.Append("et qui est surpris");
            }
            else if(person.Anger >= 0.75)
            {
                reply.Append("et qui est en colère");
            }
            else if(person.Contempt >= 0.75)
            {
                reply.Append("et qui est méprisant");
            }
            else if(person.Disgust >= 0.75)
            {
                reply.Append("et qui est dégouté");
            }
            else if(person.Fear >=0.75)
            {
                reply.Append("et qui a peur");
            }

            return reply;
        }

        public StringBuilder DescriptionEmotionFemale(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (person.Happiness >= 0.75)
            {
                reply.Append("et qui est heureuse");
            }
            else if (person.Neutral >= 0.75)
            {
                reply.Append("et qui est neutre");
            }
            else if (person.Sadness >= 0.75)
            {
                reply.Append("et qui est triste");
            }
            else if (person.Surprise >= 0.75)
            {
                reply.Append("et qui est surprise");
            }
            else if (person.Anger >= 0.75)
            {
                reply.Append("et qui est en colère");
            }
            else if (person.Contempt >= 0.75)
            {
                reply.Append("et qui est méprisante");
            }
            else if (person.Disgust >= 0.75)
            {
                reply.Append("et qui est dégoutée");
            }
            else if (person.Fear >= 0.75)
            {
                reply.Append("et qui a peur");
            }

            return reply;
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