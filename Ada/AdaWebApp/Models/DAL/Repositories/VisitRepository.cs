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

        public List<Visit> GetVisitsForStats(DateTime? date1, DateTime? date2, GenderValues? gender, int? age1, int? age2)
        {
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
            
            if (date1 != null && date2 == null )
            {
                if (gender != null)
                {
                    if (age1 != null)
                    {
                        if (age2 != null)
                        {
                            var test = Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)
                                && v.Person.Gender == gender
                                && v.Person.DateOfBirth.Year <= age1
                                && v.Person.DateOfBirth.Year >= age2).ToList();
                            return test;
                        }
                        else
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)
                                && v.Person.Gender == gender
                                && v.Person.DateOfBirth.Year == age1).ToList();
                        }
                    }
                    else
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                        v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)
                        && v.Person.Gender == gender).ToList();
                    }
                }
                else if (gender == null)
                {
                    if (age2 != null)
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)
                            && v.Person.DateOfBirth.Year <= age1
                            && v.Person.DateOfBirth.Year >= age2).ToList();
                    }
                    else
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)
                            && v.Person.DateOfBirth.Year == age1).ToList();
                    }
                }
                else
                {
                    return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) == DbFunctions.TruncateTime(date1)).ToList();
                }
            }
            else if (date1 != null && date2 != null)
            {
                if (gender != null)
                {
                    if (age1 != null)
                    {
                        if (age2 != null)
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                                && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                                && v.Person.Gender == gender
                                && v.Person.DateOfBirth.Year <= age1
                                && v.Person.DateOfBirth.Year >= age2).ToList();
                        }
                        else
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                                && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                                && v.Person.Gender == gender
                                && v.Person.DateOfBirth.Year == age1).ToList();
                        }
                    }
                    else
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                        v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                        && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                        && v.Person.Gender == gender).ToList();
                    }
                }
                else if (gender == null)
                {
                    if (age2 != null)
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                            && v.Person.DateOfBirth.Year <= age1
                            && v.Person.DateOfBirth.Year >= age2).ToList();
                    }
                    else if (age1 != null)
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)
                            && v.Person.DateOfBirth.Year == age1).ToList();
                    }
                    else
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)).ToList();
                    }
                }
                else
                {
                    return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                            v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(date1)
                            && DbFunctions.TruncateTime(v.Date) <= DbFunctions.TruncateTime(date2)).ToList();
                }
            }
            else
            {
                if (gender != null)
                {
                    if (age1 != null)
                    {
                        if (age2 != null)
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(today)
                                && v.Person.Gender == gender
                                && v.Person.DateOfBirth.Year <= age1
                                && v.Person.DateOfBirth.Year >= age2).ToList();
                        }
                        else
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(today)
                                && v.Person.Gender == gender
                                && v.Person.DateOfBirth.Year == age1).ToList();
                        }
                    }
                    else
                    {
                        return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                        v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(today)
                        && v.Person.Gender == gender).ToList();
                    }
                }
                else
                {
                    if (age1 != null)
                    {
                        if (age2 != null)
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(today)
                                && v.Person.DateOfBirth.Year <= age1
                                && v.Person.DateOfBirth.Year >= age2).ToList();
                        }
                        else
                        {
                            return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                                v => DbFunctions.TruncateTime(v.Date) >= DbFunctions.TruncateTime(today)
                                && v.Person.DateOfBirth.Year == age1).ToList();
                        }
                    }
                }
                return Table.Include(picture => picture.ProfilePictures).Include(v => v.Person).Where(
                    v => v.Date >= today).ToList();
            }
        }

        public List<Visit> GetLastVisitForAPersonByFirstname(string firstname)
        {
            //Compare the first time of the last visit of a day (v.Date) and the last visit of the person (DateOfBirth).
            return Table.Include(v => v.Person).Where(v => v.Person.FirstName == firstname 
                                                            && v.Person.DateOfBirth.Day == v.Date.Day
                                                            && v.Person.DateOfBirth.Month == v.Date.Month)
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