using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;

namespace WCS.Models
{
    public class ScholarshipAwardsListModel
    {
        public bool AllowEdit { get; set; }

        public List<ScholarshipAward> Awards { get; set; }

        /// <summary>
        /// Fills the data for the award cycle selected 
        /// </summary>
        /// <param name="context"></param>
        public void Fill(WcsContext context, int awardCycleId = 0)
        {
            if (awardCycleId == 0)
            {
                awardCycleId = Assistant.GetCurrentAwardCycle(context).Id;
                AllowEdit = true;
            }
            else
            {
                AllowEdit = awardCycleId == Assistant.GetCurrentAwardCycle(context).Id;
            }

            List<ScholarshipAwardMoney> monies = context.ScholarshipAwardMonies
                .Include(m => m.ScholarshipFund)
                .Where(m => m.ScholarshipAward.AwardCycleId == awardCycleId)
                .ToList();

            Awards = context.ScholarshipAwards
                .OrderBy(a => a.Scholarship.Name)
                .Include(a => a.Scholarship)
                .Include(a => a.StudentProfile)
                .Where(a => a.AwardCycleId == awardCycleId)
                .ToList();

            foreach (ScholarshipAward award in Awards)
                award.AwardMonies = monies.Where(m => m.ScholarshipAwardId == award.Id).ToList();
        }

        public ScholarshipAwardsListModel() { }

        /// <summary>
        /// Constructor that fills data for the selected award cycle, or the current one if awardcycle is not passed
        /// </summary>
        /// <param name="context"></param>
        /// <param name="awardCycleId"></param>
        public ScholarshipAwardsListModel(WcsContext context, int awardCycleId = 0)
        {
            Fill(context, awardCycleId);
        }
    }
}
