using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MartineOBotWebApp.Models.Entities
{
    public class StaffMember
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public virtual IList<Unavailability> Unavailabilities { get; set; }

        public StaffMember(){
            Unavailabilities = new List<Unavailability>();
        }
    }
}
