using System;
using System.Windows.Forms;

namespace HospitalTerminalOpt
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string baseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            // Initialize configuration and logging
            ConfigurationManager.Initialize(baseDir);
            AuditLogger.Initialize(baseDir);

            // Check for silent mode
            if (args.Length > 0 && args[0] == "-silent")
            {
                ExecuteSilentMode(args);
                return;
            }

            // GUI Mode
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void ExecuteSilentMode(string[] args)
        {
            var executor = new OptimizationExecutor();
            UserRole role = PermissionManager.GetCurrentUserRole();

            try
            {
                if (args.Length > 1)
                {
                    switch (args[1])
                    {
                        case "-optimize":
                            if (args.Length > 2)
                            {
                                if (args[2] == "quick")
                                {
                                    executor.ExecuteQuickOptimize();
                                    LogResults(executor.Results, "Quick Optimization", role);
                                }
                                else if (args[2] == "deep")
                                {
                                    executor.ExecuteDeepOptimize();
                                    LogResults(executor.Results, "Deep Optimization", role);
                                }
                            }
                            break;

                        case "-check-device":
                            executor.CheckDeviceStatus();
                            LogResults(executor.Results, "Device Check", role);
                            break;

                        case "-repair-printer":
                            executor.ExecutePrinterRepair();
                            LogResults(executor.Results, "Printer Repair", role);
                            break;

                        case "-repair-network":
                            executor.ExecuteNetworkRepair();
                            LogResults(executor.Results, "Network Repair", role);
                            break;

                        case "-help":
                            DisplayHelp();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in silent mode: {ex.Message}";
                WriteToLog(logEntry);
            }
        }

        private static void LogResults(System.Collections.Generic.List<string> results, string operationType, UserRole role)
        {
            bool success = true;
            string details = "";

            foreach (var result in results)
            {
                WriteToLog(result);
                if (result.Contains("失败") || result.Contains("✗"))
                    success = false;
                if (result.Contains("完成") || result.Contains("成功"))
                    details = result;
            }

            AuditLogger.LogOperation(operationType, details, role, success);
        }

        private static void WriteToLog(string message)
        {
            string logDir = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                "logs");

            if (!System.IO.Directory.Exists(logDir))
                System.IO.Directory.CreateDirectory(logDir);

            string logFile = System.IO.Path.Combine(logDir, $"audit_{DateTime.Now:yyyy-MM-dd}.log");
            System.IO.File.AppendAllText(logFile, message + Environment.NewLine);
        }

        private static void DisplayHelp()
        {
            string help = @"
医院终端系统优化工具 - 命令行帮助

用法:
  HospitalTerminalOpt.exe                           # 启动GUI模式
  HospitalTerminalOpt.exe -silent -optimize quick  # 执行极速优化（静默模式）
  HospitalTerminalOpt.exe -silent -optimize deep   # 执行深度优化（静默模式）
  HospitalTerminalOpt.exe -silent -check-device    # 检查医疗设备
  HospitalTerminalOpt.exe -silent -repair-printer  # 修复打印故障
  HospitalTerminalOpt.exe -silent -repair-network  # 修复网络故障
  HospitalTerminalOpt.exe -help                    # 显示此帮助信息

参数说明:
  -silent              后台静默模式（不显示UI）
  -optimize quick      执行极速优化（清理临时文件）
  -optimize deep       执行深度优化（完整系统优化）
  -check-device        检查医疗外设状态
  -repair-printer      修复打印服务
  -repair-network      修复网络配置

配置文件:
  config/config.xml    系统配置文件（与exe同目录）
  logs/audit_*.log     审计日志（与exe同目录）

示例计划任务配置（Windows任务计划程序）:
  任务名称: 医院终端夜间深度优化
  触发器: 每天 22:30
  操作: 启动程序
  程序: HospitalTerminalOpt.exe
  参数: -silent -optimize deep
  工作目录: exe所在目录
";

            WriteToLog(help);
            Console.WriteLine(help);
        }
    }
}
