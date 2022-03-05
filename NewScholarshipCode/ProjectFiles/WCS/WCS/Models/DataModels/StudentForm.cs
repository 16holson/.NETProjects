using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class StudentForm
    {
        public int Id { get; set; }

        public int StudentProfileId { get; set; }
        [JsonIgnore]
        public StudentProfile StudentProfile { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        public int AwardCycleId { get; set; }
        [JsonIgnore]
        public AwardCycle AwardCycle { get; set; }

        public List<FormRating> FormRatings { get; set; }
        
        public List<FormResponse> StudentResponses { get; set; }
    }
}
