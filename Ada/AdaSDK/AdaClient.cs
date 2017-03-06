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