using AdaSDK;
using AdaWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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

        public List<Visit> GetVisitsByDate(DateTime? date1, DateTime? date2)
        {
            if (date2 == null)
            {
                return Table.Include(v => v.Person).Where(v => v.Date == date1).ToList();
            }
            else
            {
                DateTime tmp = Convert.ToDateTime(date2);
                tmp = tmp.AddDays(1);
                return Table.Include(v => v.Person).Where(v => v.Date >= date1 && v.Date <= tmp).ToList();
            }
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

        public List<Visit> GetVisitForAPersonById(int id,int nbVisit) 
        {
            return Table.OrderByDescending(v => v.Date).Where(v => v.Person.Id == id).Take(nbVisit).ToList();
        }

        public int GetNbVisits(GenderValues? gender ,int? age1, int? age2)
        {
            DateTime dateAverage = DateTime.Today.AddDays(-120);
            int date = DateTime.Now.Day;
            int nbDayVisits = Table.Where(v => v.Date > dateAverage).GroupBy(x => DbFunctions.TruncateTime(x.Date)).Count();
            int nbVisits;

            if(age1 == null && gender == null)
            {
                nbVisits = Table.Count(v => v.Date > dateAverage);
            }
            else if(age1 == null)
            {
                nbVisits = Table.Where(v => v.Person.Gender == gender).Count(v => v.Date > dateAverage);
            }
            else if(gender == null)
            {
                age1 = DateTime.Today.Year - age1;

                if (age2 == null)
                {
                    nbVisits = Table.Where(v => v.Person.DateOfBirth.Year == age1).Count(v => v.Date > dateAverage);
                }
                else
                {
                    age2 = DateTime.Today.Year - age2;

                    nbVisits = Table.Where(v => v.Person.DateOfBirth.Year <= age1 && v.Person.DateOfBirth.Year >= age2).Count(v => v.Date > dateAverage);
                }
            }
            else 
            {
                age1 = DateTime.Today.Year - age1;

                if (age2 == null)
                {
                    nbVisits = Table.Where(v => v.Person.DateOfBirth.Year == age1 && v.Person.Gender == gender).Count(v => v.Date > dateAverage);
                }
                else
                {
                    age2 = DateTime.Today.Year - age2;
                    nbVisits = Table.Where(v => v.Person.DateOfBirth.Year <= age1 && v.Person.DateOfBirth.Year >= age2 && v.Person.Gender == gender).Count(v => v.Date > dateAverage);
                }
            }

            double result = (nbVisits / nbDayVisits) +0.5;

            return (int)Math.Round(result, 0);
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