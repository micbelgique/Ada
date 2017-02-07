using Newtonsoft.Json;

namespace MartineobotIOTMvvm.Models.EventsLoaderServices
{
    public class MeetupEvent
    {
        [JsonProperty("created")]
        public long Created { get; set; }
        [JsonProperty("duration")]
        public long Duration { get; set; }
        [JsonProperty("group")]
        public MeetupGroup Group { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("time")]
        public long Time { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("venue")]
        public Venue Venue { get; set; }
        [JsonProperty("how_to_find_us")]
        public string HowToFind { get; set; }
        [JsonProperty("visibility")]
        public string Visibility { get; set; }
    }

    public class Venue
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address_1")]
        public string Address { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("localized_country_name")]
        public string Country { get; set; }
    }

    public class MeetupGroup
    {
        [JsonProperty("created")]
        public long Created { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("urlname")]
        public string UrlName { get; set; }
    }
}
