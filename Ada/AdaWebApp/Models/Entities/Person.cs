using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdaBridge;

namespace AdaWebApp.Models.Entities
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Index]
        public Guid PersonApiId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        [DisplayName("Age")]
        public DateTime DateOfBirth { get; set; }

        public GenderValues Gender { get; set; }

        public int MaleCounter { get; set; }
        public int FemaleCounter { get; set; }

        // Foreign keys
        public virtual IList<Visit> Visits { get; set; }

        public Person(){
            Visits = new List<Visit>(); 
        }
    }
}
