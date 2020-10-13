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
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        [Display(Name = "姓名")]
        public string Name { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "学名")]
        public string NickName { get; set; }
        [Display(Name = "姓名(学名)")]
        public string FullName
        {
            get
            {
                return Name + "(" + NickName+")";
            }
        }
        [Display(Name = "性别")]
        public string Sex { get; set; }
        [Display(Name = "出生日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BOD { get; set; }//Birth of Date
        [Display(Name = "注册日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EOD { get; set; } = DateTime.Now;//Enrollment Of Date
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]//todo get the right RegEx for chinese ID
        public string PersonalID { get; set; }

        //***再次发生： FOREIGN KEY constraint 'FK_' on table  may cause cycles or multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
        //   *Update-database 发生此错误时，可以忽略，不影响系统运行
        //   *具体分析也影响不大，Student与Tenant的外键没有建立，后果就是可能造成垃圾数据
        //   *分析原因，应该是在Tenant之下，AppUser/Course/Student,形成了环形的关系
        //   *这是sql报出的exception，据说其它db不报这种异常 
        //   * 能够改变吗？
        public Tenant Tenant { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<GuardianRelation> GuardianRelations { get; set; }

        //[Display(Name = "班级")]
        //public int? ClassID { get; set; }
        //public virtual Class Class { get; set; }

        //public virtual ICollection<PupilLog> PupilLogs { get; set; }

    }

    public class GuardianRelation
    {
        public string AppUserId { get; set; }//becarefull, it's not Guid
        public int StudentId { get; set; }

        public AppUser AppUser { get; set; }
        public Student Student { get; set; }

        public GuardianType guardianType { get; set; }

    }

    public enum GuardianType
    {
        Father, Mother, GrandParent, Self, Other
    }

}
