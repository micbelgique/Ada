using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK.Models
{
    public class MessageDto
    {
        public string Contenu { get; set; }
        public string From { get; set; }
        public int ID { get; set; }
        public bool IsRead { get; set; }
        public DateTime? Read { get; set; }
        public DateTime Send { get; set; }
        public int To { get; set; }
    }
}
