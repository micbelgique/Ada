using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaSDK.Models
{
    public class ProfilePictureDto
    {
        public double Beard { get; set; }
        public EmotionDto EmotionScore { get; set; }
        public Glasses Glasses { get; set; }
        public double Mustache { get; set; }
        public string Uri { get; set; }
    }
}
