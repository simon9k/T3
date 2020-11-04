using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T3.Areas.Identity.Data;

namespace T3.Models
{
    //The Class is for responsible teachers/coaches
    //todo should design a delegat class for teachers/coaches, directly using appUser is not a good idea
    public class CourseAssignment
    {
        public string AppUserId { get; set; }//becarefull, it's not Guid
        public int CourseId { get; set; }

        //**** this could lead EF update-database failed cause of FK_** on cascade delete circle or multiple
        //public Guid TenantId { get; set; }  
        //public Tenant Tenant { get; set; }

        public AppUser AppUser { get; set; }
        public Course Course { get; set; }


        public bool bPrimary { get; set; } //主讲老师？或者协助
        public int FinishState { get; set; } //完成、缺席？



    }
}