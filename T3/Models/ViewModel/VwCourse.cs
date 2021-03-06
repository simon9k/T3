﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace T3.Models.ViewModel
{
    public class VwCourse
    {
        //todo Change 区域？ validation show chinese? System.Globalization.CultureInfo
        //firstly for Course Create
        public int id { get; set; }
        [StringLength(20, MinimumLength = 4)]
        public string Name { get; set; }
        [Display(Name = "每周")]
        [DefaultValue(false)]
        public bool IsCyclic { set; get; }

        //仅仅日期，no hour/minute info
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { set; get; } = DateTime.Now;

        [Display(Name = "开始时间")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:MM}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { set; get; }
        [Display(Name = "结束时间")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:MM}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { set; get; }

        [Display(Name = "课时")]
        [Range(10, 480, ErrorMessage = "课时长 {0} 需在 {1} 和 {2}分钟之间.")]
        public int Duration { get; set; } = 30;

        //[Display(Name = "学生姓名")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "学生姓名 不能为空")]
        //public string StudentName { set; get; }

        //public List<int> Instructors { get; set; }
        //public List<Instructor> Instructors { get; set; }
        ////a string for temporary storage of the instructors' id list
        //public string InstructorsList { get; set; }

        //for tokenize2 multipile select
        [Display(Name = "教练")]
        [Required(ErrorMessage = "需选择教练")]
        public int[] InstructorsIdList { get; set; }

        public SelectList InstructorsSelectList { get; set; }
        
        //public List<string> MultiInstructors { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseAssignment> CourseAssignments { get; set; }

    }
    public class Instructor 
    {

        public int Id { get; set; }
        public string Name { get; set; }
        //else info for necessary in future
        public bool Checked { get; set; }


    }

}
