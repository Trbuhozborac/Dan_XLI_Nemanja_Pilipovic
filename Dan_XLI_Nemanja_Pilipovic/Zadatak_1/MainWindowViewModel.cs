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
        BackgroundWorker bgWorker = new BackgroundWorker();
        bool CanClose;
        MainWindow mainOpen;
        public MainWindowViewModel(MainWindow main)
        {
            main = mainOpen;
            bgWorker.ProgressChanged += PopulateProgressBar;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;

        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Completed!");
        }

        private void Print()
        {
            int number = Convert.ToInt32(numberOfCopy);
            for (int i = 0; i < number; i++)
            {
                Thread.Sleep(1000);
                string fileName = i + "." + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_"
                    + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString();
                string location = $@"~\..\..\..\{fileName}.txt";
                using (StreamWriter sw = new StreamWriter(location))
                {
                    sw.Write(TextForPrinting);
                }
            }

            MessageBox.Show("Text Printed Succesfully!");
        }

        private int progressBar;

        public int ProgressBar
        {
            get { return progressBar; }
            set
            {
                progressBar = value;
                OnPropertyChanged("ProgressBar");
            }
        }


        private string textForPrinting;

        public string TextForPrinting
        {
            get { return textForPrinting; }
            set
            {
                textForPrinting = value;
                OnPropertyChanged("TextForPrinting");
            }
        }

        private string numberOfCopy;

        public string NumberOfCopy
        {
            get { return numberOfCopy; }
            set
            {
                numberOfCopy = value;
                OnPropertyChanged("NumberOfCopy");
            }
        }

        private ICommand printText;
        public ICommand PrintText
        {
            get
            {
                if (printText == null)
                {
                    printText = new RelayCommand(param => PrintTextExecute(), param => CanPrintTextExecute());
                }
                return printText;
            }
        }

        private void PrintTextExecute()
        {
            CanClose = true;
            Thread thread = new Thread(() => Print());
            thread.Start();
        }

        private bool CanPrintTextExecute()
        {
            if (String.IsNullOrEmpty(TextForPrinting) || String.IsNullOrEmpty(NumberOfCopy) || 
                String.IsNullOrWhiteSpace(TextForPrinting) || String.IsNullOrWhiteSpace(NumberOfCopy))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private ICommand close;
        public ICommand Close
        {
            get
            {
                if (close == null)
                {
                    close = new RelayCommand(param => CloseExecute(), param => CanCloseExecute());
                }
                return close;
            }
        }

        private void CloseExecute()
        {
            //TODO stopirati workera
        }

        private bool CanCloseExecute()
        {
            if (CanClose == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PopulateProgressBar(object o, ProgressChangedEventArgs e)
        {
            ProgressBar = e.ProgressPercentage;
        }
    }
}
