using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using WCS.Services;
using Microsoft.AspNetCore;

namespace WCS.Models
{
    public class AwardCycle : IRecycle
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string CycleName { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public CycleStatus Status { get; set; }

        //Navigation Propertires
        public List<StudentForm> StudentForms { get; set; }

        public List<ScholarshipAward> ScholarshipAwards { get; set; }

        public List<ScholarshipFund> ScholarshipFunds { get; set; }

        #region IRecycle Members

        public bool Recycled { get; set; }

        public RecycledItem GetRecycledItem(WcsContext context)
        {
           return context.RecycleBin.FirstOrDefault(i => i.ItemType == ItemType.AwardCycle && i.ItemId == Id);
        }

        public Task PermanentDeleteAsync(WcsContext context)
        {
            context.AwardCycles.Remove(this);
            context.RecycleBin.Remove(GetRecycledItem(context));
            return context.SaveChangesAsync();
        }

        public Task RecycleAsync(WcsContext context, User recycledBy)
        {
            context.RecycleBin.Add(new RecycledItem()
            {
                ItemType = ItemType.AwardCycle,
                ItemName = CycleName,
                ItemId = Id,
                RecycledBy = $"{recycledBy.FullName} ({ recycledBy.UserName })"
            });
            Status = CycleStatus.Recycled;
            Recycled = true;
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

    public enum CycleStatus
    {
        [Display(Name = "Not Started")]
        NotStarted = 0,
        Open = 1,
        Judging = 2,
        Closed = 3,
        Recycled = 4
    }

}
