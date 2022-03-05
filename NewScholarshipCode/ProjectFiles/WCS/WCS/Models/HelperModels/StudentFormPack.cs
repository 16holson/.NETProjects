using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;

namespace WCS.Models
{
    /// <summary>
    /// Encapsulates a StudentForm with the related Form
    /// Or a new StudentForm if the student does not have one yet
    /// </summary>
    public class StudentFormPack : IComparable<StudentFormPack>
    {
        public Form Form { get; set; }

        public StudentForm StudentForm { get; set; }

        public List<StudentResponsePack> StudentResponsePacks { get; set; }

        public List<StudentRatingPack> StudentRatingPacks { get; set; }

        /// <summary>
        /// Student has been rated by current judge
        /// </summary>
        public bool IsRatedFull { get; set; }

        /// <summary>
        /// Student has been rated on at least one criterion, but not all
        /// </summary>
        public bool IsRatedPartial { get; set; }

        public StudentFormPack(Form form, StudentForm studentForm, WcsContext context)
        {
            Form = form;
            StudentForm = studentForm ?? new StudentForm { Id = 0, FormId = form.Id, StudentResponses = new List<FormResponse>() };
            StudentResponsePacks = new List<StudentResponsePack>();
            StudentRatingPacks = new List<StudentRatingPack>();
            FillResponsePacks(context);
        }

        /// <summary>
        /// Fill StudentResponsePacks, which encapsulates each FormField with a FormResponse
        /// /// </summary>
        /// <param name="context"></param>
        public void FillResponsePacks(WcsContext context)
        {
            //load collections if necessary
            if (Form.FormFields == null)
                context.Entry(Form).Collection(f => f.FormFields).Load();
            Form.FormFields.Sort();

            if (StudentForm.StudentResponses == null)
                context.Entry(StudentForm).Collection(f => f.StudentResponses).Load();


            foreach (FormField field in Form.FormFields)
            {
                StudentResponsePacks.Add(new StudentResponsePack(
                    field,
                    StudentForm.StudentResponses.FirstOrDefault(r => r.FormFieldId == field.Id)
                    ));
            }
        }

        /// <summary>
        /// Fills ratingpacks for the current StudentForm criteria for a specific judge
        /// </summary>
        /// <param name="judgeId"></param>
        /// <param name="context"></param>
        public void FillRatingPacks(string judgeId, WcsContext context)
        {
            if (Form.FormCriteria == null)
                context.Entry(Form).Collection(f => f.FormCriteria).Load();
            Form.FormCriteria.Sort();

            if (StudentForm.FormRatings == null)
                context.Entry(StudentForm).Collection(f => f.FormRatings).Load();

            foreach (FormCriterion criterion in Form.FormCriteria)
            {
                StudentRatingPacks.Add(new StudentRatingPack(
                    StudentForm.Id,
                    criterion,
                    StudentForm.FormRatings.FirstOrDefault(r => r.StudentFormId == StudentForm.Id && r.FormCriterionId == criterion.Id && r.JudgeId == judgeId)
                    ));
            }

            //Mark as rated if criteria matches number of ratings
            IsRatedFull = true;
            IsRatedPartial = false;
            foreach (StudentRatingPack pack in StudentRatingPacks)
            {
                if (pack.FormRating.JudgeId == null)
                    IsRatedFull = false;
                else
                    IsRatedPartial = true;
            }
        }

        public int CompareTo(StudentFormPack other)
        {
            if (StudentForm.Id == other.StudentForm.Id)
                return Form.Title.CompareTo(other.Form.Title);
            else if (StudentForm.Id == 0)
                return -1;
            else if (other.StudentForm.Id == 0)
                return 1;
            else
                return Form.Title.CompareTo(other.Form.Title);
        }
    }
}
