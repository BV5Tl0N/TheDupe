using PacketDotNet;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace TheDupe.Classes
{
    public class Log
    {
        public IPAddress SourceAddress;
        public IPAddress DestinationAddress;
        public DateTime EventDateTime;
        public string ServiceName;
        public int ServiceNumber;
        public ProtocolType Protocol;

        public static ConcurrentQueue<Log> LogQueue = new();
        public static AutoResetEvent LogQueueNotif = new(false);

        public static void Initialize()
        {
            try
            {
                CheckDir();
                StartQueue();
            }
            catch (Exception ex)
            {
                WriteError(ex.Message, ex.InnerException.ToString());
                Environment.Exit(0);
            }
        }

        public static void CheckDir()
        {
            if (!Directory.Exists(Config.LogDir))
                Directory.CreateDirectory(Config.LogDir);
        }

        public static void WriteEvent(Log log)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string logFile = string.Format(@"{0}/Dupe_{1}_{2}.log",
                    Config.LogDir,
                    dateTime.ToString(Config.DateFormat),
                    dateTime.ToString("HH")
                );

                if (!Utility.FileInUse(logFile))
                {
                    StreamWriter sw = new(logFile, true);
                    sw.WriteLine(string.Format("Dupe Warning {0} {1} {2} {3} {4} {5} {6}",
                        log.EventDateTime.ToString(Config.DateFormat),
                        log.EventDateTime.ToString(Config.TimeFormat),
                        log.DestinationAddress,
                        log.SourceAddress,
                        log.Protocol,
                        log.ServiceName,
                        log.ServiceNumber)
                    ); ;
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message, ex.InnerException.ToString());
            }
        }

        public static void WriteError(string message, string innerException = "")
        {
            try
            {
                string logFile = string.Format(@"{0}/Error_{1}.log",
                    Config.LogDir,
                    DateTime.Now.ToString(Config.DateFormat)
                );

                if (!Utility.FileInUse(logFile))
                {
                    StreamWriter sw = new(logFile, true);

                    sw.WriteLine(string.Format("{0}\t{1}\t{2}",
                            DateTime.Now.ToString(Config.TimeFormat),
                            message,
                            innerException
                        )
                    );
                    sw.Dispose();
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                WriteError(ex.Message, ex.InnerException.ToString());
            }
        }

        private static void CleanUp()
        {
            string[] logFiles = Directory.GetFiles(Config.LogDir).Where(l => l.Contains("Dupe_")).ToArray();

            foreach (string file in logFiles)
                if (File.GetCreationTime(file) < DateTime.Now.AddDays(-1))
                    File.Delete(file);
        }

        private static void StartQueue()
        {
            try
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        CleanUp();
                        LogQueueNotif.WaitOne();
                        LogQueue.TryDequeue(out Log log);
                        WriteEvent(log);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                WriteError(ex.Message, ex.InnerException.ToString());
            }
        }
    }
}
