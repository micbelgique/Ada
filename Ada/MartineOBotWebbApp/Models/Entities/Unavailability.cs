using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartineOBotWebApp.Models.Entities
{
    public class Unavailability
    {
        public int Id { get; set; }

        [Display(Name = "Start Time")]
        public DateTime StarTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        //Foreign Key
        public int StaffMemberId { get; set; }
        public virtual StaffMember StaffMember { get; set; }
    }
}
