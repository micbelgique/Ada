using AdaWebApp.Models.Entities;
using System.Data.Entity;
using System.Linq;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class UserIndentifiedRepository : BaseRepository<UserIndentified>
    {
        public UserIndentifiedRepository(ApplicationDbContext context) : base(context) { }

        public void AddNewUserIndentified(UserIndentified userIndentified)
        {
            Context.UserIndentifieds.Add(userIndentified);
            Context.SaveChanges();
        }

        public bool CheckByIdFacebook(string idFacebook)
        {
            return Table.Where(u => u.IdFacebook == idFacebook).Any();
        }

        public bool GetAuthorizationByIdFacebook(string idFacebook)
        {
            return Table.Where(u => u.IdFacebook == idFacebook).Select(v => v.authorization).Single();
        }
    }
}