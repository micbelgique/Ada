using AdaSDK.Models;
using AdaWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class IndicatePassageRepository : BaseRepository<IndicatePassage>
    {
        public ApplicationDbContext context = null;
        public IndicatePassageRepository(ApplicationDbContext context) : base(context) { }

        public async Task PutMessageAsync(int id)
        {
            IndicatePassage message = await Table.FirstOrDefaultAsync(s => s.Id == id);

            message.IsSend = true;

            this.Update(message);
        }

        public List<IndicatePassage> GetIndicatePassageByPerson(int id)
        {
            return Table.Where(m => m.ToId == id && !m.IsSend).ToList();
        }
    }
}