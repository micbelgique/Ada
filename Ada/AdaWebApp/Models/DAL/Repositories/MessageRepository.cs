using AdaWebApp.Models.Entities;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class MessageRepository : BaseRepository<Message>
    {
        public ApplicationDbContext context = null;
        public MessageRepository(ApplicationDbContext context) : base(context) { }

        
    }
}