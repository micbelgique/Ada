using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using AdaBridge;
using Microsoft.ProjectOxford.Face.Contract;

namespace AdaWebApp.Models.Entities
{
    public class ProfilePicture
    {
        public int Id { get; set; }

        [Index(IsUnique = false)]
        public Guid FaceApiId { get; set; }
        
        public string Uri { get; set; }

        [DisplayName("Age")]
        public double Age { get; set; }
        public GenderValues Gender { get; set; }
        public double Confidence { get; set; }

        public FaceRectangle FaceRectangle { get; set; }

        // Foreign key
        public virtual EmotionScores EmotionScores { get; set; }
        
        public int VisitId { get; set; }
        public virtual Visit Visit { get; set; }
    }
}
