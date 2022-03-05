using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using Microsoft.EntityFrameworkCore;

namespace WCS.Models
{
    public class ScholarshipCarrier
    {
        public List<Scholarship> Scholarships { get; set; }

        public ScholarshipCarrier(WcsContext context)
        {
            int currentAwardCycleId = Assistant.GetCurrentAwardCycle(context).Id;
            Scholarships = context.Scholarships.OrderBy(s => s.Name).Where(s => s.Active).ToList();
            List<ScholarshipFund> funds = context.ScholarshipFunds.Include(f => f.AwardMonies).Where(f => f.AwardCycleId == currentAwardCycleId).ToList();
            foreach (Scholarship s in Scholarships)
                s.ScholarshipFunds = funds.Where(f => f.ScholarshipId == s.Id).ToList();
        }

        public ScholarshipCarrier() => Scholarships = new List<Scholarship>();
    }
}
