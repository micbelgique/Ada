using TechOfficeSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace TechOfficeSDK
{
    public class TechOfficeClient
    {
        public HttpClient HttpClient { get; set; }
        public string WebAppUrl { get; set; }

        public TechOfficeClient()
        {
            HttpClient = new HttpClient();
            HttpClient.MaxResponseContentBufferSize = 256000;
        }

        public async Task<List<Room>> GetRooms()
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + $"Api/Rooms"));
                var content = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<Room>>(content);
                return rooms;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new List<Room>();
            }
        }

        public async Task<Room> GetRoom(int RoomId)
        {
            try
            {
                var response = await HttpClient.GetAsync(new Uri(WebAppUrl + $"Api/Rooms/{RoomId}"));
                var content = await response.Content.ReadAsStringAsync();
                var room = JsonConvert.DeserializeObject<Room>(content);
                return room;
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                return new Room();
            }
        }

        public async Task PutRoom(Room room)
        {
            try
            {
                var json = JsonConvert.SerializeObject(room);
                var buffer = Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await HttpClient.PutAsync(new Uri(WebAppUrl + $"/api/rooms/{room.Id}"), byteContent);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            catch (Exception e)
            {
                // TODO : Propagate exception to caller
                throw;
            }

        }
    }
}
