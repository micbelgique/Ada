using AdaSDK;
using AdaWebApp.Models.Entities;
using LinqKit;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class VisitRepository : BaseRepository<Visit>
    {
        public ApplicationDbContext context = null;
        public VisitRepository(ApplicationDbContext context) : base(context) { }

        public void AddOrUpdateVisit(Entities.Person person, DateTime dateOfVisit)
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
            return Table.Include(v => v.Person).Where(v => v.Date >= date)
                .ToList();
        }

        public List<Visit> GetBestFriend()
        {
            DateTime date2 = DateTime.Today;
            DateTime date1 = date2.AddDays(-2);
            List<Visit> bestFriends = new List<Visit>();

            //Best MALE friend
            int maxPasses = Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
            && v.Person.FirstName != null
            && v.Person.Gender == GenderValues.Male).Max(v => v.NbPasses);
            bestFriends.Add( 
                Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                && v.Person.FirstName != null
                && v.Person.Gender == GenderValues.Male).First(v => v.NbPasses == maxPasses)
                );
            //Best FEMALE friend
            maxPasses = Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
            && v.Person.FirstName != null
            && v.Person.Gender == GenderValues.Female).Max(v => v.NbPasses
            );
            bestFriends.Add(
                Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                && v.Person.FirstName != null
                && v.Person.Gender == GenderValues.Female).First(v => v.NbPasses == maxPasses)
                );
            //Best beard friend
            maxPasses = Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
            && v.Person.FirstName != null
            && v.Person.Gender == GenderValues.Male
            && v.ProfilePictures.OrderByDescending(p => p.Id).FirstOrDefault().Beard >= 0.5).Max(v => v.NbPasses);
            bestFriends.Add(
                Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                && v.Person.FirstName != null
                && v.Person.Gender == GenderValues.Male
                && v.ProfilePictures.OrderByDescending(p => p.Id).FirstOrDefault().Beard >= 0.5).First(v => v.NbPasses == maxPasses)
                );
            return bestFriends;
        }

        public List<Visit> GetVisitsByDate(DateTime date1, DateTime? date2)
        {
            if (date2 == null)
            {
                return Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)).ToList();
            }
            else
            {
                return Table.Include(v => v.Person).Where(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1) && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)).ToList();
            }
        }

        public List<Visit> GetVisitsForStats(DateTime? date1, DateTime? date2, GenderValues? gender, int? age1, int? age2, bool glasses, bool beard, bool mustache)
        {
            List<Glasses> glassesTest = new List<Glasses> { Glasses.ReadingGlasses, Glasses.Sunglasses, Glasses.SwimmingGoggles };
            DateTime today = DateTime.Today;
            if (age1 != null && age2 == null)
            {
                age1 = DateTime.Today.Year - age1;
            }
            else if (age1 != null && age2 != null)
            {
                age1 = DateTime.Today.Year - age1;
                age2 = DateTime.Today.Year - age2;
            }

            var searchCriteria = new
            {
                D1 = date1,
                D2 = date2,
                Gender = gender,
                A1 = age1,
                A2 = age2,
                Glasses = glasses,
                Beard = beard,
                Mustache = mustache
            };

            var predicate = PredicateBuilder.True<Visit>();
            if (searchCriteria.D1 != null && searchCriteria.D2 == null)
            {
                predicate = predicate.And(v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(searchCriteria.D1));
            }
            if (searchCriteria.D1 != null && searchCriteria.D2 != null)
            {
                predicate = predicate.And(v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(searchCriteria.D1));
                predicate = predicate.And(v => DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(searchCriteria.D2));
            }
            if (searchCriteria.D1 == null && searchCriteria.D2 == null)
            {
                predicate = predicate.And(v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(today));
            }
            if (searchCriteria.Gender != null)
            {
                predicate = predicate.And(v => v.Person.Gender == searchCriteria.Gender);
            }
            if (searchCriteria.A1 != null && searchCriteria.A2 == null)
            {
                predicate = predicate.And(v => v.Person.DateOfBirth.Year == searchCriteria.A1);
            }
            if (searchCriteria.A1 != null && searchCriteria.A2 != null)
            {
                predicate = predicate.And(v => v.Person.DateOfBirth.Year <= searchCriteria.A1);
                predicate = predicate.And(v => v.Person.DateOfBirth.Year >= searchCriteria.A2);
            }
            if (searchCriteria.Glasses == true)
            {
                predicate = predicate.And(v => glassesTest.Contains(v.ProfilePictures.OrderByDescending(p => p.Id).FirstOrDefault().Glasses));
            }
            if (searchCriteria.Beard == true)
            {
                predicate = predicate.And(v => v.ProfilePictures.OrderByDescending(p => p.Id).FirstOrDefault().Beard >= 0.5);
            }
            if (searchCriteria.Mustache == true)
            {
                predicate = predicate.And(v => v.ProfilePictures.OrderByDescending(p => p.Id).FirstOrDefault().Moustache >= 0.5);
            }
            return Context.Visits.AsExpandable().Where(predicate).ToList();
        }

        public List<Visit> GetLastVisitForAPersonByFirstname(string firstname)
        {
            //Compare the first time of the last visit of a day (v.Date) and the last visit of the person (DateOfBirth).
            return Table.Include(v => v.Person).Where(v => v.Person.FirstName == firstname
                                                            && v.Person.DateOfBirth.Day == v.Date.Day
                                                            && v.Person.DateOfBirth.Month == v.Date.Month)
                                                            .ToList();
        }

        public List<Visit> GetVisitForAPersonById(int id, int nbVisit)
        {
            return Table.OrderByDescending(v => v.Date).Where(v => v.Person.Id == id).Take(nbVisit).ToList();
        }

        public int GetNbVisits(GenderValues? gender, int? age1, int? age2)
        {
            DateTime dateAverage = DateTime.Today.AddDays(-120);
            int date = DateTime.Now.Day;
            int nbDayVisits = Table.Where(v => v.Date > dateAverage).GroupBy(x => DbFunctions.TruncateTime(x.Date)).Count();
            if (age1 != null && age2 == null)
            {
                age1 = DateTime.Today.Year - age1;
            }
            else if (age1 != null && age2 != null)
            {
                age1 = DateTime.Today.Year - age1;
                age2 = DateTime.Today.Year - age2;
            }

            var searchCriteria = new
            {
                Gender = gender,
                A1 = age1,
                A2 = age2
            };

            var predicate = PredicateBuilder.True<Visit>();
            if (searchCriteria.Gender != null)
            {
                predicate = predicate.And(v => v.Person.Gender == searchCriteria.Gender);
            }
            if (searchCriteria.A1 != null && searchCriteria.A2 == null)
            {
                predicate = predicate.And(v => v.Person.DateOfBirth.Year == searchCriteria.A1);
            }
            if (searchCriteria.A1 != null && searchCriteria.A2 != null)
            {
                predicate = predicate.And(v => v.Person.DateOfBirth.Year <= searchCriteria.A1);
                predicate = predicate.And(v => v.Person.DateOfBirth.Year >= searchCriteria.A2);
            }
            double result = (Context.Visits.AsExpandable().Where(predicate).Count() / nbDayVisits) + 0.5;

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