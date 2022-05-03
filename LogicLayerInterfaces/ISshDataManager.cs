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
    /// Created: 2022/04/20
    /// Interface for the Ssh Data Manager.
    /// </summary>
    public interface ISshDataManager
    {
        List<string> GetAllSyncboxes();
        MtrReport GetMtrReport(bool sendToDB);
        MtrReport GetNewestMtrReport(string syncboxID, bool sendToDB);
    }
}
