using System;

namespace AdaSDK
{
    public class PersonVisitDto
    {
        public int PersonId  { get; set; }
        public string FirstName { get; set; }
        public DateTime DateVisit { get; set; }
        public GenderValues Gender { get; set; }

        public override string ToString()
        {
            return $"Person id : {PersonId}\n" +
                   $"FirstName : {FirstName}\n";
        }
    }
}
