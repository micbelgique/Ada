using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdaWebApp.Models.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string From { get; set; }
        public string Contenu { get; set; }
        public bool IsRead { get; set; }
        public DateTime Send { get; set; }
        public DateTime? Read { get; set; }

        // Foreign key
        [ForeignKey("To")]
        public int ToId { get; set; }
        public virtual Person To { get; set; }
    }
}