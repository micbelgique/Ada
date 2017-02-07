using AdaWebApp.Models.Entities;
using System.Linq;

namespace AdaWebApp.Models.DAL.Repositories
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