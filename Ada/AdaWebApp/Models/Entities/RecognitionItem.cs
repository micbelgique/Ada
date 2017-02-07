using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.ProjectOxford.Face.Contract;

namespace AdaWebApp.Models.Entities
{
    [Table("WorkList")]
    public class RecognitionItem
    {
        public int Id { get; set; }
        public Face Face { get; set; }
        public string ImageUrl { get; set; }
        public int ImageCounter { get; set; }
        public double Confidence { get; set; }
        public DateTime DateOfRecognition { get; set; }

        // Foreign key
        public int? PersonId { get; set; }
        public virtual Person Person { get; set; }

        public int? ProfilePictureId { get; set; }
        public virtual ProfilePicture ProfilePicture { get; set; }
    }
}
