using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{ 
    public class JudgeByFormViewModel
    {
        public AwardCycle AwardCycle { get; set; }

        public int SelectedFormId { get; set; }
        public FormPack SelectedForm { get; set; }

        public List<FormPack> FormPacks { get; set; }

        public List<StudentPack> StudentPacks { get; set; }
    }
}
