using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK.Models
{
    public class IndicatePassageDto
    {
        public string IdFacebookConversation { get; set; }
        public string Firtsname { get; set; }
        public bool IsSend { get; set; }
        public int To { get; set; }
    }
}
