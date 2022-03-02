using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class RecycleBinViewModel
    {
        public string StatusMessage { get; set; }

        public IEnumerable<RecycledItem> RecycledItems { get; set; }
    }
}
