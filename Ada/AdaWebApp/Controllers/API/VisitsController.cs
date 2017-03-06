using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using AdaWebApp.Models.DAL.Repositories;
using Common.Logging;
using System.Threading.Tasks;
using AdaSDK.Models;
using AdaSDK;

namespace AdaWebApp.Controllers.API
{
    [RoutePrefix("api/Visits")]
    public class VisitsController : ApiController
    {
        //ATTENTION A éliminer! Le PUT et le POST utilisent encore le DBContext
        //private ApplicationDbContext db = new ApplicationDbContext();
        private UnitOfWork _unit;
        private readonly ILog _logger;

        public VisitsController()
        {
            _unit = new UnitOfWork();

            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(GetType());
        }

        // GET: api/Visits
        public async Task<IEnumerable<Visit>> GetVisits()
        {
           return await _unit.VisitsRepository.GetAll();
        }

        [HttpGet]
        [Route("VisitsToday")]
        // GET: get visits of the day
        public List<VisitDto> GetVisitsToday()
        {
            var visits = _unit.VisitsRepository.GetVisitsToday();
            if (visits == null)
            {
                return null;
            }
            return visits.Select(v => v.ToDto()).ToList();
        }

        [HttpGet]
        [Route("VisitsByDate/{date1}/{date2}")]
        // GET: get visits of the day
        public List<VisitDto> GetVisitsByDate(DateTime? date1, DateTime? date2)
        {
            var visits = _unit.VisitsRepository.GetVisitsByDate(date1, date2);
            if (visits == null)
            {
                return null;
            }
            return visits.Select(v => v.ToDto()).ToList();
        }

        [HttpGet]
        [Route("LastVisitByFirstname/{firstname}")]
        // GET: get visits of the day
        public List<VisitDto> GetVisitByFirstname(string firstname)
        {
            var visits = _unit.VisitsRepository.GetLastVisitForAPersonByFirstname(firstname);
            return visits.Select(v => v.ToDto()).ToList();
        }

        [HttpGet]
        [Route("VisitPersonById/{id}/{nbVisit}")]
        // GET: get visits of the day
        public List<VisitDto> GetVisitPersonById(int id, int nbVisit)
        {
            var visits = _unit.VisitsRepository.GetVisitForAPersonById(id,nbVisit);
            return visits.Select(v => v.ToDto()).ToList();
        }
       
        [HttpGet]
        [Route("GetNbVisits/{gender}/{age1}/{age2}")]
        // GET: get visits of the day
        public int GetNbVisits(GenderValues? gender ,int? age1, int? age2)
        {
            int nbVisits = _unit.VisitsRepository.GetNbVisits(gender,age1, age2);
            return nbVisits;
        }

        // GET: api/Visits/5
        [ResponseType(typeof(Visit))]
        public async Task<IHttpActionResult> GetVisit(int id)
        {
            Visit visit = await _unit.VisitsRepository.GetByIdAsync(id);
            if (visit == null)
            {
                return NotFound();
            }

            return Ok(visit);
        }

        /*
        // PUT: api/Visits/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVisit(int id, Visit visit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != visit.Id)
            {
                return BadRequest();
            }

            db.Entry(visit).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VisitExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Visits
        [ResponseType(typeof(Visit))]
        public IHttpActionResult PostVisit(Visit visit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //_unit.VisitsRepository.
            db.Visits.Add(visit);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = visit.Id }, visit);
        }
        */

        // DELETE: api/Visits/5
        [ResponseType(typeof(Visit))]
        //public IHttpActionResult DeleteVisit(int id)
        public async Task<IHttpActionResult> DeleteVisit(int id)
        {
            //Visit visit = db.Visits.Find(id);
            Visit visit = await _unit.VisitsRepository.GetByIdAsync(id);
            if (visit == null)
            {
                return NotFound();
            }

            await _unit.VisitsRepository.Remove(id);
            await _unit.SaveAsync();
            //db.Visits.Remove(visit);
            //db.SaveChanges();

            return Ok(visit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
                _unit.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VisitExists(int id)
        {
            //return db.Visits.Count(e => e.Id == id) > 0;
            return _unit.VisitsRepository.CheckVisitExist(id);
        }
    }
}