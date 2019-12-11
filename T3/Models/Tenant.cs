using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T3.Areas.Identity.Data;

namespace T3.Models
{
    public class Tenant
    {
        public Guid TenantId { set; get; } = Guid.Empty;// NewGuid();
        public string Name { set; get; }
        public DateTime EnrollmentDate { get; set; }
        public string Location { get; set; }
        public string Desctiption { get; set; }
        
        
        public ICollection<AppUser> Users { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Client> Clients { get; set; }
        //public ICollection<Enrollment> Enrollments { get; set; }
        //public ICollection<CourseAssignment> CourseAssignments { get; set; }


    }
}
