using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WCS.Services;
using WCS.Data;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace WCS.Models
{
    public class RecycledItem
    {
        public int Id { get; set; }

        [Required]
        public ItemType ItemType { get; set; }

        [Required]
        public string ItemName { get; set; }

        [Required]
        public int ItemId { get; set; }

        public int? ItemDependency { get; set; }

        [Display(Name = "Recycled Date")]
        public DateTime RecycledDate { get; set; }

        public string RecycledBy { get; set; }

        public RecycledItem() => RecycledDate = DateTime.Now;

        public IRecycle GetItem(WcsContext context)
        {
            switch (ItemType)
            {
                case ItemType.Form: return context.Forms.FirstOrDefault(i => i.Id == ItemId) as IRecycle;
                case ItemType.AwardCycle: return context.AwardCycles.FirstOrDefault(i => i.Id == ItemId) as IRecycle;
                case ItemType.FormRequirement: return context.FormRequirements.FirstOrDefault(i => i.Id == ItemId) as IRecycle;
                case ItemType.FormCriterion: return context.FormCriteria.FirstOrDefault(i => i.Id == ItemId) as IRecycle;
                case ItemType.FormField: return context.FormFields.FirstOrDefault(i => i.Id == ItemId) as IRecycle;
                default: return null;
            }
        }

        public Task RestoreItemAsync(WcsContext context)
        {
            return GetItem(context).RestoreAsync(context);
        }

        public Task PermanentlyDeleteItemAsync(WcsContext context)
        {
            return GetItem(context).PermanentDeleteAsync(context);   
        }
    }
    

    public enum ItemType
    {
        Form,
        AwardCycle,
        FormRequirement,
        FormCriterion,
        FormField
    }
}

