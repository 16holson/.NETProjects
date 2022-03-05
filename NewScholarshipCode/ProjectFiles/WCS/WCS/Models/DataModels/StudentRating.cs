using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class StudentRating
    {
        public int Id { get; set; }

        public int StudentProfileId { get; set; }
        [JsonIgnore]
        public StudentProfile StudentProfile { get; set; }

        public int AwardCycleId { get; set; }
        [JsonIgnore]
        public AwardCycle AwardCycle { get; set; }

        [ForeignKey("Judge")]
        public string JudgeId { get; set; }
        [JsonIgnore]
        public User Judge { get; set; }

        public int Rating { get; set; }
    }
}
