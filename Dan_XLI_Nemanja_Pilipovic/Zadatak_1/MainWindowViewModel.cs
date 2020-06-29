using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Zadatak_1
{
    class MainWindowViewModel : BaseViewModel
    {
        readonly MainWindow main;
        private readonly BackgroundWorker bgWorker = new BackgroundWorker();
        private bool _isRunning = false;

        public MainWindowViewModel(MainWindow mainOpen)
        {
            main = mainOpen;
            bgWorker.DoWork += WorkerOnDoWork;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.ProgressChanged += WorkerOnProgressChanged;
            bgWorker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
        }

        private string textForPrinting;
        public string TextForPrinting
        {
            get
            {
                return textForPrinting;
            }
            set
            {
                textForPrinting = value;
                OnPropertyChanged("TextForPrinting");
            }
        }

        private string numberOfCopy;
        public string NumberOfCopy
        {
            get
            {
                return numberOfCopy;
            }
            set
            {
                numberOfCopy = value;
                OnPropertyChanged("NumberOfCopy");
            }
        }
       
        private int currentProgress;
        public int CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                if (currentProgress != value)
                {
                    currentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;            
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = "";
            
            for (int i = 1; i < int.Parse(NumberOfCopy) + 1; i++)
            {
                Thread.Sleep(1000);
                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                   
                    bgWorker.ReportProgress(0);
                    return;
                }

                fileName = i + "." + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" +
                    DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;

                using (StreamWriter streamWriter = new StreamWriter(fileName, append: true))
                {
                    streamWriter.WriteLine(TextForPrinting);
                }

                if (i == int.Parse(NumberOfCopy))
                {             
                    bgWorker.ReportProgress(100);
                }
                else
                {
                    bgWorker.ReportProgress(Convert.ToInt32(Math.Round(100 / double.Parse(NumberOfCopy))) * i);
                }
            }

            _isRunning = false;

            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
            }
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
              
                _isRunning = false;
            }
            else if (e.Error != null)
            {
               
                _isRunning = false;
            }
            else if (CurrentProgress == 100)
            {               
                _isRunning = false;
                MessageBox.Show("Finished printing " + NumberOfCopy + " documents.");
            }
        }

        private ICommand print;
        public ICommand Print
        {
            get
            {
                if (print == null)
                {
                    print = new RelayCommand(param => PrintExecute(), param => CanPrintExecute());
                }
                return print;
            }
        }

        private bool CanPrintExecute()
        {
            if (String.IsNullOrEmpty(TextForPrinting) || String.IsNullOrEmpty(NumberOfCopy)
                || String.IsNullOrWhiteSpace(TextForPrinting) || String.IsNullOrWhiteSpace(NumberOfCopy))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void PrintExecute()
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync();
                _isRunning = true;
            }
            else
            {
                MessageBox.Show("Printer already printing...");
            }
        }

        private ICommand cancel;
        public ICommand Cancel
        {
            get
            {
                if (cancel == null)
                {
                    cancel = new RelayCommand(param => CancelExecute(), param => CanCancelExecute());
                }
                return cancel;
            }
        }

        private bool CanCancelExecute()
        {
            if (_isRunning == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CancelExecute()
        {
            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                _isRunning = false;
            }
        }
    }
}