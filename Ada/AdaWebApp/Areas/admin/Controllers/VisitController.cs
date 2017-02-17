using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using PagedList;
using System.Collections.Generic;

namespace AdaWebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class VisitController : Controller
    {
        private readonly UnitOfWork _uof;

        public VisitController()
        {
            _uof = new UnitOfWork();
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Visit visit = await _uof.VisitsRepository.GetByIdAsync(id);

            if (visit == null){
                return HttpNotFound();
            }

            return View(visit);
        }

        public async Task<ActionResult> Index(int? page)
        {
            var visits = (await _uof.VisitsRepository.GetAll()).OrderByDescending(v => v.Date);

            int currentPage = page ?? 1;
            return View(visits.ToPagedList(currentPage, 20));
        }

        protected override void Dispose(bool disposing)
        {
            _uof.Dispose(); 
            base.Dispose(disposing);
        }
    }
}