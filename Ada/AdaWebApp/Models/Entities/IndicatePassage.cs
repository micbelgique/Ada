using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdaWebApp.Models.Entities
{
    public class IndicatePassage
    {
        [Key]
        public int Id { get; set; }
        public string IdFacebookConversation { get; set; }
        public string Firtsname { get; set; }
        public bool IsSend { get; set; }
        public string FromId { get; set; }
        public string RecipientID { get; set; }
        public string Channel { get; set; }

        [ForeignKey("To")]
        public int ToId { get; set; }
        public virtual Person To { get; set; }
    }
}