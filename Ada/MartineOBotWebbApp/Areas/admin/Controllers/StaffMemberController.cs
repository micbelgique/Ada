using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MartineOBotWebApp.Models.DAL;
using MartineOBotWebApp.Models.Entities;
using MartineOBotWebApp.Models.DAL.Repositories;
using PagedList;

namespace MartineOBotWebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class StaffMemberController : Controller
    {
        private readonly UnitOfWork _uof;
        
        public StaffMemberController()
        {
            _uof = new UnitOfWork();
        }

        // GET: Admin/StaffMember
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FName" ? "fname_desc" : "FName";

            var members = await _uof.StaffMembersRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                members = members.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    members = members.OrderByDescending(s => s.LastName);
                    break;
                case "FName":
                    members = members.OrderBy(s => s.FirstName);
                    break;
                case "fname_desc":
                    members = members.OrderByDescending(s => s.FirstName);
                    break;
                default:
                    members = members.OrderBy(s => s.LastName);
                    break;
            }

            const int nbUserPage = 10;
            int pageNumber = page ?? 1;

            return View(members.ToPagedList(pageNumber, nbUserPage));
        }
        
        // GET: Admin/StaffMember/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/StaffMember/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StaffMember staff)
        {
            if (!ModelState.IsValid) return View(staff);

            try
            {
                _uof.StaffMembersRepository.Insert(staff);
                await _uof.SaveAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("CreateError", "Update Error : Try again, and if the problem persists see your system administrator.");
                return View(staff);
            }
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            StaffMember member = await _uof.StaffMembersRepository.GetByIdAsync(id);

            if (member == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(member);
        }

        // Get: Admin/StaffMember/Edit
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StaffMember member = await _uof.StaffMembersRepository.GetByIdAsync(id);

            if (member == null){
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(member);
        }

        // POST: Admin/StaffMember/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StaffMember staff)
        {
            if (!ModelState.IsValid) return View(staff);

            try{
                _uof.StaffMembersRepository.Update(staff);
                await _uof.SaveAsync();
                return RedirectToAction("Index");
            }
            catch (Exception){
                ModelState.AddModelError("EditError", "Update Error : Try again, and if the problem persists see your system administrator.");
                return View(staff);
            }
        }

        // Get: Admin/StaffMember/Delete
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StaffMember staffMember = await _uof.StaffMembersRepository.GetByIdAsync(id);

            if (staffMember == null){
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(staffMember); 
        }

        // POST: Admin/StaffMember/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(StaffMember staffMember)
        {
            try{
                _uof.StaffMembersRepository.Remove(staffMember);
                await _uof.SaveAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("DeleteError", "Delete failed. Try again, and if the problem persists see your system administrator.");
                return View(staffMember);
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
