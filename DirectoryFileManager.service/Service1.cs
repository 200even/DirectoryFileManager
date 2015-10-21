using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DirectoryFileManager.service
{
    public partial class Service1 : ServiceBase
    {
        private static Timer timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Library.WriteErrorLog(new Exception("Directory File Manager started"));
            timer = new Timer(10000);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = true;     
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //Project logic TODO
            //var directory = new DirectoryInfo("C:\\MyDirectory");
            //var myFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First()
            Library.WriteErrorLog(new Exception("The job has completed."));
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            Library.WriteErrorLog(new Exception("Directory File Manager stopped"));
        }
    }
}
