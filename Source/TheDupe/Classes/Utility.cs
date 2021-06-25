using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace TheDupe.Classes
{
    static class Utility
    {
        public static bool FileInUse(string path)
        {
            try
            {
                using (FileStream fs = new(path, FileMode.OpenOrCreate)) { }
                return false;
            }
            catch (IOException) { return true; }
        }

        public static bool IsAdmin()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string testFile = "/var/log/test.txt";
                    File.CreateText(testFile).Dispose();
                    File.Delete(testFile);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string testFile = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\test.txt";
                    File.CreateText(testFile).Dispose();
                    File.Delete(testFile);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        public static string GetFileLocation(string file)
        {
            return Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), file);
        }
    }
}
