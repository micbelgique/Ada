using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechOfficeSDK.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Picture { get; set; }
        public int RoomStatusId { get; set; }
        
        public string displayStatus()
        {
            string status;

            switch(RoomStatusId)
            {
                case 1: status = "Libre"; break;
                case 2: status = "Reservée"; break;
                case 3: status = "Occupée"; break;
                default:  status = "Inconnu"; break;
            }

            return status;
        }
        public int DoUndo()
        {
            switch (RoomStatusId)
            {
                case 1: RoomStatusId = 2; break;
                case 2: RoomStatusId = 1; break;
            }

            return RoomStatusId;
        }

    }
}
