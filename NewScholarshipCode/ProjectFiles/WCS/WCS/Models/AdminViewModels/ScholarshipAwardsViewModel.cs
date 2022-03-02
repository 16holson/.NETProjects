using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class ScholarshipAwardsViewModel
    {
        public List<Scholarship> Scholarships { get; set; }

        public ScholarshipAwardsListModel AwardsListModel { get; set; }

        public StudentRatingsListPack StudentRatingsListPack { get; set; }

        public AwardCycle AwardCycle { get; set; }
    }
}
