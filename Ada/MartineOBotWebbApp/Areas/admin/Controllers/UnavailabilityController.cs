using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MartineOBotWebApp.Areas.Admin.ViewModels;
using MartineOBotWebApp.Models.DAL;
using MartineOBotWebApp.Models.Entities;

namespace MartineOBotWebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UnavailabilityController : Controller
    {
        private readonly UnitOfWork _uof;
        
        public UnavailabilityController()
        {
            _uof = new UnitOfWork();
        }

        // GET: Admin/StaffMember/Unavailability
        public ActionResult Create(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(new UnavailabilityViewModel { StaffMemberId = id.Value });
        }

        // Post: Admin/StaffMember/Unavailability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UnavailabilityViewModel unavailability)
        {
            if (!ModelState.IsValid) return View(unavailability);

            try
            {
                _uof.UnavailabilitieRepository.Insert(new Unavailability
                {
                    StaffMemberId = unavailability.StaffMemberId, 
                    StarTime = unavailability.StarTime, 
                    EndTime = unavailability.EndTime    
                });

                await _uof.SaveAsync();
                return RedirectToAction("Detail", "StaffMember", new { id = unavailability.StaffMemberId });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", 
                    "Creation Unavailability failed. Try again, and if the problem persists see your system administrator.");

                return View(unavailability);
            }
        }

        // Delete: Admin/StaffMember/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, int memberId)
        {
            try
            {
                await _uof.UnavailabilitieRepository.Remove(id);
                await _uof.SaveAsync();
                return RedirectToAction("Detail", "StaffMember", new { id = memberId });
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Delete failed. Try again, and if the problem persists see your system administrator.";
                return RedirectToAction("Detail", "StaffMember", new { id = memberId });
            }
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