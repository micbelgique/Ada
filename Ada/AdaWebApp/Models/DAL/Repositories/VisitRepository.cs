using AdaWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class VisitRepository : BaseRepository<Visit>
    {
        public VisitRepository(ApplicationDbContext context) : base(context) { }

        public void AddOrUpdateVisit(Person person, DateTime dateOfVisit)
        {
            if (person == null) return; 

            if (!Table.Any(v => v.PersonId == person.Id && DbFunctions.DiffDays(v.Date, dateOfVisit) == 0))
                person.Visits.Add(new Visit { Date = dateOfVisit, NbPasses = 1 });
            else
                person.Visits.Last().NbPasses++;
        }

        public List<Visit> GetVisitsByDate()
        {
            DateTime day = DateTime.Today;
            List<Visit> visits = new List<Visit>();
            visits.Any(v => v.Date == day);
            return visits;
        }

        public bool CheckVisitExist(int id)
        {
            if (!Table.Any(v => v.Id == id))
            {
                return true;
            }
            return false;
        } 
    }
}