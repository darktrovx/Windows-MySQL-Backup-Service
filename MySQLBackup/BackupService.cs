using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace MySqlBackup
{
    public partial class BackupService : ServiceBase
    {
        private Timer backupTimer;
        private readonly string BackupFolder = ConfigurationManager.AppSettings["BackupFolder"];
        private readonly string MySqlDumpPath = ConfigurationManager.AppSettings["MySqlDumpPath"];
        private readonly string DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
        private readonly string Username = ConfigurationManager.AppSettings["Username"];
        private readonly string Password = ConfigurationManager.AppSettings["Password"];
        private readonly double IntervalInHours = double.Parse(ConfigurationManager.AppSettings["IntervalInHours"]);

        public BackupService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log("Service started.");
            if (!Directory.Exists(BackupFolder))
            {
                Directory.CreateDirectory(BackupFolder);
            }

            backupTimer = new Timer
            {
                Interval = IntervalInHours * 60 * 60 * 1000, // Convert hours to milliseconds
                AutoReset = true,
                Enabled = true
            };
            backupTimer.Elapsed += PerformBackup;
            backupTimer.Start();

            // Perform the first backup immediately
            PerformBackup(null, null);
        }

        protected override void OnStop()
        {
            Log("Service stopped.");
            backupTimer.Stop();
        }

        private void PerformBackup(object sender, ElapsedEventArgs e)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFile = Path.Combine(BackupFolder, $"{DatabaseName}_backup_{timestamp}.sql");

                string arguments = $"-u {Username} -p{Password} {DatabaseName} > \"{backupFile}\"";

                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{MySqlDumpPath}\" {arguments}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        Log($"Backup successful: {backupFile}");
                    }
                    else
                    {
                        Log($"Backup failed. Error: {process.StandardError.ReadToEnd()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Exception during backup: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            string logFile = Path.Combine(BackupFolder, "backup_service.log");
            File.AppendAllText(logFile, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
