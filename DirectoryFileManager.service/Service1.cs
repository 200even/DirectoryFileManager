using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace DirectoryFileManager.service
{
    public partial class Service1 : ServiceBase
    {
        private static Timer timer;
        private static FileSystemWatcher watcher;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Library.WriteErrorLog(new Exception("Directory File Manager started"));
            scheduledCheck(60000);
            monitorDirectories();

        }

        private void monitorDirectories()
        {
            watcher = new FileSystemWatcher("C:/Users/esfer_000/Desktop/DirectoryTest");
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnChanged);

            //Begin watching
            watcher.EnableRaisingEvents = true;
        }

        private void scheduledCheck(int interval)
        {
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Library.WriteErrorLog(new Exception("A file has been added."));
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //Project logic
            var directory = new DirectoryInfo("C:/Users/esfer_000/Desktop/DirectoryTest");
            //var myFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First()
            Library.WriteErrorLog(new Exception("The job has completed."));
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            timer.Enabled = false;
            Library.WriteErrorLog(new Exception("Directory File Manager stopped"));
        }
    }
}
