using DataObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataAccessUtilities
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/20
    /// This holds utility methods for the SSH Data Accessor.
    ///     SSH connection information
    ///     Parsing of SSH command output into data objects
    /// </summary>
    public static class SshAccessUtilities
    {
        //Connection Info
        public static string _host = "master3.syncbak.com";
        public static string _user = "usernameGoesHere";
        public static string _pass = "passwordGoesHere";
        public static string _rootMtrDirectory = "/var/log/syncbak/catcher-mtrs";

        /// <summary>
        /// This method initiates parsing an Mtr string and returns an MtrReport
        ///     Determines the type of MTR based on starting line
        ///     Depending on the type, sends the rest of the parsing to be done in secondary method
        /// </summary>
        /// <param name="mtrDataString"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static MtrReport parseSshStringIntoMtrReport(string mtrDataString, DateTime datetime)
        {
            MtrReport report = new MtrReport();
            //Split the string returned into a list of lines to parse
            List<string> linesInReport = mtrDataString.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

            // Determine Standard or No-Start parsing
            if (linesInReport[0].Contains("START") || linesInReport[0].Contains("Start"))
            {
                report = StandardMtrParse(report, linesInReport);
            }
            else if(linesInReport[0].Contains("HOST") || linesInReport[0].Contains("Host"))
            {
                report.UTCStartTime = datetime.ToUniversalTime();
                report = NoStartMtrParse(report, linesInReport);
            }

            return report;
        }// END METHOD : parseSshStringIntoMtrReport

        /// <summary>
        /// Secondary method that handles parsing the MtrHop data into the MtrReport's Hops List
        /// </summary>
        /// <param name="report"></param>
        /// <param name="linesInReport"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static MtrReport NoStartMtrParse(MtrReport report, List<string> linesInReport)
        {
            try
            {
                //Remove 'Host:' from Line 1 and set SyncboxID to the first element in the string once split on '.'
                string syncboxID = linesInReport[0].Remove(0, 5);
                report.SyncboxID = syncboxID.Split('.').ElementAt(0).Trim();

                linesInReport.RemoveAt(0);//Remove host line

                //Iterate through hop lines
                foreach (string line in linesInReport)
                {
                    string trueLine = line.TrimStart();
                    string[] delimiters = { " ", "\t" };
                    MtrHop hop = new MtrHop();
                    List<string> fields = trueLine.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

                    string hopNum = fields[0];
                    hopNum = hopNum.Split('.').ElementAt(0);
                    hop.HopNum = byte.Parse(hopNum);
                    hop.Host = fields[1].ToUpper();
                    hop.PacketLoss = decimal.Parse(fields[2].Remove(fields[2].Count() - 1));//Removes '%' before parsing
                    hop.PacketsSent = byte.Parse(fields[3]);
                    hop.LastPingMS = decimal.Parse(fields[4]);
                    hop.AvgPingMS = decimal.Parse(fields[5]);
                    hop.BestPingMS = decimal.Parse(fields[6]);
                    hop.WorstPingMS = decimal.Parse(fields[7]);
                    hop.StDev = decimal.Parse(fields[8]);

                    report.Hops.Add(hop);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error parsing(NoStart) the Mtr data for " + report.SyncboxID + ".\n\n", ex);
            }
            return report;
        }

        /// <summary>
        /// Secondary method that handles parsing the MtrHop data into the MtrReport's Hops List
        /// </summary>
        /// <param name="report"></param>
        /// <param name="linesInReport"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static MtrReport StandardMtrParse(MtrReport report, List<string> linesInReport)
        {
            try
            {
                //Get the datetime field from line 1.
                //NOTE: currently this is changing the datetime format to be accepted as a valid datetime
                string datetime = linesInReport[0].Remove(0, 7).Replace("  ", " ");
                
                report.UTCStartTime = DateTime.ParseExact(datetime, "ddd MMM dd HH':'mm':'ss yyyy", null, DateTimeStyles.AssumeUniversal).ToUniversalTime();
                

                //Remove 'Host:' from Line 2 and set SyncboxID to the first element in the string once split on '.'
                string syncboxID = linesInReport[1].Remove(0, 5);
                report.SyncboxID = syncboxID.Split('.').ElementAt(0).Trim();

                linesInReport.RemoveAt(0);//Remove start line
                linesInReport.RemoveAt(0);//Remove host line

                //Iterate through hop lines
                foreach (string line in linesInReport)
                {
                    string trueLine = line.TrimStart();
                    string[] delimiters = { " ", "\t" };
                    MtrHop hop = new MtrHop();
                    List<string> fields = trueLine.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
                    string hopNum = fields[0];
                    hopNum = hopNum.Split('.').ElementAt(0);
                    hop.HopNum = byte.Parse(hopNum);
                    hop.Host = fields[1].ToUpper();
                    hop.PacketLoss = decimal.Parse(fields[2].Remove(fields[2].Count() - 1));//Removes '%' before parsing
                    hop.PacketsSent = byte.Parse(fields[3]);
                    hop.LastPingMS = decimal.Parse(fields[4]);
                    hop.AvgPingMS = decimal.Parse(fields[5]);
                    hop.BestPingMS = decimal.Parse(fields[6]);
                    hop.WorstPingMS = decimal.Parse(fields[7]);
                    hop.StDev = decimal.Parse(fields[8]);

                    report.Hops.Add(hop);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error parsing(standard) the Mtr data for" + report.SyncboxID + ".\n\n", ex);
            }
            return report;
        }
    }
}
