using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class ReportViewModel
    {
        public AwardCycle AwardCycle { get; set; }

        public List<Scholarship> Scholarships { get; set; }
    }
}
