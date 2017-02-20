using System;

namespace AdaSDK
{
    public class PersonDto
    {
        public int RecognitionId { get; set; }
        public Guid PersonId  { get; set; }
        public string FirstName { get; set; }
        public bool IsRecognized { get; set; }
        public int NbPasses { get; set; }
        public string ReasonOfVisit { get; set; }

        public int Age { get; set; }
        public GenderValues Gender { get; set; }

        public override string ToString()
        {
            return $"Recognition id : {RecognitionId}\n" +
                   $"Person id : {PersonId}\n" +
                   $"FirstName : {FirstName}\n" +
                   $"Gender : {Enum.GetName(typeof(GenderValues), Gender)}\n" +
                   $"Is recognized : {IsRecognized}\n" +
                   $"Number of passases : {NbPasses}\n" +
                   $"Reason of visit : {ReasonOfVisit}";
        }
    }
}
