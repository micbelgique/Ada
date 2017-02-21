using AdaSDK.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

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

        public async Task<List<VisitDto>> GetLastVisitPerson()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri("http://adawebapp.azurewebsites.net/Api/Visits/LastVisitByFirstname"));
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
    }
}