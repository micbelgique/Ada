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

        public List<Visit> GetVisitsToday()
        {
            DateTime date = DateTime.Today;
            return Table.Include(v => v.Person).Where(v => v.Date >= date).ToList();
        }

        public List<Visit> GetLastVisitForAPersonByFirstname(string firstname)
        {
            //Compare the first time of the last visit of a day (v.Date) and the last visit of the person (DateOfBirth).
            return Table.Include(v => v.Person).Where(v => v.Person.FirstName == firstname 
                                                            && v.Person.DateOfBirth.Day == v.Date.Day
                                                            && v.Person.DateOfBirth.Month == v.Date.Month)
                                                            //On retire ici l'année car DateOfBirth calcul son année via l'estimation d'ada
                                                            //et ne prend donc pas l'année de la dernière visite.
                                                            //&& v.Person.DateOfBirth.Year == v.Date.Year)
                                                            .ToList();
        }

        public List<Visit> GetVisitForAPersonById(int id)
        {
            return Table.Include(v => v.Person).Where(v => v.Person.Id == id).Take(10).ToList();
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