using AdaBridge;
using AdaWebApp.Models.Entities;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Web.Hosting.HostingEnvironment;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class PersonRepository : BaseRepository<Entities.Person>
    {
        public PersonRepository(ApplicationDbContext context) : base(context)
        {}

        public Entities.Person CreatePerson(Guid personApiId, Face face)
        {
            // Creates a new person entity
            var person = new Entities.Person { PersonApiId = personApiId };
            person.UpdateAge(face.FaceAttributes.Age);
            person.UpdateGender(GenderValuesHelper.Parse(face.FaceAttributes.Gender));

            // Creates directory to store person's pictures
            Directory.CreateDirectory(MapPath($"{Global.PersonsDatabaseDirectory}{personApiId}"));

            return person; 
        }

        public Entities.Person GetByApiId(Guid personApiId)
        {
            return Table.FirstOrDefault(p => p.PersonApiId == personApiId);
        }

        public IEnumerable<Entities.Person> GetPersonsByCandidateIds(IList<Guid> candidateIds)
        {
            return Table.Where(p => candidateIds.Any(c => c.Equals(p.PersonApiId)));
        }

        
    }
}