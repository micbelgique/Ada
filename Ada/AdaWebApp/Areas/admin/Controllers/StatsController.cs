using System;
using System.Diagnostics;
using AdaWebApp.Areas.Admin.ViewModels;
using System.Web.Mvc;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using System.Linq;
using AdaBridge;
using AdaWebApp.Helpers;
using System.Threading.Tasks;

namespace AdaWebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class StatsController : Controller
    {
        private readonly UnitOfWork _uof;

        public StatsController()
        {
            _uof = new UnitOfWork();
           
        }

        // GET: Admin/Stats
        public ActionResult Index()
        {
            var model = new StatsViewModel
            {
                AverageAge = _uof.StatRepository.GetAverageAge(),
                TotalVisitors = _uof.StatRepository.GetNumberOfPersons(),
                Female = _uof.StatRepository.GetPercentOfFemale(),
                Male = _uof.StatRepository.GetPercentOfMale(),
                EmotionScores = _uof.StatRepository.GetAverageEmotions()
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing){
                _uof.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}