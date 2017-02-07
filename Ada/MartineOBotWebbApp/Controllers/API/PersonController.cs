using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using MartineobotBridge;
using MartineOBotWebApp.Models.DAL;
using MartineOBotWebApp.Models.Entities;
using MartineOBotWebApp.Models.Services.PersonService;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Person = MartineOBotWebApp.Models.Entities.Person;


namespace MartineOBotWebApp.Controllers.API
{
    [Authorize(Roles = "martineobot")]
    [RoutePrefix("api/person")]
    public class PersonController : ApiController
    {
        private UnitOfWork _unit; 
        private readonly PersonService _personService;
        private readonly ILog _logger;

        public PersonController()
        {
            _unit = new UnitOfWork(); 
            _personService = new PersonService(_unit); 

            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(GetType()); 
        }

        [HttpPost]
        [Route("recognizepersons")]
        public async Task<HttpResponseMessage> RecognizePersonsAsync()
        {
            if (!Request.Content.IsMimeMultipartContent() || HttpContext.Current.Request.Files.Count == 0){
                return GetWebServiceError("BadRequest", "Request must be multipart and contains one picture file");
            }

            try
            {
                // Get uploaded file
                HttpPostedFile file = HttpContext.Current.Request.Files[0];

                // Save the temporary file and get its path
                string filePath = await _personService.SaveTemporaryFileAsync(file);

                // Detect faces into picture
                Face[] faces = await _personService.DetectFacesFromPictureAsync(filePath);

                List<RecognitionItem> recogItems = await _personService.RecognizePersonsAsync(faces, filePath);

                // Add items to queue
                await _personService.Enqueue(recogItems);
                
                return Request.CreateResponse(HttpStatusCode.OK, recogItems.Select(r => r.ToPersonDto()));
            }
            catch (FaceAPIException e)
            {
                _logger.Error($"Exception : {e.ErrorCode} : {e.Message}", e);
                return GetWebServiceError(e.ErrorCode, e.ErrorMessage);
            }
            catch (Exception e)
            {
                _logger.Error($"Exception : {e.Message}", e);
                return GetWebServiceError("InternalError", e.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("startprocessingqueue")]
        public async Task<HttpResponseMessage> StartProcessingQueue()
        {
            await _personService.ProcessQueue();
            _logger.Info("Process of queue started at " + DateTime.UtcNow);
            return Request.CreateResponse(HttpStatusCode.OK);  
        }


        [HttpPost]
        [Route("updatepersoninformation")]
        public async Task<HttpResponseMessage> UpdatePersonInformation(PersonUpdateDto personUpdateDto)
        {
            if (personUpdateDto.RecognitionId != 0)
            {
                bool isFillFirstName = !string.IsNullOrEmpty(personUpdateDto.FirstName);
                bool isFillReason = !string.IsNullOrEmpty(personUpdateDto.ReasonOfVisit);

                Person person;

                if (personUpdateDto.PersonId == default(Guid)){
                    person = await _personService.ProcessRecognitionItem(personUpdateDto.RecognitionId);
                }
                else{
                    person = _unit.PersonRepository.GetByApiId(personUpdateDto.PersonId); 
                }

                if (person != null)
                {
                    if (isFillFirstName) person.FirstName = personUpdateDto.FirstName;

                    if (isFillReason)
                    {
                        var lastOrDefault = person.Visits.LastOrDefault();
                        if (lastOrDefault != null) lastOrDefault.Reason = personUpdateDto.ReasonOfVisit;
                    }

                    await _unit.SaveAsync(); 
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, GetWebServiceError("BadRequest", "Recognition id must be specified")); 
        }

        [HttpGet]
        [Route("cleanqueue")]
        public async Task<HttpResponseMessage> CleanQueue()
        {
            await _personService.CleanQueue();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private HttpResponseMessage GetWebServiceError(string errorCode, string errorMessage,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return Request.CreateResponse(statusCode, new WebServiceError
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                HttpStatus = statusCode
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_unit != null)
                {
                    _unit.Dispose();
                    _unit = null; 
                }
            }
            base.Dispose(disposing);
        }
    }
}
