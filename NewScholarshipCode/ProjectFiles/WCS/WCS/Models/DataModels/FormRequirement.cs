using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using WCS.Services;

namespace WCS.Models
{
    public class FormRequirement : IRecycle
    {
        public int Id { get; set; }

        public string ProfileField { get; set; }

        public string Operator { get; set; }

        public string Value { get; set; }

        //Navigation Properties
        public int? FormId { get; set; }

        [JsonIgnore]
        public Form Form { get; set; }

        #region IRecycle Members

        public bool Recycled { get; set; }

        public RecycledItem GetRecycledItem(WcsContext context)
        {
            return context.RecycleBin.FirstOrDefault(i => i.ItemType == ItemType.FormRequirement && i.ItemId == Id);
        }

        public Task PermanentDeleteAsync(WcsContext context)
        {
            context.FormRequirements.Remove(this);
            context.RecycleBin.Remove(GetRecycledItem(context));
            return context.SaveChangesAsync();
        }

        public Task RecycleAsync(WcsContext context, User recycledBy)
        {
            if (Form == null)
                Form = context.Forms.FirstOrDefault(f => f.Id == FormId);

            context.RecycleBin.Add(new RecycledItem()
            {
                ItemType = ItemType.FormRequirement,
                ItemName = $"<span class='recycle-item-formname'>{ Form.Title }</span>: { ProfileField } { Operator } { Value }",
                ItemId = Id,
                ItemDependency = FormId,
                RecycledBy = $"{recycledBy.FullName} ({ recycledBy.UserName })"
            });
            FormId = null;
            context.Database.ExecuteSqlCommand($"UPDATE dbo.FormRequirements SET FormId = null WHERE Id = { Id }");
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
