using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using T3.Areas.Identity.Data;

namespace T3.Models
{
    //********Student vs AppUser
    //We can treat Student like somekind of complementary or detail info for some AppUser
    //and This AppUser have Role of Guardian(监护人）
    //and multi-zero to multi-zero
    public class Student:Person
    {
       
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<GuardianRelation> GuardianRelations { get; set; }

        

    }

    //Student/client connect to AppUser 
    public class GuardianRelation
    {
        public string AppUserId { get; set; }//becarefull, it's not Guid
        public int Id { get; set; }

        public AppUser AppUser { get; set; }
        public Student Student { get; set; }

        public GuardianType guardianType { get; set; }

    }

    public enum GuardianType
    {
        Father, Mother, GrandPa, GrandMa,Self, Other
    }

}
