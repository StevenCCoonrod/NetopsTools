using DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessInterfaces
{
    public interface ISqlDataAccesor
    {
        bool InsertNewMtrReport(MtrReport report);
        bool InsertMtrHops(List<MtrHop> hops);
        bool InsertNewReportHops(MtrReport report);

        List<MtrReport> GetAllMtrs();
    }
}
