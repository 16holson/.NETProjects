using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using Microsoft.EntityFrameworkCore;

namespace WCS.Models
{
    /// <summary>
    /// Special pack for the scholarshipaward widget
    /// </summary>
    public class ScholarshipAwardEditorPack
    {
        public List<ScholarshipStudent> Students { get; set; }

        public List<Scholarship> Scholarships { get; set; }

        public ScholarshipAwardEditorPack(WcsContext context)
        {
            Students = new List<ScholarshipStudent>();
            Scholarships = new List<Scholarship>();
            int currentCycleId = Assistant.GetCurrentAwardCycle(context).Id;

            List<StudentProfile> allStudents = (from s in context.StudentProfiles.Include(p => p.StudentForms)
                                                where s.StudentForms.Any(f => f.AwardCycleId == currentCycleId)
                                                select s).ToList();

            List<ScholarshipAward> allAwards = context.ScholarshipAwards.Include(a => a.AwardMonies).Include(a => a.Scholarship).Where(a => a.AwardCycleId == currentCycleId).ToList();
            Scholarships = context.Scholarships.OrderBy(s => s.Name).Include(s => s.ScholarshipFunds).Where(s => s.Active).ToList();
            foreach(Scholarship s in Scholarships)
                s.ScholarshipFunds = s.ScholarshipFunds.Where(f => f.AwardCycleId == currentCycleId).ToList();

            string formatstring = "";
            if (allStudents != null && allStudents.Count > 0)
            {
                //Get max digits for formatting
                int max = allStudents.Max(s => s.Id);
                for (int i = 0; i < Math.Floor(Math.Log10(max) + 1); i++)
                    formatstring += "0";
            }
            else
                formatstring += "0";


            //Generate student list
            foreach (StudentProfile student in allStudents)
            {
                Students.Add(new ScholarshipStudent()
                {
                    Id = student.Id,
                    FullName = student.Id.ToString(formatstring) + " -- " + student.FullName,
                    AwardedScholarship = allAwards.FirstOrDefault(a => a.StudentProfileId == student.Id)
                });
            }

            Students.Sort();
        }
    }

    public class ScholarshipStudent : IComparable<ScholarshipStudent>
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public ScholarshipAward AwardedScholarship { get; set; }

        public int CompareTo(ScholarshipStudent other)
        {
            return FullName.Substring(FullName.IndexOf("--")).CompareTo(other.FullName.Substring(other.FullName.IndexOf("--")));
        }
    }
}
