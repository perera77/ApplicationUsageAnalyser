using ApplicationUsageAnalyser.Infrastructure;
using Microsoft.Win32;
using ModelLib;
using ServiceLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ApplicationUsageAnalyser.ViewModel
{
    public class ApplicationUsageAnalyserVM : INotifyPropertyChanged
    {
        #region PrivateData
        
        public string selectedFilePath;
        private List<ApplicationUsageAnalysisModel> applicationList = new List<ApplicationUsageAnalysisModel>();
        private string serviceStatus;
        private int progressValue = 0;
        System.Windows.Input.Cursor cursor = System.Windows.Input.Cursors.Arrow;
        #endregion PrivateData

        public ApplicationUsageAnalyserVM()
        { }

        #region Proprties
        public List<ApplicationUsageAnalysisModel> ApplicationList
        {
            get => applicationList;
            set { applicationList = value; NotifyPropertyChanged("ApplicationList"); }
        }

        public string ApplicationUsageVisibility => string.IsNullOrEmpty(SelectedFilePath) ? "Hidden" : "Visible";

        public string SelectedFilePath => selectedFilePath;

        public int ProgressValue => progressValue;
        public string ProgressText => serviceStatus + string.Format(" {0:0}%", progressValue);

        public System.Windows.Input.Cursor Cursor
        {
            get => cursor;
            set { cursor = value; NotifyPropertyChanged("Cursor"); }
        }
        #endregion Proprties


        #region Commands
        public ICommand OpenDataFileCommand =>
            new CommandHandler(
                (parameter) =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    if (openFileDialog.ShowDialog() == true)
                    {
                        selectedFilePath = openFileDialog.FileName;
                        ApplicationList = null;
                        NotifyPropertyChanged("SelectedFilePath");
                        NotifyPropertyChanged("ApplicationUsageVisibility");

                        // setting-up background worker to run th long data analysis task and report progrss
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoWork;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                        Cursor = System.Windows.Input.Cursors.Wait;
                        worker.RunWorkerAsync(10000);
                    }
                },
                () => { return string.IsNullOrEmpty(serviceStatus) ? true : false; } // disablee the filer button when the srvic in progress
            );

        #endregion Commands

        #region Backgrounf Worker
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var service = new ApplicationUsageAnalysisService();
                var task = service.AnalyseDataFileAsync(selectedFilePath);
                int i = 0;
                while (!task.IsCompleted)
                {
                    serviceStatus = service.Status;
                    (sender as BackgroundWorker).ReportProgress(service.Progress);
                    System.Threading.Thread.Sleep(100);
                }

                ApplicationList = task.Result.ToList(); //populates analysis results when complted.
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Analysing usage data failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressValue = e.ProgressPercentage;
            NotifyPropertyChanged("ProgressValue");
            NotifyPropertyChanged("ProgressText");
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressValue = 100;
            NotifyPropertyChanged("ProgressValue");
            NotifyPropertyChanged("ProgressText");
            
            NotifyPropertyChanged("ApplicationUsageVisibility");
            progressValue = 0;
            serviceStatus = string.Empty;
            Cursor = System.Windows.Input.Cursors.Hand;
        }
        #endregion Backgrounf Worker

        #region INotifyPropertyChanged Interface
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion INotifyPropertyChanged Interface

    }
}
