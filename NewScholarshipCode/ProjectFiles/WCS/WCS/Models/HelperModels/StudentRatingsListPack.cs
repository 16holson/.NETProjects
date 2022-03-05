using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using Microsoft.EntityFrameworkCore;

namespace WCS.Models
{
    public class StudentRatingsListPack
    {
        public List<FormRatingsListPack> FormRatingsListPacks { get; set; }

        public List<User> AllJudges { get; set; }

        public List<StudentProfile> allStudentProfiles { get; set; }

        public StudentRatingsListPack() { }

        public StudentRatingsListPack(WcsContext context)
        {
            Fill(context);
        }

        public void Fill(WcsContext context)
        {
            int currentCycleId = Assistant.GetCurrentAwardCycle(context).Id;

            List<Form> forms = (from f in context.Forms.OrderBy(f => f.Title).Include(f => f.StudentForms).Where(f => f.Active).ToList()
                                where f.StudentForms.Any(s => s.AwardCycleId == currentCycleId)
                                select f).ToList();

            allStudentProfiles = context.StudentProfiles.ToList();
            List<StudentCourse> allStudentCourses = context.StudentCourses.Include(c => c.Course).ToList();
            foreach (StudentProfile studentProfile in allStudentProfiles)
            {
                studentProfile.StudentCourses =
                        from c in allStudentCourses
                        where c.StudentId == studentProfile.Id
                        select c.Course.CourseNumber;
            }

            List<StudentRating> allStudentRatings = context.StudentRatings
                .Include(s => s.Judge)
                .Where(s => s.AwardCycleId == currentCycleId).ToList();

            List<FormRating> allFormRatings = context.FormRatings
                .Include(f => f.Judge)
                .Include(f => f.FormCriterion)
                .Include(f => f.StudentForm)
                .Where(f => f.StudentForm.AwardCycleId == currentCycleId).ToList();

            List<ScholarshipAward> allAwards = context.ScholarshipAwards.Where(a => a.AwardCycleId == currentCycleId).ToList();

            AllJudges = (from j in context.Users.ToList()
                         where allFormRatings.Any(r => r.JudgeId == j.Id)
                         select j).ToList();

            FormRatingsListPacks = new List<FormRatingsListPack>();

            foreach (Form form in forms)
            {
                form.StudentForms = form.StudentForms.Where(f => f.AwardCycleId == currentCycleId).ToList();

                FormRatingsListPack newformpack = new FormRatingsListPack()
                {
                    Form = form,
                    StudentRatingListItemPacks = new List<StudentRatingListItemPack>()
                };

                foreach (StudentForm sForm in form.StudentForms)
                {
                    StudentRatingListItemPack newListItemPack = new StudentRatingListItemPack()
                    {
                        StudentProfile = allStudentProfiles.FirstOrDefault(p => p.Id == sForm.StudentProfileId),
                        HasAward = allAwards.Any(a => a.StudentProfileId == sForm.StudentProfileId),
                        IncludeStudentRating = form.IncludeStudentRating,
                        StudentRatings = allStudentRatings.Where(r => r.StudentProfileId == sForm.StudentProfileId).ToList(),
                        StudentFormRatings = allFormRatings.Where(f => f.StudentFormId == sForm.Id).ToList()
                    };

                    newformpack.StudentRatingListItemPacks.Add(newListItemPack);
                }

                newformpack.StudentRatingListItemPacks.Sort();
                FormRatingsListPacks.Add(newformpack);
            }
        }
    }

    public class FormRatingsListPack
    {
        public Form Form { get; set; }

        public List<StudentRatingListItemPack> StudentRatingListItemPacks { get; set; }
    }

    public class StudentRatingListItemPack : IComparable<StudentRatingListItemPack>
    {
        public StudentProfile StudentProfile { get; set; }

        public bool IncludeStudentRating { get; set; }

        public bool HasAward { get; set; }

        public List<StudentRating> StudentRatings { get; set; }

        public List<FormRating> StudentFormRatings { get; set; }

        public string AverageRating
        {
            get
            {
                double studentratingtotal = StudentRatings.Sum(r => r.Rating);
                double formratingtotal = StudentFormRatings.Sum(r => r.Rating);
                double totalrating = formratingtotal + (IncludeStudentRating ? studentratingtotal : 0);
                double ratingcount = StudentFormRatings.Count + (IncludeStudentRating ? StudentRatings.Count : 0);

                if (ratingcount < 1)
                    return "0.00";
                else
                    return Math.Round(totalrating / ratingcount, 2).ToString("0.00");
            }
        }

        public int CompareTo(StudentRatingListItemPack other)
        {
            return other.AverageRating.CompareTo(AverageRating);
        }
    }
}
