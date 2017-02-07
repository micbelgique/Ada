using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.DAL.Repositories;
using AdaWebApp.Models.Entities;
using PagedList;

namespace AdaWebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class PersonController : Controller
    {
        private readonly UnitOfWork _uof;

        public PersonController()
        {
            _uof = new UnitOfWork();
        }

        // GET: Admin/Visitor
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FName" ? "fname_desc" : "FName";

            const int nbUserPage = 10;
            int pageNumber = page ?? 1;

            //Using LING to Entities to retrieve our visitors from the database 
            var visitors = await _uof.PersonRepository.GetAll();  

            //If there is a specefic research, get the information from the collection
            if (!string.IsNullOrEmpty(searchString))
            {
                visitors = visitors.Where(s => (s.LastName?.ToLower().Contains(searchString.ToLower()) ?? false)
                                       ||  (s.FirstName?.ToLower().Contains(searchString.ToLower()) ?? false));
            }

            //Sorting the list
            switch (sortOrder)
            {
                case "name_desc":
                    visitors = visitors.OrderByDescending(s => s.LastName);
                    break;
                case "FName":
                    visitors = visitors.OrderBy(s => s.FirstName);
                    break;
                case "fname_desc":
                    visitors = visitors.OrderByDescending(s => s.FirstName);
                    break;
                default:
                    visitors = visitors.OrderBy(s => s.LastName);
                    break;
            }

            return View(visitors.ToPagedList(pageNumber, nbUserPage));
        }

        public async Task<ActionResult> Detail(int? id, string sortOrder)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Person visitor = await _uof.PersonRepository.GetByIdAsync(id);

            if (visitor == null){
                return HttpNotFound();
            }

            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            switch (sortOrder)
            {
                case "Date":
                    visitor.Visits = visitor.Visits.OrderBy(v => v.Date).ToList();
                    break;
                case "date_desc":
                    visitor.Visits = visitor.Visits.OrderByDescending(v => v.Date).ToList();
                    break;
            }


            return View(visitor);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Person person = await _uof.PersonRepository.GetByIdAsync(id);

            if (person == null){
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(person); 
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Person person)
        {
            if (!ModelState.IsValid){
                return View(person); 
            }

            try
            {
                Person personToUpdate = await _uof.PersonRepository.GetByIdAsync(person.Id);
                personToUpdate.FirstName = person.FirstName;
                personToUpdate.LastName = person.LastName;
                _uof.PersonRepository.Update(personToUpdate);
                await _uof.SaveAsync();
                return RedirectToAction("Detail", new {id = person.Id});
            }
            catch(Exception)
            {
                ModelState.AddModelError("", "An error occured while saving changes");
                return View(person); 
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