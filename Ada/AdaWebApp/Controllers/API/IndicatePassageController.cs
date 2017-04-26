using AdaSDK.Models;
using AdaWebApp.Migrations;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace AdaWebApp.Controllers.API
{
    public class IndicatePassageController : ApiController
    {
        private UnitOfWork _unit;
        private readonly ILog _logger;

        public IndicatePassageController()
        {
            _unit = new UnitOfWork();

            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(GetType());
        }

        [HttpPost]
        [Route("Api/IndicatePassageController/PostIndicatePassage")]
        public async void PostIndicatePassage(IndicatePassageDto indicatePassageDto)
        {
            Models.Entities.IndicatePassage indicatePassage = new Models.Entities.IndicatePassage()
            {
                IdFacebookConversation = indicatePassageDto.IdFacebookConversation,
                Firtsname = indicatePassageDto.Firtsname,
                IsSend = false,
                ToId = indicatePassageDto.To
            };

            _unit.IndicatePassageRepository.Insert(indicatePassage);
            await _unit.SaveAsync();
        }

        [HttpPut]
        [Route("Api/IndicatePassageController/PutIndicatePassage/{id}")]
        public async void PutIndicatePassage(string id)
        {
            await _unit.IndicatePassageRepository.PutMessageAsync(id);

            await _unit.SaveAsync();
        }
    }
}
