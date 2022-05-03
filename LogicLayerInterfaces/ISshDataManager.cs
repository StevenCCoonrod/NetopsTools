using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayerInterfaces
{
    public interface ISshDataManager
    {
        List<string> GetAllSyncboxes();
        MtrReport GetMtrReport(bool sendToDB);
        MtrReport GetNewestMtrReport(string syncboxID, bool sendToDB);
    }
}
