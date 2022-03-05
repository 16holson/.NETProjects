using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;

namespace WCS.Models
{
    public class StudentPack : IComparable<StudentPack>
    {
        public List<StudentFormPack> StudentFormPacks { get; set; }

        public StudentProfile Student { get; set; }

        public StudentRating StudentRating { get; set; }

        public int TotalApplicationsCount
        {
            get
            {
                return StudentFormPacks.Count;
            }
        }

        public int RatedApplicationsCount
        {
            get
            {
                return StudentFormPacks.Count(p => p.IsRatedFull);
            }
        }

        public bool IsFullyRated
        {
            get
            {
                return TotalApplicationsCount == RatedApplicationsCount;
            }
        }

        public StudentPack(StudentProfile studentProfile, StudentRating studentRating, List<StudentForm> studentForms, string judgeId, WcsContext context)
        {
            Student = studentProfile;
            StudentRating = studentRating ?? new StudentRating()
            {
                Id = 0,
                StudentProfileId = Student.Id,
                Rating = -1
            };

            StudentFormPacks = new List<StudentFormPack>();
            foreach (StudentForm studentForm in studentForms)
            {
                StudentFormPack pack = new StudentFormPack(studentForm.Form, studentForm, context);
                pack.FillRatingPacks(judgeId, context);
                StudentFormPacks.Add(pack);
            }
        }

        public int CompareTo(StudentPack other)
        {
            if (Student.LastName.Equals(other.Student.LastName))
                return Student.FirstName.CompareTo(other.Student.FirstName);
            else
                return Student.LastName.CompareTo(other.Student.LastName);
        }
    }
}
