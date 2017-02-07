using System;

namespace MartineobotBridge
{
    public class PersonUpdateDto
    {
        public int RecognitionId { get; set; }
        public Guid PersonId { get; set; }
        public string FirstName { get; set; }
        public string ReasonOfVisit { get; set; }
    }
}
