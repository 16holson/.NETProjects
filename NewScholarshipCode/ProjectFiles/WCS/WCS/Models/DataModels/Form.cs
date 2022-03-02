using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using WCS.Services;

namespace WCS.Models
{
    public class Form : IRecycle
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IncludeStudentRating { get; set; }

        public bool Active { get; set; }

        //Navigation Properties
        [JsonIgnore]
        public List<StudentForm> StudentForms { get; set; }

        public List<FormRequirement> FormRequirements { get; set; }

        public List<FormCriterion> FormCriteria { get; set; }

        public List<FormField> FormFields { get; set; }

        #region IRecycle Members

        public bool Recycled { get; set; }

        public RecycledItem GetRecycledItem(WcsContext context)
        {
            return context.RecycleBin.FirstOrDefault(i => i.ItemType == ItemType.Form && i.ItemId == Id);
        }

        public Task PermanentDeleteAsync(WcsContext context)
        {
            context.Forms.Remove(this);
            context.RecycleBin.Remove(GetRecycledItem(context));
            return context.SaveChangesAsync();
        }

        public Task RecycleAsync(WcsContext context, User recycledBy)
        {
            context.RecycleBin.Add(new RecycledItem()
            {
                ItemType = ItemType.Form,
                ItemName = Title,
                ItemId = Id,
                RecycledBy = $"{recycledBy.FullName} ({ recycledBy.UserName })"
            });
            Recycled = true;
            Active = false;
            return context.SaveChangesAsync();
        }

        public Task RestoreAsync(WcsContext context)
        {
            context.RecycleBin.Remove(GetRecycledItem(context));
            Recycled = false;
            return context.SaveChangesAsync();
        }

        #endregion
    }
}
