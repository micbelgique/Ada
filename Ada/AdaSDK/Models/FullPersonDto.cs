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

        public float Anger { get; set; }
        public float Contempt { get; set; }
        public float Disgust { get; set; }
        public float Fear { get; set; }
        public float Happiness { get; set; }
        public float Neutral { get; set; }
        public float Sadness { get; set; }
        public float Surprise { get; set; }

    }
}
