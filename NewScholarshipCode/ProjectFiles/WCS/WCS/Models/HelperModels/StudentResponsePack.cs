using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCS.Models
{
    /// <summary>
    /// Encapsulates a FormResponse with the related FormField
    /// </summary>
    public class StudentResponsePack
    {
        public FormField FormField { get; set; }

        public FormResponse FormResponse { get; set; }

        public StudentResponsePack(FormField field, FormResponse response)
        {
            FormField = field;
            FormResponse = response ?? new FormResponse { Id = 0, FormFieldId = field.Id, Response = "" };
        }
    }
}
