using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Text;

namespace HospitalTerminalOpt
{
    public static class SystemCleaner
    {
        public static string CleanTempFiles()
        {
            int delCount = 0;
            int protectedCount = 0;
            try
            {
                string tempDir = Path.GetTempPath();
                if (Directory.Exists(tempDir))
                {
                    foreach (var file in Directory.GetFiles(tempDir))
                    {
                        if (BusinessDataProtection.IsPathProtected(file) || BusinessDataProtection.IsFileProtected(file))
                        {
                            protectedCount++;
                            continue;
                        }

                        try
                        {
                            File.Delete(file);
                            delCount++;
                        }
                        catch { }
                    }
                }

                string prefetch = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch");
                if (Directory.Exists(prefetch))
                {
                    foreach (var file in Directory.GetFiles(prefetch))
                    {
                        if (!BusinessDataProtection.IsFileProtected(file))
                        {
                            try
                            {
                                File.Delete(file);
                                delCount++;
                            }
                            catch { }
                        }
                    }
                }

                string result = $"临时文件清理完成，共删除 {delCount} 个文件";
                if (protectedCount > 0)
                    result += $"，保护 {protectedCount} 个医疗数据文件";

                return result;
            }
            catch (Exception ex)
            {
                return $"清理异常：{ex.Message}";
            }
        }
    }

    public static class PrinterTool
    {
        public static string ResetPrinter()
        {
            try
            {
                string printSpool = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    @"System32\spool\PRINTERS");

                if (TimeSecurityLock.IsBusinessPeakTime())
                {
                    if (Directory.Exists(printSpool))
                    {
                        foreach (var file in Directory.GetFiles(printSpool))
                        {
                            try { File.Delete(file); }
                            catch { }
                        }
                    }
                    return "【日间模式】打印队列清理完成，未重启打印服务（保护医疗运行）";
                }
                else
                {
                    Process.Start("net", "stop spooler").WaitForExit();
                    if (Directory.Exists(printSpool))
                    {
                        foreach (var file in Directory.GetFiles(printSpool))
                        {
                            try { File.Delete(file); }
                            catch { }
                        }
                    }
                    Process.Start("net", "start spooler").WaitForExit();
                    return "【夜间模式】打印服务重启 + 队列清理完成";
                }
            }
            catch (Exception ex)
            {
                return $"打印修复失败：{ex.Message}";
            }
        }
    }

    public static class NetworkTool
    {
        public static string RepairNetwork()
        {
            try
            {
                Process.Start("ipconfig", "/flushdns").WaitForExit();
                Process.Start("ipconfig", "/release").WaitForExit();
                Process.Start("ipconfig", "/renew").WaitForExit();
                Process.Start("netsh", "winsock reset").WaitForExit();
                return "网络修复完成：DNS刷新、网络组件重置成功";
            }
            catch (Exception ex)
            {
                return $"网络修复失败：{ex.Message}";
            }
        }
    }

    public class OptimizationExecutor
    {
        public List<string> Results { get; private set; } = new List<string>();

        public void ExecuteQuickOptimize()
        {
            Results.Clear();
            Results.Add("===== 开始执行【极速优化】=====");
            Results.Add(SystemCleaner.CleanTempFiles());
            Results.Add(PrinterTool.ResetPrinter());
            Results.Add("===== 极速优化执行完毕 =====");
        }

        public void ExecuteDeepOptimize()
        {
            if (!TimeSecurityLock.AllowDeepOpt())
            {
                Results.Clear();
                Results.Add("✗ 深度优化被业务高峰保护机制禁用");
                return;
            }

            Results.Clear();
            Results.Add("===== 开始执行【深度优化】=====");
            Results.Add(SystemCleaner.CleanTempFiles());
            Results.Add(PrinterTool.ResetPrinter());
            Results.Add("===== 深度优化执行完毕 =====");
        }

        public void ExecutePrinterRepair()
        {
            Results.Clear();
            Results.Add("开始执行【打印故障修复】");
            Results.Add(PrinterTool.ResetPrinter());
        }

        public void ExecuteNetworkRepair()
        {
            Results.Clear();
            Results.Add("开始执行【内网网络修复】");
            Results.Add(NetworkTool.RepairNetwork());
        }

        public void CheckDeviceStatus()
        {
            Results.Clear();
            Results.Add("════════════════════════════════════════════════════");
            Results.Add("医疗设备检查");
            Results.Add("════════════════════════════════════════════════════");
            Results.Add(MedicalDeviceManager.CheckMedicalDeviceStatus());
            Results.Add("");
            Results.Add(MedicalDeviceManager.GetDeviceProtectionSummary());
            Results.Add("════════════════════════════════════════════════════");
        }
    }
}
