using AdaSDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdaSDK
{
    public class AdaClient
    {
        public HttpClient HttpClient { get; set; }

        public AdaClient()
        {
            HttpClient = new HttpClient();
            HttpClient.MaxResponseContentBufferSize = 256000;
        }

        public async Task<List<VisitDto>> GetVisitsToday()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri("http://adawebapp.azurewebsites.net/Api/Visits/VisitsToday"));
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

        public async Task<List<VisitDto>> GetVisitsByDate(DateTime? date1, DateTime? date2)
        {
            try
            {
                //Convertion date1
                var tmp = Convert.ToString(date1).Split(' ');
                tmp[0] = tmp[0].Replace('/', '-');
                date1 = Convert.ToDateTime(tmp[0]);
                tmp[0] = Convert.ToDateTime(date1).ToString("yyyy-MM-dd");
                //Convertion date2
                var tmp2 = Convert.ToString(date2).Split(' ');
                tmp2[0] = tmp2[0].Replace('/', '-');
                if (date2 == null)
                {
                    tmp2[0] = "null";
                }

                var response = await HttpClient.GetAsync(new Uri("http://adawebapp.azurewebsites.net/Api/Visits/VisitsByDate/" + tmp[0] + "/" + tmp2[0])); //Possiblement mettre .Value
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
                var response = await HttpClient.GetAsync(new Uri("http://adawebapp.azurewebsites.net/Api/Visits/LastVisitByFirstname/" + firstname));
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
                var response = await HttpClient.GetAsync(new Uri("http://adawebapp.azurewebsites.net/Api/Visits/VisitPersonById/" + id + "/" + nbVisit));
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

        public async Task<int> GetNbVisits(string gender,string age1, string age2)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri("http://adawebapp.azurewebsites.net/Api/Visits/GetNbVisits/" + gender + "/" + age1 + "/" + age2));
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
    }
}