using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using T3.Models;

namespace T3.Areas.Identity.Data
{
    public class AppUser:IdentityUser
    {
        [Required]
        public Guid TenantId { get; set; }// = Guid.Empty;

        [PersonalData]
        public DateTime DOB { get; set; }

        public Tenant Tenant{  get; set; }
        public int MyProperty { get; set; }
        //public ICollection<CourseAssignment> CourseAssignments { get; set; }
        public ICollection<GuardianRelation> GuardianRelations { get; set; }

    }
}
