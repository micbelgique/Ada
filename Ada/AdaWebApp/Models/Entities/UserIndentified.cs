using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdaWebApp.Models.Entities
{
    public class UserIndentified
    {
        [Key]
        public int IdUserIndentified { get; set; }

        public string Firtsname { get; set; }

        public string LastName { get; set; }

        public string IdFacebook { get; set; }

        public bool Authorization { get; set; }
    }
}