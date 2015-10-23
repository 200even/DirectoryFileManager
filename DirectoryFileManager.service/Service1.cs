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
        DirectoryInfo d = GetDirectory();
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

        private static DirectoryInfo GetDirectory()
        {
            List<UserDirectorySettings> Directories = new List<UserDirectorySettings>();
            string[] fileContents = File.ReadAllLines(@"C:/Users/esfer_000/Desktop/DirectoryTest/DirectoriesFollowed.csv");
            foreach (string row in fileContents)
            {
                if (row.StartsWith("Directory"))
                {
                    continue;
                }
                string[] directoryInfo = row.Split(',');
                UserDirectorySettings activeDirectory = new UserDirectorySettings();
                activeDirectory.directory = new DirectoryInfo(directoryInfo[0]);
                activeDirectory.maxCount = int.Parse(directoryInfo[1]);
                Directories.Add(activeDirectory);
            }

            var directory = Directories.FirstOrDefault().directory;
            //var directory = new DirectoryInfo("C:/Users/esfer_000/Desktop/DirectoryTest");
            return directory;
        }

        //File watcher events
        private void monitorDirectories()
        {
            
            watcher = new FileSystemWatcher(d.ToString());
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnChanged);

            //Begin watching
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            List<FileSystemInfo> items = GetItems(d);
            DeleteOldItems(d, items);
        }

        private static void DeleteOldItems(DirectoryInfo directory, List<FileSystemInfo> items)
        {
            var oldest = items.Last();
            int maxCount = 5;
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
