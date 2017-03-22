using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdaBot.Models.EventsLoaderServices
{
    public interface IEventsLoaderService
    {
        Task<List<MeetupEvent>> GetEventsJsonAsync(int page);
    }
}
