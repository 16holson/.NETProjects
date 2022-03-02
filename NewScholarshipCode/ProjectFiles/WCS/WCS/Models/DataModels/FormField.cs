using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WCS.Data;
using WCS.Services;

namespace WCS.Models
{
    public class FormField : IComparable<FormField>, IRecycle
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public bool Required { get; set; }

        public string Options { get; set; }

        public int Order { get; set; }

        //Nav Prop
        public int? FormId { get; set; }
        [JsonIgnore]
        public Form Form { get; set; }

        public List<FormResponse> FormResponses { get; set; }

        public int CompareTo(FormField other)
        {
            return Order.CompareTo(other.Order);
        }

        #region IRecycle Members

        public bool Recycled { get; set; }

        public RecycledItem GetRecycledItem(WcsContext context)
        {
            return context.RecycleBin.FirstOrDefault(i => i.ItemType == ItemType.FormField && i.ItemId == Id);
        }

        public Task PermanentDeleteAsync(WcsContext context)
        {
            context.FormFields.Remove(this);
            context.RecycleBin.Remove(GetRecycledItem(context));
            return context.SaveChangesAsync();
        }

        public Task RecycleAsync(WcsContext context, User recycledBy)
        {
            if (Form == null)
                Form = context.Forms.FirstOrDefault(f => f.Id == FormId);

            context.RecycleBin.Add(new RecycledItem()
            {
                ItemType = ItemType.FormField,
                ItemName = $"<span class='recycle-item-formname'>{ Form.Title }:</span> ( { Type } ) { Title }",
                ItemId = Id,
                ItemDependency = FormId,
                RecycledBy = $"{ recycledBy.FullName } ({ recycledBy.UserName })"
            });
            FormId = null;
            //Have to do a manual query to detach the field from the form, because EF is a little bitch about it
            context.Database.ExecuteSqlCommand($"UPDATE dbo.FormFields SET FormId = null WHERE Id = { Id }");
            Recycled = true;
            return Task.CompletedTask;
        }

        public Task RestoreAsync(WcsContext context)
        {
            RecycledItem item = GetRecycledItem(context);
            FormId = item.ItemDependency;
            context.Entry(this).Property(f => f.FormId).IsModified = true;
            context.RecycleBin.Remove(item);
            Recycled = false;
            context.Entry(this).State = EntityState.Modified;
            return context.SaveChangesAsync();
        }

        #endregion
    }
}
