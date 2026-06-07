namespace HospitalTerminalOpt
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnQuickOpt = new System.Windows.Forms.Button();
            this.btnDeepOpt = new System.Windows.Forms.Button();
            this.btnRepairPrint = new System.Windows.Forms.Button();
            this.btnRepairNet = new System.Windows.Forms.Button();
            this.btnViewConfig = new System.Windows.Forms.Button();
            this.btnDeviceCheck = new System.Windows.Forms.Button();
            this.btnViewAuditLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.btnQuickOpt.Location = new System.Drawing.Point(15, 15);
            this.btnQuickOpt.Name = "btnQuickOpt";
            this.btnQuickOpt.Size = new System.Drawing.Size(120, 35);
            this.btnQuickOpt.TabIndex = 0;
            this.btnQuickOpt.Text = "极速优化";
            this.btnQuickOpt.UseVisualStyleBackColor = true;
            this.btnQuickOpt.Click += new System.EventHandler(this.btnQuickOpt_Click);

            this.btnDeepOpt.Location = new System.Drawing.Point(140, 15);
            this.btnDeepOpt.Name = "btnDeepOpt";
            this.btnDeepOpt.Size = new System.Drawing.Size(120, 35);
            this.btnDeepOpt.TabIndex = 1;
            this.btnDeepOpt.Text = "深度优化";
            this.btnDeepOpt.UseVisualStyleBackColor = true;
            this.btnDeepOpt.Click += new System.EventHandler(this.btnDeepOpt_Click);

            this.btnRepairPrint.Location = new System.Drawing.Point(265, 15);
            this.btnRepairPrint.Name = "btnRepairPrint";
            this.btnRepairPrint.Size = new System.Drawing.Size(120, 35);
            this.btnRepairPrint.TabIndex = 2;
            this.btnRepairPrint.Text = "打印修复";
            this.btnRepairPrint.UseVisualStyleBackColor = true;
            this.btnRepairPrint.Click += new System.EventHandler(this.btnRepairPrint_Click);

            this.btnRepairNet.Location = new System.Drawing.Point(390, 15);
            this.btnRepairNet.Name = "btnRepairNet";
            this.btnRepairNet.Size = new System.Drawing.Size(120, 35);
            this.btnRepairNet.TabIndex = 3;
            this.btnRepairNet.Text = "网络修复";
            this.btnRepairNet.UseVisualStyleBackColor = true;
            this.btnRepairNet.Click += new System.EventHandler(this.btnRepairNet_Click);

            this.btnViewConfig.Location = new System.Drawing.Point(515, 15);
            this.btnViewConfig.Name = "btnViewConfig";
            this.btnViewConfig.Size = new System.Drawing.Size(110, 35);
            this.btnViewConfig.TabIndex = 4;
            this.btnViewConfig.Text = "查看配置";
            this.btnViewConfig.UseVisualStyleBackColor = true;
            this.btnViewConfig.Click += new System.EventHandler(this.btnViewConfig_Click);

            this.btnDeviceCheck.Location = new System.Drawing.Point(630, 15);
            this.btnDeviceCheck.Name = "btnDeviceCheck";
            this.btnDeviceCheck.Size = new System.Drawing.Size(110, 35);
            this.btnDeviceCheck.TabIndex = 5;
            this.btnDeviceCheck.Text = "设备检查";
            this.btnDeviceCheck.UseVisualStyleBackColor = true;
            this.btnDeviceCheck.Click += new System.EventHandler(this.btnDeviceCheck_Click);

            this.btnViewAuditLog.Location = new System.Drawing.Point(745, 15);
            this.btnViewAuditLog.Name = "btnViewAuditLog";
            this.btnViewAuditLog.Size = new System.Drawing.Size(110, 35);
            this.btnViewAuditLog.TabIndex = 6;
            this.btnViewAuditLog.Text = "审计日志";
            this.btnViewAuditLog.UseVisualStyleBackColor = true;
            this.btnViewAuditLog.Click += new System.EventHandler(this.btnViewAuditLog_Click);

            this.btnClearLog.Location = new System.Drawing.Point(860, 15);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(90, 35);
            this.btnClearLog.TabIndex = 7;
            this.btnClearLog.Text = "清空显示";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);

            this.txtLog.Location = new System.Drawing.Point(15, 60);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(935, 480);
            this.txtLog.TabIndex = 8;
            this.txtLog.ReadOnly = true;
            this.txtLog.Font = new System.Drawing.Font("Courier New", 9F);

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 560);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnViewAuditLog);
            this.Controls.Add(this.btnDeviceCheck);
            this.Controls.Add(this.btnViewConfig);
            this.Controls.Add(this.btnRepairNet);
            this.Controls.Add(this.btnRepairPrint);
            this.Controls.Add(this.btnDeepOpt);
            this.Controls.Add(this.btnQuickOpt);
            this.Name = "MainForm";
            this.Text = "医院终端系统优化工具";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnQuickOpt;
        private System.Windows.Forms.Button btnDeepOpt;
        private System.Windows.Forms.Button btnRepairPrint;
        private System.Windows.Forms.Button btnRepairNet;
        private System.Windows.Forms.Button btnViewConfig;
        private System.Windows.Forms.Button btnDeviceCheck;
        private System.Windows.Forms.Button btnViewAuditLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnClearLog;
    }
}
