using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogicLayer;
using DataObjects;
using LogicLayerInterfaces;

namespace WPFPresentationLayer
{
    /// <summary>
    /// CREATOR: Steve C
    /// Created: 2022/04/20
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ISshDataManager _sshDataManager;
        public ISqlDataManager _sqlDataManager;
        public List<string> _syncboxList;
        public List<MtrReport> _MtrReportList;

        /// <summary>
        /// CONSTRUCTOR
        /// Primary instantiation of SSH and SQL Data Managers
        /// Population of UI with Data Objects
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _sshDataManager = new SshDataManager();
            _sqlDataManager = new SqlDataManager();

            try
            {
                _syncboxList = _sshDataManager.GetAllSyncboxes();
                _MtrReportList = getAllSqlDbMtrReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to retrieve Syncbox List.\n\n", ex.Message);
                _syncboxList = new List<string>();
                _MtrReportList = getAllSqlDbMtrReports();
            }

            if(_MtrReportList != null)
            {
                lstMtrData.ItemsSource = _MtrReportList.Distinct().OrderBy(x => x.SyncboxID)
                    .ThenByDescending(x => x.UTCStartTime);
            }

            cboSyncboxes.ItemsSource = _syncboxList;
            cboStationId.ItemsSource = _syncboxList;
        }

        /// <summary>
        /// Private method called by the constructor.
        /// Returns a list of ALL Mtrs in the DB
        /// Writes any errors that occur to an error log file
        /// </summary>
        /// <returns></returns>
        private List<MtrReport> getAllSqlDbMtrReports()
        {
            List<MtrReport> list = new List<MtrReport>();
            try
            {
                list = _sqlDataManager.GetAllMtrs();

            }catch(Exception ex)
            {
                using (StreamWriter errorLog = new StreamWriter(WPFUtilities._errorLogFilePath, true))
                {
                    errorLog.WriteLine(ex.Message + "\n" + ex.InnerException);
                }
            }
            return list;
        }

        /// <summary>
        /// Calls a TEST METHOD and outputs to txtDataReturned box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            txtDataReturned.Text = "";
            txtDataReturned.Text = _sshDataManager.GetMtrReport(true).ToString();
        }

        /// <summary>
        /// Calls SSH Manager method to get the newest Mtr for a syncbox provided in the txtStationID box
        /// When provided 'true' the method inserts the Mtr Record into the Sql DB
        /// Outputs the Mtr to the txtDataReturned box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetNewestMtr_Click(object sender, RoutedEventArgs e)
        {
            txtDataReturned.Text = "";
            
                lblStationIDError.Visibility = Visibility.Hidden;
                txtDataReturned.Text = _sshDataManager.GetNewestMtrReport(cboStationId.Text.ToLower(), true).ToString();
                lblStationIDError.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Gets all the newest Mtrs for every syncbox and inserts all records into the Sql DB
        /// It currently also outputs all the data returned to the txtDataReturned box 
        ///     as each thread finishes it's task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetAllNewMtrs_Click(object sender, RoutedEventArgs e)
        {
            txtDataReturned.Text = "Beginning Mtr Sweep...\n\n";
            // A temp list copy of the _syncboxList
            List<string> tempSyncboxList = new List<string>(_syncboxList);

            // For loop dividing the temp list of syncboxes into 40, each loop:
            int threadsToCreate = _syncboxList.Count / 40;
            for (int i = 0; i <= threadsToCreate; i++)
            {
                // Creates a seperate list of the first 40 syncboxes from the temp list, 
                List<string> syncboxesForThread = new List<string>();

                // Checks if its less than 40, and removes them from the temp list once copied
                if (tempSyncboxList.Count >= 40)
                {
                    syncboxesForThread = tempSyncboxList.GetRange(0, 40);
                    tempSyncboxList.RemoveRange(0, 40);
                }
                else
                {
                    syncboxesForThread = tempSyncboxList.GetRange(0, tempSyncboxList.Count);
                    tempSyncboxList.RemoveRange(0, tempSyncboxList.Count);
                }

                /*  Uses a string array[2] to:
                 *      [0] Append the txtDataReturned.Text with each Mtr Report returned
                 *      [1] Append the error log file with any errors returned from the method
                 */
                string[] updateFromThread = new string[2];//0 = update string, 1 = error string

                // Starts a threadpool queue to:
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
                {
                    // Call SSH Manager method to get the newest mtr log for each syncbox on it's list
                    updateFromThread = WPFUtilities.GetAllMtrs(syncboxesForThread);
                    this.Dispatcher.Invoke(() =>
                    {
                        txtDataReturned.Text += updateFromThread[0];
                        if (updateFromThread[1] != null || updateFromThread[1] != "")
                        {
                            using (StreamWriter errorLog = new StreamWriter(WPFUtilities._errorLogFilePath, true))
                            {
                                errorLog.WriteLine(updateFromThread[1]);
                            }
                        }
                    });

                    if (Interlocked.Decrement(ref i) == 0) new ManualResetEvent(false).Set();
                }), null);
            }// End for loop
        }// End btnGetAllNewMtrs_Click Event

        /// <summary>
        /// Handles the Selection of a syncbox in the Combo Box
        ///     Resets the Mtr datagrid to filter the Mtr List distictly for the specified syncbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSyncboxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstMtrData.ItemsSource = _MtrReportList.Where(x => x.SyncboxID == cboSyncboxes.SelectedItem.ToString()).DistinctBy(x => x.MtrReportID).ToList();
        }

        /// <summary>
        /// This button updates and resets the Full Mtr List to the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateDataGrid_AllMtrs_Click(object sender, RoutedEventArgs e)
        {
            _MtrReportList = getAllSqlDbMtrReports();
            lstMtrData.ItemsSource = _MtrReportList.Distinct().OrderBy(x => x.SyncboxID).ThenByDescending(x => x.UTCStartTime);
        }

    }// End MainWindow Class
}
