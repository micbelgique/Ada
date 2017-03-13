using System.Net;
using System.Web.Http;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using Common.Logging;

namespace AdaWebApp.Controllers.API
{
    [RoutePrefix("api/UserIndentified")]
    public class UserIndentifiedController : ApiController
    {
        private UnitOfWork _unit; 
        private readonly ILog _logger; 

        public UserIndentifiedController()
        {
            _unit = new UnitOfWork();

            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(GetType());
        }

        [HttpPost]
        [Route("AddUserIndentified")]
        public IHttpActionResult AddUserIndentified(UserIndentified userIndentified)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _unit.UserIndentifiedRepository.AddNewUserIndentified(userIndentified);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("CheckIdFacebook/{idFacebook}")]
        public bool CheckIdFacebook(string idFacebook)
        {
           return _unit.UserIndentifiedRepository.CheckByIdFacebook(idFacebook);
        }
    }
}