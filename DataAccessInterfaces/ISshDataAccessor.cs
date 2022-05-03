using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessInterfaces
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/20
    /// Interface for the Ssh Data Accessor
    /// </summary>
    public interface ISshDataAccessor
    {
        List<string> GetAllSyncboxes();
        MtrReport GetMtrReport();
        MtrReport GetMostRecentMtrReport(string syncboxID);
    }
}