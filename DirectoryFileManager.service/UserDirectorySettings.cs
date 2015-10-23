using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryFileManager.service
{
    class UserDirectorySettings
    {
        public DirectoryInfo directory { get; set; }
        public int maxCount { get; set; }

        public static List<UserDirectorySettings> GetDirectories()
        {
            List<UserDirectorySettings> Directories = new List<UserDirectorySettings>();
            string[] fileContents = File.ReadAllLines(@"C:/Users/esfer_000/Desktop/DirectoriesFollowed.csv");
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

            return Directories;
        }
    }
}
