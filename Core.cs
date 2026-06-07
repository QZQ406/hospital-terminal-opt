using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Xml.Linq;

namespace HospitalTerminalOpt
{
    public enum UserRole
    {
        Operator,
        Viewer,
        Unknown
    }

    public static class PermissionManager
    {
        private static readonly List<string> AdminGroups = new List<string>()
        {
            "HOSPITAL\\IT-ADMIN",
            "HOSPITAL\\运维组",
            "Administrators"
        };

        private static readonly List<string> MedicalStaff = new List<string>()
        {
            "HOSPITAL\\医护人员",
            "HOSPITAL\\门诊医生",
            "HOSPITAL\\急诊科"
        };

        public static UserRole GetCurrentUserRole()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                foreach (var adminGroup in AdminGroups)
                    if (principal.IsInRole(adminGroup))
                        return UserRole.Operator;

                foreach (var medicalGroup in MedicalStaff)
                    if (principal.IsInRole(medicalGroup))
                        return UserRole.Viewer;

                return UserRole.Unknown;
            }
            catch
            {
                return UserRole.Unknown;
            }
        }

        public static bool CanExecuteOperation(UserRole role, string operationType)
        {
            if (role == UserRole.Operator)
                return true;
            if (role == UserRole.Viewer)
                return operationType == "Query" || operationType == "ViewLog";
            return false;
        }

        public static string GetCurrentUsername()
        {
            return WindowsIdentity.GetCurrent().Name;
        }
    }

    public static class ConfigurationManager
    {
        private static string _configDir;
        private static string _configFile;
        private static XDocument _config;

        public static void Initialize(string baseDir = null)
        {
            if (baseDir == null)
                baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _configDir = Path.Combine(baseDir, "config");
            _configFile = Path.Combine(_configDir, "config.xml");

            if (!Directory.Exists(_configDir))
                Directory.CreateDirectory(_configDir);

            LoadConfiguration();
        }

        private static void LoadConfiguration()
        {
            if (!File.Exists(_configFile))
                CreateDefaultConfig();

            try
            {
                _config = XDocument.Load(_configFile);
            }
            catch
            {
                CreateDefaultConfig();
                _config = XDocument.Load(_configFile);
            }
        }

        private static void CreateDefaultConfig()
        {
            var config = new XDocument(
                new XElement("HospitalTerminalConfig",
                    new XElement("BusinessPeakHours",
                        new XAttribute("startHour", "7"),
                        new XAttribute("endHour", "22"),
                        new XAttribute("disableDeepOpt", "true")),
                    new XElement("AutoRestart",
                        new XAttribute("enabled", "false")),
                    new XElement("ForcedUpdate",
                        new XAttribute("enabled", "false")),
                    new XElement("BackgroundPopups",
                        new XAttribute("enabled", "false")),
                    new XElement("OptimizationPolicy",
                        new XElement("QuickOpt",
                            new XAttribute("maxDurationSeconds", "300"),
                            new XAttribute("allowDuringPeakHours", "true")),
                        new XElement("DeepOpt",
                            new XAttribute("maxDurationSeconds", "1800"),
                            new XAttribute("allowDuringPeakHours", "false"))),
                    new XElement("DataProtection",
                        new XAttribute("protectPatientData", "true"),
                        new XAttribute("protectMedicalImages", "true")),
                    new XElement("NotificationPolicy",
                        new XAttribute("logRetentionDays", "90"))
                )
            );

            config.Save(_configFile);
        }

        public static bool IsBusinessPeakHours()
        {
            var elem = _config?.Root?.Element("BusinessPeakHours");
            if (elem == null) return false;

            if (!int.TryParse(elem.Attribute("startHour")?.Value, out int startHour)) startHour = 7;
            if (!int.TryParse(elem.Attribute("endHour")?.Value, out int endHour)) endHour = 22;

            int currentHour = DateTime.Now.Hour;
            return currentHour >= startHour && currentHour < endHour;
        }

        public static bool CanExecuteDeepOptimization()
        {
            var elem = _config?.Root?.Element("BusinessPeakHours");
            if (elem == null) return true;
            bool disableDeepOpt = elem.Attribute("disableDeepOpt")?.Value?.ToLower() == "true";
            return !disableDeepOpt || !IsBusinessPeakHours();
        }

        public static string GetConfigSummary()
        {
            return $@"系统配置概览
{'='} {new string('=', 50)}

【业务高峰保护】
★ 高峰时段: 07:00 - 22:00
★ 当前状态: {(IsBusinessPeakHours() ? "【业务高峰】深度优化已禁用" : "【低峰时段】所有功能开放")}

【核心原则执行情况】
✓ 零侵入: 仅优化系统底层
✓ 高稳定: 禁止自动重启
✓ 数据安全: 保护医疗数据
✓ 外设兼容: 保护医疗设备
✓ 权限可控: 区分用户角色";
        }
    }

    public static class BusinessDataProtection
    {
        private static readonly List<string> ForbiddenCleanPaths = new List<string>()
        {
            @"D:\HIS", @"D:\LIS", @"D:\PACS",
            @"E:\病历数据", @"E:\影像存档", @"C:\医疗系统",
            @"C:\Program Files\HIS", @"C:\Program Files\LIS", @"C:\Program Files (x86)\PACS",
            @"C:\ProgramData\HIS", @"C:\ProgramData\LIS", @"C:\ProgramData\医疗保险"
        };

        private static readonly List<string> ForbiddenKillProcess = new List<string>()
        {
            "hisclient", "hisserver", "lisclient", "lisserver",
            "pacsclient", "pacsserver", "medicalinsure",
            "cardreader", "printerdriver", "scannerdriver",
            "spool", "sqlserver", "oracle", "mysql"
        };

        private static readonly List<string> ForbiddenFileExtensions = new List<string>()
        {
            ".dcm", ".db", ".mdb", ".sqlite",
            ".csv", ".xml", ".ini", ".config", ".log"
        };

        public static bool IsPathProtected(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            string lowerPath = path.ToLower();
            foreach (var forbiddenPath in ForbiddenCleanPaths)
                if (lowerPath.Contains(forbiddenPath.ToLower()))
                    return true;
            return false;
        }

        public static bool IsProcessProtected(string processName)
        {
            if (string.IsNullOrEmpty(processName)) return false;
            string lowerProcess = processName.ToLower().Replace(".exe", "");
            foreach (var forbiddenProcess in ForbiddenKillProcess)
                if (lowerProcess.Contains(forbiddenProcess.ToLower()))
                    return true;
            return false;
        }

        public static bool IsFileProtected(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            string extension = Path.GetExtension(filePath).ToLower();
            foreach (var forbiddenExt in ForbiddenFileExtensions)
                if (extension == forbiddenExt)
                    return true;
            return false;
        }

        public static string GetProtectionSummary()
        {
            return $@"业务数据保护配置：
- 保护路径数: {ForbiddenCleanPaths.Count}
- 保护进程数: {ForbiddenKillProcess.Count}
- 保护文件类型: {ForbiddenFileExtensions.Count}

核心保护项:
✓ 患者病历不清理
✓ 医学影像不清理
✓ 医疗系统数据不清理
✓ 医疗外设驱动不卸载
✓ 业务进程不强制终止";
        }
    }

    public static class TimeSecurityLock
    {
        public static bool IsBusinessPeakTime()
        {
            return ConfigurationManager.IsBusinessPeakHours();
        }

        public static bool AllowDeepOpt()
        {
            return ConfigurationManager.CanExecuteDeepOptimization();
        }
    }
}
