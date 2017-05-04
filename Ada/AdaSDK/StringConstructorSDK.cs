using AdaSDK.Models;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdaSDK.Services
{
    public class StringConstructorSDK
    {
        public static readonly string ApiBasePath = $"/api/person";
        public static readonly string PersonsRecognitionQuery = "recognizepersonsPicture";
        public static readonly string EmotionPicture = "EmotionPicture";

        public HttpClient HttpClient { get; set; }
        public string WebAppUrl { get; set; }

        public StringConstructorSDK()
        {
            HttpClient = new HttpClient();
            HttpClient.MaxResponseContentBufferSize = 256000;
        }

        public async Task<string> PictureAnalyseAsync(string visionApiKey, Stream imageFileStream)
        {
            VisionServiceClient visionClient = new VisionServiceClient(visionApiKey);
            VisualFeature[] visualFeatures = new VisualFeature[] {
                                        VisualFeature.Adult, //recognize adult content
                                        VisualFeature.Categories, //recognize image features
                                        VisualFeature.Description //generate image caption
                                        };
            AnalysisResult analysisResult = null;
            string OCR = "NULL";
            VisionService visionService = new VisionService();
            StringBuilder reply = new StringBuilder();
            GoogleTranslatorServiceSDK translator = new GoogleTranslatorServiceSDK();


            analysisResult = await visionClient.AnalyzeImageAsync(imageFileStream, visualFeatures);

            imageFileStream.Seek(0, SeekOrigin.Begin);

            OCR = await visionService.MakeOCRRequest(imageFileStream, visionApiKey);
            string translate = await translator.TranslateText(analysisResult.Description.Captions[0].Text.ToString(), "en|fr");
            reply.Append("Je vois " + translate.ToLower() + ". ");

            if (analysisResult.Description.Tags.Contains("person"))
            {
                imageFileStream.Seek(0, SeekOrigin.Begin);

                FullPersonDto[] persons = await this.recognizepersonsPictureAsync(imageFileStream);
                bool inconnu = false;
                if (persons != null)
                {
                    reply.Append("Je vois " + persons.Count() + " personne(s) sur la photo.|");

                    foreach (FullPersonDto result in persons)
                    {
                        if (result.FirstName != null)
                        {
                            reply.Append(this.DescriptionPersonImage(result));
                        }
                        else
                        {
                            inconnu = true;
                        }
                    }
                    if (inconnu)
                        reply.Append("|Passons aux inconnus, je vois:|");
                    foreach (FullPersonDto result in persons)
                    {
                        if (result.FirstName == null)
                        {
                            reply.Append(this.DescriptionPersonImage(result));
                        }
                    }

                }
            }

            if (OCR != "")
            {
                reply.Append("|Il me semble que je peux distinguer le texte suivant:");
                reply.Append(OCR);
            }

            return reply.ToString();
        }

        public StringBuilder DescriptionPersonImage(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (person.FirstName != null)
            {
                reply.Append("Parmi ces personnes, je connais " + person.FirstName);
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
            else if (Convert.ToString(person.Gender) == "female")
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
                if (person.FirstName == null)
                    reply.Append("Un homme");
                else
                    reply.Append(" qui est un homme");

                if (person.Beard >= 0.5 && person.Mustache >= 0.5)
                {
                    reply.Append(" barbu et moustachu");
                }
                else if (person.Beard >= 0.5)
                {
                    reply.Append(" barbu");
                }
                else if (person.Mustache >= 0.5)
                {
                    reply.Append(" moustachu");
                }

                reply.Append(" d'environ " + (int)person.Age + " ans ");
            }
            else
            {
                if (person.FirstName == null)
                    reply.Append("Un femme d'environ " + (int)person.Age + " ans ");
                else
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

            if (person.Happiness >= 0.75)
            {
                reply.Append("et qui est heureux");
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
                reply.Append("et qui est surpris");
            }
            else if (person.Anger >= 0.75)
            {
                reply.Append("et qui est en colère");
            }
            else if (person.Contempt >= 0.75)
            {
                reply.Append("et qui est méprisant");
            }
            else if (person.Disgust >= 0.75)
            {
                reply.Append("et qui est dégouté");
            }
            else if (person.Fear >= 0.75)
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

        public async Task<FullPersonDto[]> recognizepersonsPictureAsync(Stream picture)
        {
            System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

            using (var streamContent = new StreamContent(picture))
            using (var formData = new MultipartFormDataContent())
            {
                // Creates uri from configuration
                var uri = new Uri(WebAppUrl + $"{ApiBasePath}/{PersonsRecognitionQuery}");

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
    }
}