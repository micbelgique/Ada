using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdaW10.Models.EventsLoaderServices
{
    public interface IEventsLoaderService
    {
        Task<List<MeetupEvent>> GetEventsJsonAsync(int page);
    }
}
