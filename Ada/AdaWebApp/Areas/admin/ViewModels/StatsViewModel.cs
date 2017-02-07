using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.DAL.Repositories;
using AdaWebApp.Models.Entities;

namespace AdaWebApp.Areas.Admin.ViewModels
{
    public class StatsViewModel
    {

        [Display(Name = "Total Visitors")]
        public int TotalVisitors { get; set; }

        [Display(Name = "Average Age")]
        public double AverageAge { get; set; }

        public double Male { get; set; }

        public double Female { get; set; }

        [Display(Name = "Average Mood")]
        public EmotionScores EmotionScores { get; set; }
    }
}
