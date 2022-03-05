using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class ScholarshipFund
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

        [NotMapped]
        [JsonIgnore]
        public double Remaining
        {
            get
            {
                try
                {
                    try
                    {
                        double amt = Amount;
                        foreach (ScholarshipAwardMoney awarded in AwardMonies)
                            try
                            {
                                amt -= awarded.Amount;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        return amt;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }

        //Nav Prop
        public int ScholarshipId { get; set; }
        [JsonIgnore]
        public Scholarship Scholarship { get; set; }

        public int AwardCycleId { get; set; }
        [JsonIgnore]
        public AwardCycle AwardCycle { get; set; }

        [JsonIgnore]
        public List<ScholarshipAwardMoney> AwardMonies { get; set; }
    }
}
