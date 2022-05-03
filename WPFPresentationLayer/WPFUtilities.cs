using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DataObjects;
using LogicLayer;
using System.Text.RegularExpressions;

namespace WPFPresentationLayer
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/26
    /// Provides UI utility methods for the WPF Presentation Layer
    /// </summary>
    internal static class WPFUtilities
    {

        public static string _errorLogFilePath = Directory.GetParent(path: Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "/mtrErrorLog.txt";

        /// <summary>
        /// Takes as a parameter a list of syncboxes and calls the SSH manager's method
        ///     to retrieve the newest Mtr log for each syncbox and insert it into the Sql DB
        /// This returns a string array[2]
        ///     [0] Appends with the string object of each Mtr Report returned
        ///     [1] Appends with any errors returned from the method to be sent to an error log file
        /// </summary>
        /// <param name="syncboxes"></param>
        /// <returns></returns>
        public static string[] GetAllMtrs(List<string> syncboxes)
        {
            SshDataManager tempDataManager = new SshDataManager();// Not a great implementation I know...
            string[] updateString = new string[2];
            try
            {
                MtrReport returnedMtrReport;
                foreach (string syncbox in syncboxes)
                {
                    updateString[0] += "Loading " + syncbox + " Mtr...\n";

                    returnedMtrReport = tempDataManager.GetNewestMtrReport(syncbox, true);

                    if (returnedMtrReport.SyncboxID == null && returnedMtrReport.Hops.Count == 0)
                    {
                        updateString[0] += "No Mtr data for " + syncbox + ".\n\n";
                    }
                    else if (returnedMtrReport != null && returnedMtrReport.Hops.Count <= 2)
                    {
                        updateString[0] += "There is a firewall blocking the traceroute.\n\n";
                    }
                    else
                    {
                        updateString[0] += returnedMtrReport.ToString() + "\n";
                    }
                    
                }
            }catch(Exception ex)
            {
                updateString[1] += "Error parsing Data!\n" + ex.Message + "\n" + ex.InnerException + "\n\n";
            }
                
            return updateString;
        }

        /// <summary>
        /// Regex string validation method to ensure an [abcd] or [abcd-1234] pattern
        /// </summary>
        /// <param name="stationIdInput"></param>
        /// <returns></returns>
        public static bool ValidateSyncboxIDinput(string stationIdInput)
        {
            bool isValid = false;

            Regex regexfor2300 = new Regex(@"[a-zA-Z]{4}");
            Match matchFor2300 = regexfor2300.Match(stationIdInput);
            Regex regexfor2309 = new Regex(@"[a-zA-Z]{4}-[0-9]{4}");
            Match matchFor2309 = regexfor2309.Match(stationIdInput);
            if (matchFor2300.Success || matchFor2309.Success)
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
