using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartineobotIOTMvvm.Models.EventsLoaderServices
{
    public interface IEventsLoaderService
    {
        Task<List<MeetupEvent>> GetEventsJsonAsync(int page);
    }
}
