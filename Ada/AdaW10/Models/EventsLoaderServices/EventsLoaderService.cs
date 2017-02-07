using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AdaW10.Models.EventsLoaderServices
{
    public class EventsLoaderService : IEventsLoaderService
    {
        private HttpClient _httpClient;

        public EventsLoaderService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Get an events list from meetup
        /// </summary>
        public async Task<List<MeetupEvent>> GetEventsJsonAsync(int page)
        {
            var resp = await _httpClient.GetStringAsync($"https://api.meetup.com/micbelgique/events?&sign=true&photo-host=public&page={page}");
            var events = JsonConvert.DeserializeObject<List<MeetupEvent>>(resp);
            return events;
        }

        public List<MeetupEvent> GetEventsFromDate(List<MeetupEvent> events, DateTime date)
        {
            return events.Where(p =>
            {
                var d = new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(p.Time));
                return d.Date == date.Date;
            }).ToList();
            
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing || _httpClient == null) return;

            _httpClient.Dispose();
            _httpClient = null;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
