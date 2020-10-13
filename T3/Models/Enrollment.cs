using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T3.Models
{
    //todo use generic combine Client & Teacher Enrollment
    public class Enrollment
    {
        //public int EnrollmentId { get; set; } 
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        //Introducing FOREIGN KEY constraint 'FK_Enrollment_Tenants_TenantId' on table 'Enrollment' may cause cycles or multiple cascade paths.
        //maybe because there are have two cascading delete paths 
        //from Tenant to Enrollment - which causes the exception.
        //how to get all Enrollment in this Tenant? 
        //   **No need to do this
        //public Guid TenantId { get; set; }
        //public Tenant Tenant { get; set; }


        public Student Student { get; set; }
        public Course Course { get; set; }



        public DateTime DateTime { get; set; }
        public int FinishState { get; set; } //完成、缺席？


    }
}
