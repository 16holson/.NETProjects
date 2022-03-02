using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class JudgeByStudentViewModel
    {
        public AwardCycle AwardCycle { get; set; }

        public List<StudentPack> StudentPacks { get; set; }
    }
}
