using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK.Models
{
    public class IndicatePassageDto
    {
        public int Id { get; set; }
        public string IdFacebookConversation { get; set; }
        public string Firtsname { get; set; }
        public bool IsSend { get; set; }
        public string FromId { get; set; }
        public string RecipientID { get; set; }
        public string Channel { get; set; }
        public int To { get; set; }
    }
}
