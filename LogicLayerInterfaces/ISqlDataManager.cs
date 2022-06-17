using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayerInterfaces
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/26
    /// Interface for the Sql Data Manager.
    /// </summary>
    public interface ISqlDataManager
    {
        List<MtrReport> GetAllMtrs();
        List<MtrReport> GetAllMtrsWithinRange(DateTime startTime, DateTime endTime);
        List<MtrReport> GetSyncboxMtrsWithinRange(string? targetSyncbox, DateTime startTime, DateTime endTime);
    }
}
