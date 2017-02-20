using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK.Models
{
    public class VisitDto
    {
        public DateTime Date { get; set; }
        public int ID { get; set; }
        public int NbPasses { get; set; }
        public PersonVisitDto PersonVisit { get; set; }
        public object ProfilePicture { get; set; }
    }
}
