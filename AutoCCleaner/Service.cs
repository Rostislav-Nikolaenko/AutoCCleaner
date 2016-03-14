using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace AutoCCleaner
{
    public partial class Service : ServiceBase
    {
        private string path = String.Empty;
        

        void FindPath(string Folder)
        {
            foreach (string file in Directory.GetFiles(Folder, "CCleaner.exe"))
            {
                if (file.Length > 0) path = file.Substring(0, file.Length-13);
            }
            foreach (string subDir in Directory.GetDirectories(Folder))
            {
                try
                {
                    if (path == String.Empty)
                        FindPath(subDir);
                }
                catch
                {

                }
            }
        }

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DriveInfo.GetDrives().ToList().ForEach(driver => {

                if (path == String.Empty && driver.IsReady)
                    FindPath(driver.Name);
            });
            Timer timer = new Timer(TimeUp, null, TimeSpan.Zero, new TimeSpan(0, 0, 0, 1));

        }
        void TimeUp(object obj)
        {
            DateTime time = DateTime.Now;
            if (time.Hour==17 && time.Minute % 59 == 0)
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = String.Format("/c {0} && cd {1} && CCleaner.exe /AUTO", path.Substring(0, 2), path);
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();
            }
                
        }

        protected override void OnStop()
        {
        }
    }
}
