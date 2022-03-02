using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class StudentRatingPack
    {
        public FormCriterion FormCriterion { get; set; }

        public FormRating FormRating { get; set; }

        public StudentRatingPack(int studentFormId, FormCriterion criterion, FormRating rating)
        {
            FormCriterion = criterion;
            FormRating = rating ?? new FormRating()
            {
                Id = 0,
                FormCriterionId = criterion.Id,
                StudentFormId = studentFormId,
                JudgeId = null,
                Rating = -1,
            };
        }
    }
}
