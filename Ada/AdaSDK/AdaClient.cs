using AdaSDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK
{
    public class AdaClient
    {
        public HttpClient HttpClient { get; set; }
        public string WebAppUrl { get; set; }

        public AdaClient()
        {
            HttpClient = new HttpClient();
            HttpClient.MaxResponseContentBufferSize = 256000;
        }

        public async Task<string> CheckIdFacebook(string idfacebook)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/UserIndentified/CheckIdFacebook/" + idfacebook));
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return "false";
            }
        }

        public async Task<string> GetAuthorizationFacebook(string idfacebook)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/UserIndentified/GetAuthorization/" + idfacebook));
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return "false";
            }
        }

        public async Task<HttpResponseMessage> AddNewMessage(MessageDto message)
        {
            try
            {
                var json = JsonConvert.SerializeObject(message);

                var buffer = Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await HttpClient.PostAsync(WebAppUrl + "/api/Message", byteContent);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return result.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return null;
            }
        }

        public async Task PutMessage(MessageDto message)
        {
            try
            {
                var json = JsonConvert.SerializeObject(message);

                var buffer = Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await HttpClient.PutAsync(new Uri(WebAppUrl + "/Api/Message/PutMessage/" + message.ID), byteContent);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                throw;
            }
        }

        public async Task<HttpResponseMessage> AddNewUserIndentified(UserIndentifiedDto userIndentified)
        {
            try
            {
                var json = JsonConvert.SerializeObject(userIndentified);

                var buffer = Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var client = new HttpClient();

                var result = await HttpClient.PostAsync(new Uri(WebAppUrl + "Api/UserIndentified/AddUserIndentified"), byteContent);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return result.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return null;
            }
        }

        public async Task<List<VisitDto>> GetVisitsToday()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/VisitsToday"));
                var content = await response.Content.ReadAsStringAsync();
                var visits = JsonConvert.DeserializeObject<List<VisitDto>>(content);
                return visits;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<VisitDto>();
            }
        }

        public async Task<VisitDto> GetLastVisit()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/LastVisit"));
                var content = await response.Content.ReadAsStringAsync();
                var visit = JsonConvert.DeserializeObject<VisitDto>(content);
                return visit;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new VisitDto();
            }
        }

        public async Task<List<VisitDto>> GetVisitsNow()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/VisitsNow"));
                var content = await response.Content.ReadAsStringAsync();
                var visits = JsonConvert.DeserializeObject<List<VisitDto>>(content);
                return visits;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<VisitDto>();
            }
        }

        public async Task<List<VisitDto>> GetBestFriend()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/BestFriend"));
                var content = await response.Content.ReadAsStringAsync();
                var visit = JsonConvert.DeserializeObject<List<VisitDto>>(content);
                return visit;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<VisitDto>();
            }
        }

        public async Task<List<VisitDto>> GetVisitsForStats(DateTime? date1, DateTime? date2, GenderValues? gender, int? age1, int? age2, bool glasses, bool beard, bool mustache)
        {
            try
            {
                string d1 = null;
                string d2 = null;
                string gend = null;
                string a1 = null;
                string a2 = null;
                string[] tmp = { "null" };
                string[] tmp2 = { "null" };

                if (date1 != null)
                {
                    //Convertion date1
                    tmp = Convert.ToString(date1).Split(' ');
                    tmp[0] = tmp[0].Replace('/', '-');
                    date1 = Convert.ToDateTime(tmp[0]);
                    tmp[0] = Convert.ToDateTime(date1).ToString("yyyy-MM-dd");

                    if (date2 != null)
                    {
                        //Convertion date2
                        tmp2 = Convert.ToString(date2).Split(' ');
                        tmp2[0] = tmp2[0].Replace('/', '-');
                        date2 = Convert.ToDateTime(tmp2[0]);
                        tmp2[0] = Convert.ToDateTime(date2).ToString("yyyy-MM-dd");
                    }
                }
                if (tmp[0] == null)
                {
                    d1 = "null";
                }
                else
                {
                    d1 = tmp[0];
                }
                if (tmp2[0] == null)
                {
                    d2 = "null";
                }
                else
                {
                    d2 = tmp2[0];
                }
                if (gender == null)
                {
                    gend = "null";
                }
                else
                {
                    gend = gender.ToString();
                }
                if (age1 == null)
                {
                    a1 = "null";
                }
                else
                {
                    a1 = age1.ToString();
                }
                if (age2 == null)
                {
                    a2 = "null";
                }
                else
                {
                    a2 = age2.ToString();
                }

                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/VisitsForStats/" + d1 + "/" + d2 + "/" + gend + "/" + a1 + "/" + a2 + "/" + glasses + "/" + beard + "/" + mustache));
                var content = await response.Content.ReadAsStringAsync();
                var visits = JsonConvert.DeserializeObject<List<VisitDto>>(content);
                return visits;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<VisitDto>();
            }
        }

        public async Task<List<VisitDto>> GetLastVisitPerson(string firstname)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/LastVisitByFirstname/" + firstname));
                var content = await response.Content.ReadAsStringAsync();
                var visits = JsonConvert.DeserializeObject<List<VisitDto>>(content);
                return visits;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<VisitDto>();
            }
        }

        public async Task<List<VisitDto>> GetVisitPersonById(int id, int nbVisit)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/VisitPersonById/" + id + "/" + nbVisit));
                var content = await response.Content.ReadAsStringAsync();
                var visits = JsonConvert.DeserializeObject<List<VisitDto>>(content);
                return visits;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<VisitDto>();
            }
        }

        public async Task<List<MessageDto>> GetMessageByReceiver(int id)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "/Api/Message/MessageReceiver/" + id));
                var content = await response.Content.ReadAsStringAsync();
                var messages = JsonConvert.DeserializeObject<List<MessageDto>>(content);
                return messages;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<MessageDto>();
            }
        }

        public async Task<int> GetNbVisits(string gender, string age1, string age2)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + "Api/Visits/GetNbVisits/" + gender + "/" + age1 + "/" + age2));
                var content = await response.Content.ReadAsStringAsync();
                var nbVisitsAge = JsonConvert.DeserializeObject<int>(content);
                return nbVisitsAge;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new int();
            }
        }
        public async Task<PersonVisitDto> GetPersonByFaceId(Guid id)
        {
            try
            {
                string url = (WebAppUrl + "/Api/Visits/GetPersonByFaceId/" + id);
                url = url.Replace("{", "");
                url = url.Replace("}", "");
                var response = await HttpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var person = JsonConvert.DeserializeObject<PersonVisitDto>(content);
                return person;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new PersonVisitDto();
            }
        }
    }
}