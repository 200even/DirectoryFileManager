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
        private static List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private static List<UserDirectorySettings> d = UserDirectorySettings.GetDirectories();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Library.WriteErrorLog(new Exception("Directory File Manager started"));
            monitorDirectories();
        }

        protected override void OnStop()
        {
            foreach(var watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }

            Library.WriteErrorLog(new Exception("Directory File Manager stopped"));
        }

        private void monitorDirectories()
        {
            
            foreach (var dir in d)
            {
                try
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(dir.directory.ToString());
                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
                    watcher.Created += new FileSystemEventHandler(OnChanged);
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(OnChanged);
                    watcher.Renamed += new RenamedEventHandler(OnChanged);
                    //Begin watching
                    watcher.EnableRaisingEvents = true;
                    watchers.Add(watcher);
                }
                catch
                {
                }
            }

            
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher changedDirectory = (FileSystemWatcher)sender;
            var changedDirectoryInfo = new DirectoryInfo(changedDirectory.Path);
            List<FileSystemInfo> items = GetItems(changedDirectoryInfo);
            DeleteOldItems(changedDirectoryInfo, items);
        }

        private static void DeleteOldItems(DirectoryInfo directory, List<FileSystemInfo> items)
        {
            var oldest = items.Last();
            var changedDirectory = d.FirstOrDefault(d => d.directory.CreationTime == directory.CreationTime);
            int maxCount = changedDirectory.maxCount;
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
    }
}
