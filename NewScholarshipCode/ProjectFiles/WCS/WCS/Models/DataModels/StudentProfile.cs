using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Data;

namespace WCS.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }

        public User Student { get; set; }

        public List<StudentForm> StudentForms { get; set; }

        public List<StudentRating> StudentRatings { get; set; }

        #region Personal Data

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "W Number")]
        public string WNumber { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }

        }

        public string Prefix { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Address { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }
        #endregion

        #region Academic
        [Required]
        [Display(Name = "Overall GPA")]
        [Range(0, 4.0)]
        public float OverallGPA { get; set; }

        [Required]
        [Display(Name = "Major GPA")]
        [Range(0, 4.0)]
        public float? MajorGPA { get; set; }

        [Display(Name = "ACT Score")]
        public int? ActScore { get; set; }
        #endregion

        #region Educational
        [Display(Name = "Current Major")]
        public string CurrentMajor { get; set; }

        [Display(Name = "Future Major")]
        public string FutureMajor { get; set; }

        [Display(Name = "Current Academic Level")]
        public string CurrentAcademicLevel { get; set; }

        [Display(Name = "Ultimate Degree Goal")]
        public string UltimateDegreeGoal { get; set; }

        [Display(Name = "High School Attended")]
        public string HighSchool { get; set; }

        [Display(Name = "Last University Attended")]
        public string LastUniversity { get; set; }

        [Display(Name = "First WSU Semester")]
        public string FirstWsuSemester { get; set; }

        [Display(Name = "First WSU Year")]
        public string FirstWsuYear { get; set; }

        [Display(Name = "Current Schedule Status")]
        public string CurrentScheduleStatus { get; set; }
        #endregion

        #region Courses
        [Display(Name = "Past CS Courses Taken")]
        public List<StudentCourse> PastCourses { get; set; }

        [Display(Name = "AP Tests Passed")]
        public string PassedApTests { get; set; }

        [NotMapped]
        public IEnumerable<string> ApTestList { get; set; }

        [NotMapped]
        [Display(Name = "Concurrent Enrollment Courses Taken")]
        public List<StudentCourse> ConcurrentCourses
        {
            get
            {
                try
                {
                    return PastCourses.Where(c => c.Course.ConcurrentCourse).ToList();
                }
                catch (Exception)
                {
                    return new List<StudentCourse>();
                }
            }
        }
        #endregion

        #region Extracurricular
        [Display(Name = "Clubs and/or Organizations")]
        public string ClubAndOrganizationHistory { get; set; }

        [Display(Name = "Honors and/or Awards")]
        public string AwardsHistory { get; set; }

        [Display(Name = "CS Topics Of Interest")]
        public string CsInterest { get; set; }

        [Display(Name = "Past Scholarships and/or Financial Aid")]
        public string ScholarshipAidHistory { get; set; }

        [Display(Name = "Greatest Achievements")]
        public string AchievementsHistory { get; set; }
        #endregion


        #region OtherThings

        [NotMapped]
        [Display(Name = "CS Courses Completed")]
        public IEnumerable<string> StudentCourses { get; set; }

        [NotMapped]
        public List<SelectListItem> AvailableCourses { get; set; }

        public void FillAvailableCourses(WcsContext context)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (Course course in context.Courses.ToList())
            {
                items.Add(new SelectListItem
                {
                    Value = course.Id.ToString(),
                    Text = course.CourseNumber + " - " + course.CourseName,
                    Selected = context.StudentCourses.Any(c => c.StudentId == Id && c.CourseId == course.Id)
                });
            }

            AvailableCourses = items;
        }

        public void GetListOfStudentCourses(WcsContext context)
        {
            List<StudentCourse> studentCourses = context.StudentCourses.Include(c => c.Course).Where(sc => sc.StudentId == Id).ToList();
            List<string> courses = new List<string>();
            foreach(StudentCourse sc in studentCourses)
            {
                courses.Add(sc.Course.CourseNumber);
            }

            StudentCourses = courses;
        }

        [NotMapped]
        public List<StudentForm> FormsForCycle { get; set; }

        /// <summary>
        /// Fill data for "FormsForCycle" property
        /// </summary>
        /// <param name="awardCycleId"></param>
        /// <returns>Number of forms added</returns>
        public int FillFormsForCycle(int awardCycleId, WcsContext context)
        {
            if (StudentForms == null)
                context.Entry(this).Collection(c => c.StudentForms).Load();

            FormsForCycle = StudentForms.Where(f => f.AwardCycleId == awardCycleId).ToList();

            return FormsForCycle == null ? 0 : FormsForCycle.Count;
        }

        #endregion

        #region Nav Properties 

        [JsonIgnore]
        public List<ScholarshipAward> ScholarshipAwards { get; set; }

        #endregion
    }

    public class StudentCourse
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required]
        [Display(Name = "Course Number")]
        public string CourseNumber { get; set; }

        [Display(Name = "Is a Concurrent Course")]
        public bool ConcurrentCourse { get; set; }
    }
}
