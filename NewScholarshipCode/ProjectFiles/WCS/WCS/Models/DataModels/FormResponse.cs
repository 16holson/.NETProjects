using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    public class FormResponse
    {
        public int Id { get; set; }

        public string Response { get; set; }

        //Nav Prop
        public int StudentFormId { get; set; }
        [JsonIgnore]
        public StudentForm StudentForm { get; set; }

        public int FormFieldId { get; set; }
        [JsonIgnore]
        public FormField FormField { get; set; }
    }
}
