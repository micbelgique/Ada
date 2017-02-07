using MartineOBotWebApp.Models.Entities;
using System.Linq;

namespace MartineOBotWebApp.Models.DAL.Repositories
{
    public class RecognitionItemRepository : BaseRepository<RecognitionItem>
    {
        public RecognitionItemRepository(ApplicationDbContext context) : base(context) { }

        public RecognitionItem Pop()
        {
            return Table.FirstOrDefault(); 
        }
    }
}