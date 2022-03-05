using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.Data;
using WCS.Models;

namespace WCS.Services
{
    public interface IRecycle
    {
        #region IRecycle Members

        bool Recycled { get; set; }
        RecycledItem GetRecycledItem(WcsContext context);

        Task RecycleAsync(WcsContext context, User recycledBy);
        Task RestoreAsync(WcsContext context);
        Task PermanentDeleteAsync(WcsContext context);

        #endregion
    }
}
