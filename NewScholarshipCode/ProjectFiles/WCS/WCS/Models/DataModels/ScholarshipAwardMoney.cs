using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class ScholarshipAwardMoney
    {
        public int Id { get; set; }

        public double Amount { get; set; }

        //Nav
        public int ScholarshipAwardId { get; set; }
        [JsonIgnore]
        public ScholarshipAward ScholarshipAward { get; set; }

        public int ScholarshipFundId { get; set; }
        [JsonIgnore]
        public ScholarshipFund ScholarshipFund { get; set; }
    }
}
