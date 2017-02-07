using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using PagedList;

namespace AdaWebApp.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProfilePictureController : Controller
    {
        private readonly UnitOfWork _uof;

        public ProfilePictureController()
        {
            _uof = new UnitOfWork();
        }

        public async Task<ActionResult> Index(int? page)
        {
            int currentPage = page ?? 1;

            IEnumerable<ProfilePicture> profilePictures = (await _uof.ProfilePicturesRepository.GetAll()).OrderByDescending(v => v.Id);
            return View(profilePictures.ToPagedList(currentPage, 20));
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null){
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProfilePicture picture = await _uof.ProfilePicturesRepository.GetByIdAsync(id);

            if (picture == null)
            {
                return HttpNotFound();
            }

            return View(picture);
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