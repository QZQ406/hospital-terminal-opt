using System;
using System.Windows.Forms;

namespace HospitalTerminalOpt
{
    public partial class MainForm : Form
    {
        private UserRole _currentUserRole;
        private OptimizationExecutor _executor;

        public MainForm()
        {
            InitializeComponent();
            _executor = new OptimizationExecutor();
            InitForm();
        }

        private void InitForm()
        {
            this.Text = "医院终端系统优化工具 V1.0 - " + PermissionManager.GetCurrentUsername();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(900, 600);

            _currentUserRole = PermissionManager.GetCurrentUserRole();

            AppendLog("════════════════════════════════════════════════════");
            AppendLog("医院终端系统优化工具 - 启动日志");
            AppendLog("════════════════════════════════════════════════════");
            AppendLog($"当前用户: {PermissionManager.GetCurrentUsername()}");
            AppendLog($"用户角色: {GetRoleDescription(_currentUserRole)}");
            AppendLog("");

            if (_currentUserRole == UserRole.Unknown)
            {
                AppendLog("⚠ 警告: 无法识别用户身份，系统以受限模式运行");
                DisableAllOperations();
                return;
            }

            if (_currentUserRole == UserRole.Viewer)
            {
                AppendLog("ℹ 提示: 您是医护人员，仅拥有查看权限");
                btnQuickOpt.Enabled = false;
                btnDeepOpt.Enabled = false;
                btnRepairPrint.Enabled = false;
                btnRepairNet.Enabled = false;
                btnViewConfig.Enabled = true;
                btnDeviceCheck.Enabled = true;
                btnViewAuditLog.Enabled = true;
            }

            AppendLog($"系统时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"业务状态: {(TimeSecurityLock.IsBusinessPeakTime() ? "【高峰时段】" : "【低峰时段】")}");
            AppendLog("");

            if (TimeSecurityLock.IsBusinessPeakTime())
            {
                btnDeepOpt.Enabled = false;
                btnDeepOpt.Text = "深度优化(夜间可用)";
                AppendLog("系统检测：当前为业务高峰时段(7:00-22:00)");
                AppendLog("✗ 深度优化功能已锁定（保护医疗业务）");
                AppendLog("✓ 极速优化和故障修复功能正常开放");
            }
            else
            {
                btnDeepOpt.Enabled = (_currentUserRole == UserRole.Operator);
                AppendLog("系统检测：当前为低峰时段");
                AppendLog("✓ 所有优化功能正常开放");
            }

            AppendLog("");
            AppendLog(MedicalDeviceManager.CheckMedicalDeviceStatus());
            AppendLog("");
            AppendLog("系统就绪。请选择要执行的操作...");
            AppendLog("════════════════════════════════════════════════════");
        }

        private string GetRoleDescription(UserRole role)
        {
            return role switch
            {
                UserRole.Operator => "运维人员 (完全权限)",
                UserRole.Viewer => "医护人员 (仅查看)",
                _ => "未知身份"
            };
        }

        private void DisableAllOperations()
        {
            btnQuickOpt.Enabled = false;
            btnDeepOpt.Enabled = false;
            btnRepairPrint.Enabled = false;
            btnRepairNet.Enabled = false;
            btnViewConfig.Enabled = false;
            btnDeviceCheck.Enabled = false;
        }

        private void AppendLog(string msg)
        {
            string log = $"[{DateTime.Now:HH:mm:ss}] {msg}\r\n";
            txtLog.AppendText(log);
            txtLog.ScrollToCaret();
        }

        private void ExecuteOperation(string operationName, Action operation)
        {
            if (!PermissionManager.CanExecuteOperation(_currentUserRole, "Execute"))
            {
                AppendLog($"✗ 权限拒绝: 您无权执行此操作");
                AuditLogger.LogOperation(operationName, "权限拒绝", _currentUserRole, false);
                return;
            }

            try
            {
                AppendLog($"===== 开始执行【{operationName}】=====");
                operation();
                AuditLogger.LogOperation(operationName, "执行成功", _currentUserRole, true);
                AppendLog($"===== 【{operationName}】执行完毕 =====");
            }
            catch (Exception ex)
            {
                AppendLog($"✗ 执行出错: {ex.Message}");
                AuditLogger.LogOperation(operationName, $"执行失败: {ex.Message}", _currentUserRole, false);
            }
        }

        private void btnQuickOpt_Click(object sender, EventArgs e)
        {
            ExecuteOperation("日间极速优化", () =>
            {
                _executor.ExecuteQuickOptimize();
                foreach (var result in _executor.Results)
                    AppendLog(result);
            });
        }

        private void btnDeepOpt_Click(object sender, EventArgs e)
        {
            if (!TimeSecurityLock.AllowDeepOpt())
            {
                AppendLog("✗ 深度优化被业务高峰保护机制禁用");
                AuditLogger.LogOperation("夜间深度优化", "被业务高峰保护禁用", _currentUserRole, false);
                return;
            }

            ExecuteOperation("夜间深度优化", () =>
            {
                _executor.ExecuteDeepOptimize();
                foreach (var result in _executor.Results)
                    AppendLog(result);
            });
        }

        private void btnRepairPrint_Click(object sender, EventArgs e)
        {
            ExecuteOperation("打印故障修复", () =>
            {
                _executor.ExecutePrinterRepair();
                foreach (var result in _executor.Results)
                    AppendLog(result);
            });
        }

        private void btnRepairNet_Click(object sender, EventArgs e)
        {
            ExecuteOperation("内网网络修复", () =>
            {
                _executor.ExecuteNetworkRepair();
                foreach (var result in _executor.Results)
                    AppendLog(result);
            });
        }

        private void btnViewConfig_Click(object sender, EventArgs e)
        {
            AppendLog("════════════════════════════════════════════════════");
            AppendLog("系统配置信息");
            AppendLog("════════════════════════════════════════════════════");
            AppendLog(ConfigurationManager.GetConfigSummary());
            AppendLog("════════════════════════════════════════════════════");
            AuditLogger.LogOperation("查看配置", "用户查看了系统配置", _currentUserRole, true);
        }

        private void btnDeviceCheck_Click(object sender, EventArgs e)
        {
            _executor.CheckDeviceStatus();
            foreach (var result in _executor.Results)
                AppendLog(result);
            AuditLogger.LogOperation("设备检查", "用户检查了医疗设备状态", _currentUserRole, true);
        }

        private void btnViewAuditLog_Click(object sender, EventArgs e)
        {
            AppendLog("════════════════════════════════════════════════════");
            AppendLog("最近7天的审计日志");
            AppendLog("════════════════════════════════════════════════════");
            AppendLog(AuditLogger.GetRecentLogs(7));
            AppendLog("════════════════════════════════════════════════════");
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            AppendLog("日志窗口已清空（审计日志已保存到磁盘）");
        }
    }
}
