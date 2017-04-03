using AdaSDK.Models;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using Common.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace AdaWebApp.Controllers.API
{
    public class MessageController : ApiController
    {
        private UnitOfWork _unit;
        private readonly ILog _logger;

        public MessageController()
        {
            _unit = new UnitOfWork();

            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(GetType());
        }

        // GET: api/Message
        public async Task<IEnumerable<Message>> GetMessage()
        {
            return await _unit.MessagesRepository.GetAll();
        }

        //POST: api/Message
        [ResponseType(typeof(Visit))]
        public async void PostMessage(MessageDto msg)
        {
            Message message = new Message()
            {
                From = msg.From,
                Contenu = msg.Contenu,
                IsRead = msg.IsRead,
                Send = msg.Send,
                Read = null,
                ToId = msg.To,
            };

            _unit.MessagesRepository.Insert(message);
            await _unit.SaveAsync();
        }
    }
}
