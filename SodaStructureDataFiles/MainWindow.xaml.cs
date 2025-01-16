using System;
using System.IO;
using System.Windows;

using System.ComponentModel;
using NLog;
using System.Windows.Threading;

namespace SodaStructureDataFiles
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker = new BackgroundWorker();
        private CurrentValue currentValue;

        private static Logger logger = LogManager.GetCurrentClassLogger();


        public MainWindow()
        {
            try
            {
                logger.Info("Boot");

                InitializeComponent();

                //BackgroundWorker setup     
                worker.WorkerSupportsCancellation = true;
                worker.WorkerReportsProgress = true;

                worker.ProgressChanged += Worker_ProgressChanged;
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                logger.Info("Worker Initialized");
             
            }
            catch (Exception ex)
            {
                logger.Error("Boot: " + ex.Message);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                Move(worker, e);
            }
            catch (Exception ex)
            {
                logger.Error("Worker do work: " + ex.Message);
            }
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                currentValue.Progress = e.ProgressPercentage.ToString();

            }
            catch (Exception ex)
            {
                logger.Error("Worker progress: " + ex.Message);
            }
        }
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    SetStatus("Error worker: " + e.Error.Message);
                }
                else if (e.Cancelled)
                {
                    SetStatus("worker Canceled");

                }
                else
                {
                    SetStatus("worker Finished");
                    worker.ReportProgress(100);

                }
                currentValue.Start = Visibility.Visible;
                currentValue.Stop = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                logger.Error("Worker completed: " + ex.Message);
            }
        }


        #region EVENTS CONTROLLERS


        private void btn_path_source_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        currentValue.Source = fbd.SelectedPath;
                        logger.Info("Set path source first: " + currentValue.Source);
                    }
                }
            }
            catch (Exception ex) { logger.Error("Set path source first: " + ex.Message); }
        }
   
        private void btn_path_destination_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        currentValue.Destination = fbd.SelectedPath;
                        logger.Info("Set path destination: " + currentValue.Destination);
                    }
                }
            }
            catch (Exception ex) { logger.Error("Set path destinationt: " + ex.Message); }
        }
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Start();

            }
            catch (Exception ex) { logger.Error("Start: " + ex.Message); }
        }
    
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stop();
            }
            catch (Exception ex) { logger.Error("Stop: " + ex.Message); }

        }

        #endregion


        private bool Start()
        {
            try
            {
                if (worker.IsBusy)
                {
                    SetStatus("running");
                }
                else
                {
                    if (string.IsNullOrEmpty(currentValue.Source) || string.IsNullOrEmpty(currentValue.Destination))
                    {
                        SetStatus("NO PATH SET");

                        return false;
                    }

                    currentValue.Start = Visibility.Hidden;
                    currentValue.Stop = Visibility.Visible;


                    worker.RunWorkerAsync();

                    SetStatus("started");
                }
                return true;
            }
            catch (Exception ex)
            {
                SetStatus("error start: " + ex.Message);
                return false;
            }
        }

        private bool Stop()
        {
            try
            {
                currentValue.Start = Visibility.Visible;
                currentValue.Stop = Visibility.Hidden;

                worker.CancelAsync();
                return true;
            }
            catch (Exception ex)
            {
                SetStatus("error stop: " + ex.Message);
                return false;
            }

        }

        public static bool CreateDirectory(string path)
        {
            try
            {
                if (!System.IO.Directory.Exists(path))
                {
                    // Try to create the directory.
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetStatus(string status)
        {

            try
            {
                currentValue.Status = status;

            }
            catch (Exception ex) { logger.Error("Set status: " + ex.Message); }
        }


        private void Move(BackgroundWorker worker, DoWorkEventArgs e)
        {
            try
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    worker.ReportProgress(0);

                    if (!string.IsNullOrEmpty(currentValue.Source) && !string.IsNullOrEmpty(currentValue.Destination))
                    {
                        worker.ReportProgress(0);
                        string[] files = Directory.GetFiles(currentValue.Source);
                        double count = 0;
                        int count_display = 0;
                        double increment = 100.0 / files.Length;

                        
                        string source = currentValue.Source;

                        if (files.Length > 0)
                        {
                            logger.Info("Source first Count: " + files.Length);
                        }

                        foreach (string file in files)
                        {

                            string name = Path.GetFileName(file);
                            DateTime creation_time = File.GetCreationTime(file);

                            string destination = currentValue.Destination;

                            if ((creation_time <= currentValue.Date) )
                            {

                                switch (name.Substring(0, 1))
                                {
                                    case "1": destination = destination + "\\A2"; break;
                                    case "5": destination = destination + "\\A3"; break;
                                    case "6": destination = destination + "\\A1"; break;
                                    default:  destination = destination + "\\A";  break;
                                }

                                destination = destination + "\\" + creation_time.Year + "\\" + creation_time.Month;
                                if (CreateDirectory(destination))
                                {

                                    destination = destination + "\\" + name;
                                    if (!File.Exists(destination))
                                    {
                                        try
                                        {
                                            File.Move(file, destination);
                                            count = count + increment;
                                            count_display = (int)Math.Round(count, 0);
                                            worker.ReportProgress(count_display);

                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error("Move File [" + name + "]: " + ex.Message);
                                        }
                                    }
                                }
                            }


                        }

                        worker.ReportProgress(100);
                    }

                }
            }
            catch (Exception ex)
            {
                SetStatus("Error move: " + ex.Message);

                logger.Error("Move: " + ex.Message);
            }
            finally
            {
                
                    currentValue.Start = Visibility.Visible;
                    currentValue.Stop = Visibility.Hidden;
              
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

            try
            {
                currentValue = new CurrentValue();
                DataContext = currentValue;

                
                currentValue.Date = DateTime.Today.AddMonths(-2);
              

                currentValue.Source = Properties.Settings.Default.source_path;
                currentValue.Destination = Properties.Settings.Default.destination_path;
                currentValue.Start = Visibility.Visible;
                currentValue.Stop = Visibility.Hidden;
                currentValue.Progress = "0.0";

                SetStatus("booted");


            }
            catch (Exception ex) { logger.Error("Load: " + ex.Message); }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.destination_path = currentValue.Destination;
            Properties.Settings.Default.source_path = currentValue.Source;
            Properties.Settings.Default.Save();
        }
    }

    public class CurrentValue : INotifyPropertyChanged
    {


        private string source = "";
        public string Source
        {
            get { return source; }
            set { if (value != null) { source = value; OnPropertyChanged("Source"); } }
        }


        private string destination = "";
        public string Destination
        {
            get { return destination; }
            set { if (value != null) { destination = value; OnPropertyChanged("Destination"); } }
        }


        private string progress = "";
        public string Progress
        {
            get { return progress; }
            set { if (value != null) { progress = value; OnPropertyChanged("Progress"); } }
        }

        private string status = "";
        public string Status
        {
            get { return status; }
            set { if (value != null) { status = value; OnPropertyChanged("Status"); } }
        }


        private Visibility start;
        public Visibility Start
        {
            get { return start; }
            set { start = value; OnPropertyChanged("Start"); }
        }

        private Visibility stop;
        public Visibility Stop
        {
            get { return stop; }
            set { stop = value; OnPropertyChanged("Stop"); }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; OnPropertyChanged("Date"); }
        }

        public CurrentValue() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
