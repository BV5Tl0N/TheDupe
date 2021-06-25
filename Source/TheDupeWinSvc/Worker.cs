using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TheDupeWinSvc
{
#pragma warning disable CA1416 // Validate platform compatibility
    public class Worker : BackgroundService
    {
        private Process Proc;
        private string ExePath;
        private string EventSource;
        private string EventLogName;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            EventSource = "The Dupe";
            EventLogName = "Application";

            if (!EventLog.SourceExists(EventSource))
            {
                EventSourceCreationData eventSource = new(EventSource, EventLogName);
                EventLog.CreateEventSource(eventSource);
            }

            StartTheDupe();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            StopTheDupe();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (Proc.HasExited)
                        Proc.Start();
                    else
                        Proc.Refresh();

                    await Task.Delay(5000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EventSource, ex.StackTrace, EventLogEntryType.Error);
            }
        }

        private void StartTheDupe()
        {
            try
            {
                Proc = new();
                ExePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                Proc.StartInfo.FileName = Path.Combine(ExePath, "TheDupe.exe");
                Proc.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EventSource, ex.StackTrace, EventLogEntryType.Error);
            }
        }

        private void StopTheDupe()
        {
            try
            {
                Proc.Kill();
                Proc.WaitForExit();
                Proc.Dispose();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(EventSource, ex.StackTrace, EventLogEntryType.Error);
            }
        }
    }
}
