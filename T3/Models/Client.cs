using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace T3.Models
{
    public class Client
    {
        public int ClientID { get; set; }
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

        public Tenant Tenant { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }

        //[Display(Name = "班级")]
        //public int? ClassID { get; set; }
        //public virtual Class Class { get; set; }

        //public virtual ICollection<PupilLog> PupilLogs { get; set; }

    }
}
