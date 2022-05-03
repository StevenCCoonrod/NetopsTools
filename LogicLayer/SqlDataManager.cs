using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;
using LogicLayerInterfaces;
using DataAccessInterfaces;
using DataAccessLayer;

namespace LogicLayer
{
    public class SqlDataManager : ISqlDataManager
    {
        ISqlDataAccesor sqlDataAccessor;

        public SqlDataManager()
        {
            sqlDataAccessor = new SqlDataAccessor();
        }

        public List<MtrReport> GetAllMtrs()
        {
            List<MtrReport> mtrReports = new List<MtrReport>();
            try
            {
                mtrReports = sqlDataAccessor.GetAllMtrs();
            }
            catch(Exception ex)
            {
                throw new Exception("There is an error accessing the Mtr data in the DB.\n" 
                    + ex.Message + "\n" + ex.InnerException + "\n");
            }

            return mtrReports;
        }

    }
}
