using System;
using System.Collections.Generic;
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
        private static List<UserDirectorySettings> d = UserDirectorySettings.GetDirectories();
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

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            timer.Enabled = false;
            Library.WriteErrorLog(new Exception("Directory File Manager stopped"));
        }

        //File watcher events
        private void monitorDirectories()
        {
            foreach (var dir in d)
            {
                watcher = new FileSystemWatcher(dir.directory.ToString());
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnChanged);
            }

            //Begin watching
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            List<FileSystemInfo> items = GetItems(d.FirstOrDefault().directory);
            DeleteOldItems(d.FirstOrDefault().directory, items);
        }

        private static void DeleteOldItems(DirectoryInfo directory, List<FileSystemInfo> items)
        {
            var oldest = items.Last();
            int maxCount = d.FirstOrDefault().maxCount;
            if (items.Count > maxCount)
            {
                try
                {
                    oldest.Delete();
                    Library.WriteErrorLog(new Exception($"{oldest.Name} has been deleted to make room for {items.FirstOrDefault()} The maximum number of files in {directory.Name} is {maxCount}."));
                }
                catch
                {
                }
            }
        }

        private static List<FileSystemInfo> GetItems(DirectoryInfo directory)
        {
            List<FileSystemInfo> items = new List<FileSystemInfo>();
            FileSystemInfo[] folders = directory.GetDirectories();
            FileSystemInfo[] files = directory.GetFiles();
            foreach (var f in folders)
            {
                items.Add(f);
            }
            foreach (var f in files)
            {
                items.Add(f);
            }
            List<FileSystemInfo> orderedItems = items.OrderByDescending(f => f.LastWriteTime).ToList();
            return orderedItems;
        }



        //Timed checking events
        private void scheduledCheck(int interval)
        {
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;
        }

        
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //Project logic
            var directory = new DirectoryInfo("C:/Users/esfer_000/Desktop/DirectoryTest");
            //var myFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First()
            Library.WriteErrorLog(new Exception("The job has completed."));
        }

        
    }
}
