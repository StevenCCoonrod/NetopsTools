using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/26
    /// Interface for the Sql Data Accessor
    /// </summary>
    public interface ISqlDataAccesor
    {
        bool InsertNewMtrReport(MtrReport report);
        bool InsertMtrHops(List<MtrHop> hops);
        bool InsertNewReportHops(MtrReport report);

        List<MtrReport> GetAllMtrs();
        List<MtrReport> GetAllMtrsWithinRange(DateTime startTime, DateTime endTime);
        List<MtrReport> GetSyncboxMtrsWithinRange(string? targetSyncbox, DateTime startTime, DateTime endTime);
    }
}
