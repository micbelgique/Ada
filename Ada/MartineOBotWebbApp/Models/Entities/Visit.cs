using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MartineOBotWebApp.Models.Entities
{
    public class Visit
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:YYYY-MM-DD HH:mm:ss}")]
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public int NbPasses { get; set; }

        // Foreign keys
        public virtual IList<ProfilePicture> ProfilePictures { get; set; }

        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        public Visit(){
            ProfilePictures = new List<ProfilePicture>(); 
        }
    }
}