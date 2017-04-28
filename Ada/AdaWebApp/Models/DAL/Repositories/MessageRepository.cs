using AdaWebApp.Models.Entities;
using AdaSDK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class MessageRepository : BaseRepository<Message>
    {
        public ApplicationDbContext context = null;
        public MessageRepository(ApplicationDbContext context) : base(context) { }

        public List<Message> GetMessageByReceiver(int id)
        {
            return Table.Where(m => m.ToId == id && !m.IsRead).ToList();
        }

        public async System.Threading.Tasks.Task PutMessageAsync(int id, MessageDto msg)
        {
            Message message = await Table.FirstOrDefaultAsync(s => s.Id == id);

            message.IsRead = msg.IsRead;
            message.Read = msg.Read;
            
            this.Update(message);
        }
    }
}