using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessInterfaces;
using DataObjects;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/26
    /// This is the Data Access Class for the SQL Server Database
    /// Holds methods to call stored procedures for DB CRUD functions
    /// </summary>
    public class SqlDataAccessor : ISqlDataAccesor
    {
        /// <summary>
        /// Method for inserting a list of MtrHops to the MtrHop DB table
        ///     Stored procedure returns each MtrHop's ID
        /// </summary>
        /// <param name="hops"></param>
        /// <returns></returns>
        public bool InsertMtrHops(List<MtrHop> hops)
        {
            bool successfulInsert = false;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_InsertMtrHop", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter output = new SqlParameter("MtrHopID", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(output);
            cmd.Parameters.Add("@HostName", SqlDbType.NVarChar);
            cmd.Parameters.Add("@HopNumber", SqlDbType.TinyInt);
            cmd.Parameters.Add("@PacketLoss", SqlDbType.Float);
            cmd.Parameters.Add("@PacketsSent", SqlDbType.Float);
            cmd.Parameters.Add("@LastPingMS", SqlDbType.Float);
            cmd.Parameters.Add("@AvgPingMS", SqlDbType.Float);
            cmd.Parameters.Add("@BestPingMS", SqlDbType.Float);
            cmd.Parameters.Add("@WorstPingMS", SqlDbType.Float);
            cmd.Parameters.Add("@StandardDev", SqlDbType.Float);
            foreach (MtrHop hop in hops)
            {
                cmd.Parameters["@HostName"].Value = hop.Host;
                cmd.Parameters["@HopNumber"].Value = hop.HopNum;
                cmd.Parameters["@PacketLoss"].Value = hop.PacketLoss;
                cmd.Parameters["@PacketsSent"].Value = hop.PacketLoss;
                cmd.Parameters["@LastPingMS"].Value = hop.LastPingMS;
                cmd.Parameters["@AvgPingMS"].Value = hop.AvgPingMS;
                cmd.Parameters["@BestPingMS"].Value = hop.BestPingMS;
                cmd.Parameters["@WorstPingMS"].Value = hop.WorstPingMS;
                cmd.Parameters["@StandardDev"].Value = hop.StDev;

                try
                {
                    conn.Open();
                    hop.MtrHopID = Convert.ToInt32(cmd.ExecuteScalar());
                    if (hop.MtrHopID == 0)
                    {
                        throw new ApplicationException("Database insert for Mtr Hop failed.");
                    }
                    else
                    {
                        successfulInsert = true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
            return successfulInsert;
        }

        /// <summary>
        /// Method for inserting an MtrReport to the MtrReport DB table
        ///     Stored procedure returns the MtrHop's ID
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool InsertNewMtrReport(MtrReport report)
        {
            bool successfulInsert = false;

            var conn = DBConnection.GetConnection();
            
            var cmd = new SqlCommand("sp_InsertMtrReport", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter output = new SqlParameter("MtrReportID", SqlDbType.Int);
            output.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(output);
            cmd.Parameters.AddWithValue("@SyncboxID", report.SyncboxID);
            cmd.Parameters.AddWithValue("@StartTime", report.UTCStartTime);

            try
            {
                conn.Open();
                report.MtrReportID = Convert.ToInt32(cmd.ExecuteScalar());
                if(report.MtrReportID == 0)
                {
                    throw new ApplicationException("Database Insert for Mtr Report Failed.");
                }
                else
                {
                    successfulInsert = true;
                }
            }catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return successfulInsert;
        }

        /// <summary>
        /// Method for inserting a record containing the MtrReportID and MtrHopID for each hop of a given report.
        ///     This is added to the ReportHops DB table
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool InsertNewReportHops(MtrReport report)
        {
            bool successfulInsert = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_InsertMtrReportHops", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MtrReportID", report.MtrReportID);
            cmd.Parameters.Add("@MtrHopID", SqlDbType.Int);

            foreach(MtrHop hop in report.Hops)
            {
                cmd.Parameters["@MtrHopID"].Value = hop.MtrHopID;
                try
                {
                    conn.Open();
                    int rowsEffected = cmd.ExecuteNonQuery();
                    if (rowsEffected == 0)
                    {
                        throw new ApplicationException("Database Insert for MtrReportHops Failed.");
                    }
                    else
                    {
                        successfulInsert = true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
            return successfulInsert;
        }// End InsertNewReportHops method

        //  Retrieval/Select Methods
        /// <summary>
        /// Method to return a List of all the MtrReports in the DB
        /// **** With enough data this will probably not be a good idea ****
        /// </summary>
        /// <returns></returns>
        public List<MtrReport> GetAllMtrs()
        {
            List<MtrReport> mtrReports = new List<MtrReport>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_SelectAllMtrs", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                MtrReport report = new MtrReport();

                conn.Open();
                var reader1 = cmd.ExecuteReader();
                while (reader1.Read())
                {
                    if(reader1.GetInt32(0) == report.MtrReportID)// Existing MtrReport
                    {
                        MtrHop mtrHop = new MtrHop();
                        mtrHop.MtrHopID = reader1.GetInt32(3);
                        mtrHop.HopNum = reader1.GetByte(4);
                        mtrHop.Host = reader1.GetString(5);
                        mtrHop.PacketLoss = reader1.GetDecimal(6);
                        mtrHop.PacketsSent = reader1.GetByte(7);
                        mtrHop.LastPingMS = reader1.GetDecimal(8);
                        mtrHop.AvgPingMS = reader1.GetDecimal(9);
                        mtrHop.BestPingMS = reader1.GetDecimal(10);
                        mtrHop.WorstPingMS = reader1.GetDecimal(11);
                        mtrHop.StDev = reader1.GetDecimal(12);
                        report.Hops.Add(mtrHop);
                    }
                    else // NEW MtrReport
                    {
                        report = new MtrReport();

                        report.MtrReportID = reader1.GetInt32(0);
                        report.SyncboxID = reader1.GetString(1);
                        report.UTCStartTime = reader1.GetDateTime(2);

                        MtrHop mtrHop = new MtrHop();
                        mtrHop.MtrHopID = reader1.GetInt32(3);
                        mtrHop.HopNum = reader1.GetByte(4);
                        mtrHop.Host = reader1.GetString(5);
                        mtrHop.PacketLoss = reader1.GetDecimal(6);
                        mtrHop.PacketsSent = reader1.GetByte(7);
                        mtrHop.LastPingMS = reader1.GetDecimal(8);
                        mtrHop.AvgPingMS = reader1.GetDecimal(9);
                        mtrHop.BestPingMS = reader1.GetDecimal(10);
                        mtrHop.WorstPingMS = reader1.GetDecimal(11);
                        mtrHop.StDev = reader1.GetDecimal(12);
                        report.Hops.Add(mtrHop);
                    }
                    
                    mtrReports.Add(report);
                }
                reader1.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }

            return mtrReports;
        }


    }
}
