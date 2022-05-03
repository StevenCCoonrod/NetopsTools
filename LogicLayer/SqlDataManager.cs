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
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/26
    /// This is the Data Manager for the SQL Server Data Accessor
    /// Methods here call Acessor methods to retrieve data
    ///     as well as handle exceptions that arise
    /// </summary>
    public class SqlDataManager : ISqlDataManager
    {
        ISqlDataAccesor sqlDataAccessor;


        public SqlDataManager()
        {
            sqlDataAccessor = new SqlDataAccessor();
        }

        /// <summary>
        /// Method to retrieve a list of ALL Mtr Reports in the DB
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
