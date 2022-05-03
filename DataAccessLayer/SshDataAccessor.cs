using DataAccessInterfaces;
using DataAccessUtilities;
using DataObjects;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace DataAccessLayer
{
    public class SshDataAccessor : ISshDataAccessor 
    {
        private string _host = SshAccessUtilities._host;
        private string _user = SshAccessUtilities._user;
        private string _pass = SshAccessUtilities._pass;
        private string _rootMtrDirectory = SshAccessUtilities._rootMtrDirectory;
        
        
        
        /*
         * 
         * Builds a command to retrieve a list of all the syncboxes listed in the Mtr directory
         * 
         */
        public List<string> GetAllSyncboxes()
        {
            List<string> syncboxes = new List<string>();

            string output = "";
            string command = "find " + _rootMtrDirectory + " -maxdepth 4 -type d | cut -d '/' -f 9 | uniq | sort";
            output = RunClientCommand(command);

            syncboxes = output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            List<string> syncboxesUpper = new List<string>();
            foreach (string syncbox in syncboxes)
            {
                syncboxesUpper.Add(syncbox.ToUpper().Trim());
            }


            return syncboxesUpper;
        }


        /*
         * 
         * TESTING METHOD:
         * Uses a hardcoded file path
         * Simple test method to retrieve data from a specific log file
         * and parse the data returned into an MtrReport object.
         * 
         */
        public MtrReport GetMtrReport()
        {
            MtrReport report = new MtrReport();
            string command = "cat /var/log/syncbak/catcher-mtrs/2022/04/17/kxly-2309/kxly-2309-2022-04-17-03-07-dc-mtr-catcher.log";
            string output = RunClientCommand(command);

            report = SshAccessUtilities.parseSshStringIntoMtrReport(output, DateTime.Now);

            return report;
        }


        /*
         * 
         * Method which gets the current datetime and builds a command
         * to retrieve the most recent MTR log in the directory
         * for that date and Syncbox
         * 
         */
        public MtrReport GetMostRecentMtrReport(string syncboxID)
        {
            MtrReport report = new MtrReport();

            //Get Current Datetime and ensure day and month have 
            DateTime date = DateTime.Now;
            string dateYear = date.Year.ToString();
            string dateMonth = date.Month.ToString();
            if (dateMonth.Length == 1)
            {
                dateMonth = dateMonth.Insert(0, "0");
            }
            string dateDay = date.Day.ToString();
            if (dateDay.Length == 1)
            {
                dateDay = dateDay.Insert(0, "0");
            }
            
            //Build Command String
            string command = "cat " + _rootMtrDirectory + "/" +
                dateYear + "/" + dateMonth + "/" + dateDay + "/" +
                syncboxID + "/" + "\"$(ls -Art " + _rootMtrDirectory + "/" + 
                dateYear + "/" + dateMonth + "/" + dateDay + "/" + 
                syncboxID + " | tail -n 1)" + "\"";
            // Example command: cat /var/log/syncbak/catcher-mtrs/2022/04/18/kxly-2309/"$(ls -Art /var/log/syncbak/catcher-mtrs/2022/04/18/kxly-2309 | tail -n 1)"
           
            //Run the command to retrieve the mtr report
            try
            {
                string output = RunClientCommand(command);

                if(output != "")
                {
                    report = SshAccessUtilities.parseSshStringIntoMtrReport(output, date);
                }
            }
            catch (Exception)
            {
                throw;
            }
            

            return report;
        }


        /**
         * 
         * Simple SSH Client Connection
         * Takes in the command to be sent to the SSH server
         * 
         */
        private string RunClientCommand(string command)
        {
            string dataReturned = "";

            using (var client = new SshClient(_host, _user, _pass))
            {
                client.Connect(); 
                var output = client.RunCommand(command);
                client.Disconnect();

                dataReturned = output.Result;
            }

            return dataReturned;
        }

    }
}