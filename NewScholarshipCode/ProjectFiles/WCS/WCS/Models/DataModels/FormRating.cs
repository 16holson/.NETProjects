using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace WCS.Models
{
    public class FormRating
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        //Nav Prop
        public int StudentFormId { get; set; }
        [JsonIgnore]
        public StudentForm StudentForm { get; set; }

        [ForeignKey("Judge")]
        public string JudgeId { get; set; }
        public User Judge { get; set; }

        public int FormCriterionId { get; set; }

        [JsonIgnore]
        public FormCriterion FormCriterion { get; set; }
    }
}
