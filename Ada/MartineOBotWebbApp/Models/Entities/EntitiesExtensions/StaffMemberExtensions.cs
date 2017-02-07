using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace MartineOBotWebApp.Models.Entities
{
    public static class StaffMemberExtensions
    {
        public static bool IsAvailable(this StaffMember staffMember, DateTime date)
        {
            return !staffMember.Unavailabilities.Any(u => u.StarTime <= date && u.EndTime >= date);
        }
    }
}
