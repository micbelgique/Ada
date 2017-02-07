using System;
using System.ComponentModel.DataAnnotations;
using MartineOBotWebApp.Models.Entities;

namespace MartineOBotWebApp.Areas.Admin.ViewModels
{
    public class UnavailabilityViewModel
    {
        public int StaffMemberId { get; set; }

        public int Duration { get; set; }

        private DateTime _startTime;

        [Display(Name = "Start Time")]
        [DataType(DataType.DateTime)]
        public DateTime StarTime
        {
            get { return Duration != default(int) ? DateTime.UtcNow : _startTime; }
            set { _startTime = value; }
        }

        private DateTime _endTime;

        [Display(Name = "End Time")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime
        {
            get { return Duration != default(int) ? DateTime.UtcNow.AddMinutes(Duration) : _endTime;  }
            set { _endTime = value; }
        }
    }
}
