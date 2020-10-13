using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using T3.Areas.Identity.Data;

namespace T3.Models
{
    public class Course
    {
        public int CourseId { set; get; }
        public int? OriginCourseId { set; get; }
        public string Name { get; set; }
        [Required]
        public Guid TanentId { get; set; }

        [Display(Name = "每周")]
        [DefaultValue(false)]
        public bool IsCyclic { set; get; }

        [Display(Name = "开始时间")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { set; get; }
        [Display(Name = "结束时间")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { set; get; }

        //[Display(Name = "学生姓名")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "学生姓名 不能为空")]
        //public string StudentName { set; get; }
        
        public Tenant Tenant { get; set; }


        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseAssignment> CourseAssignments { get; set; }

    }
}
