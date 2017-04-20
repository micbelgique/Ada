using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK.Models
{
    public class FullPersonDto
    {
        public Guid PersonId { get; set; }
        public string FirstName { get; set; }
        public double Age { get; set; }
        public string Gender { get; set; }
        public double Beard { get; set; }
        public Glasses Glasses { get; set; }
        public double Mustache { get; set; }

    }
}
