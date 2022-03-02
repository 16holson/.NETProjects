using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace WCS.Models
{
    public class Scholarship
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        [NotMapped]
        [JsonIgnore]
        public double FundingTotal
        {
            get
            {
                try
                {
                    return ScholarshipFunds.Sum(i => i.Amount);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        [JsonIgnore]
        public double FundingRemaining
        {
            get
            {
                try
                {
                    return ScholarshipFunds.Sum(i => i.Remaining);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public List<ScholarshipFund> ScholarshipFunds { get; set; }

        public List<ScholarshipAward> ScholarshipAwards { get; set; }
    }
}
