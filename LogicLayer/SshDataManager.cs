using DataAccessLayer;
using DataAccessInterfaces;
using LogicLayerInterfaces;
using DataObjects;
using System.Globalization;
using System.Windows;

namespace LogicLayer
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/20
    /// This is the data manager for retrieving data via the SSH Data Accessor,
    ///     and inserting the aquired data into the Sql DB via the Sql Data Accessor.
    /// </summary>
    public class SshDataManager : ISshDataManager
    {
        private ISshDataAccessor _sshDataAccessor;
        private ISqlDataAccesor _sqlDataAccesor;

        public SshDataManager()
        {
            _sshDataAccessor = new SshDataAccessor();
            _sqlDataAccesor = new SqlDataAccessor();
        }
        
        /// <summary>
        /// Retrieves an Mtr via SSH and Inserts the MtrReport data into the SQL Server DB via the SqlServerDataAccessor
        /// Takes a bool to allow adding the data to the DB to be optional
        /// </summary>
        /// <param name="sendToDB"></param>
        /// <param name="syncboxID"></param>
        public MtrReport GetNewestMtrReport(string syncboxID, bool sendToDB)
        {
            MtrReport report = new MtrReport();

            try
            {
                report = _sshDataAccessor.GetMostRecentMtrReport(syncboxID.ToLower()); //TO LOWER... Important for ssh command
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve Mtr data.\n", ex.InnerException);
            }
            finally
            {
                if (report != null && report.Hops.Count != 0)
                {
                    // INSERT MtrRecord data int the SQL DB
                    if (sendToDB)
                    {
                        try
                        {
                            //INSERT MtrRecord
                            bool successful = _sqlDataAccesor.InsertNewMtrReport(report);
                            if (!successful)
                            {
                                throw new Exception("Attempt to insert Mrt Report into database failed.\n");
                            }
                            else
                            {
                                //INSERT EACH MtrHop
                                bool successfulHopsInsert = _sqlDataAccesor.InsertMtrHops(report.Hops);
                                if (!successfulHopsInsert)
                                {
                                    //Might want to remove the MtrRecord from the DB if the Hops arent inserted properly
                                    throw new Exception("An error occured while attempting to insert the Mtr hop data.\n");
                                }
                                else
                                {
                                    //INSERT EACH JOIN TABLE RECORD
                                    bool successfulHopReportInsert = _sqlDataAccesor.InsertNewReportHops(report);
                                    if (!successfulHopReportInsert)
                                    {
                                        throw new Exception("The Report for " + report.SyncboxID + 
                                            " and all hops included were successfully entered into the DB.\nReportID: "
                                            + report.MtrReportID + "\nHowever there was a problem entering an Mtr hop into the MtrReportHops Join Table.");
                                    }
                                }
                            }
                        }catch (Exception ex)
                        {
                            throw new Exception (ex.Message + "\n");
                        }//End nested try-catch for Sql DB methods
                    }// End if -sendToDB-
                }//End if -report and hops aren't empty- statement
            } // End finally

            return report;

        }// END GetNewestMtrReport()
        
        /// <summary>
        /// Retrieves a list of all the syncboxes that have a directory in the SSH Mtr directory for the current date.
        /// Called in the Main Window's constructor.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSyncboxes()
        {
            return _sshDataAccessor.GetAllSyncboxes();
        }
        
        /// <summary>
        /// ****TESTING METHOD**** Uses hardcoded file path in command
        /// Retrieves an Mtr via SSH and can insert the MtrReport data into the SQL Server DB via the SqlServerDataAccessor
        /// </summary>
        /// <param name="sendToDB"></param> To allow adding the data to the DB to be optional
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public MtrReport GetMtrReport(bool sendToDB)
        {
            MtrReport report = new MtrReport();

            try
            {   
                report = _sshDataAccessor.GetMtrReport();
                if (sendToDB)
                {
                    bool successful = _sqlDataAccesor.InsertNewMtrReport(report);
                    if (!successful)
                    {
                        throw new Exception("Attempt to insert Mrt Report into database failed.");
                    }

                }
            }catch(Exception ex)
            {
                throw new ArgumentException("Unable to retrieve Mtr data.\n\n", ex);
            }

            return report;
        }// END GetMtrReport()
    }
}