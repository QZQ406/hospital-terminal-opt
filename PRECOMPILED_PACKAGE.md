# 医院终端优化工具 - 预编译部署包

## 📦 包内容说明

本部署包已为医疗终端优化，无需编译环境即可直接使用。

### 包含文件

```
HospitalTerminalOpt-v1.0.0-Precompiled/
├─ HospitalTerminalOpt.exe              主程序（单文件，<10MB）
├─ config/                              配置目录（自动创建）
├─ logs/                                日志目录（自动创建）
├─ 快速开始.txt                         启动指南
├─ 部署清单.txt                         部署前检查
└─ 常见问题.txt                         故障排除
```

## 🚀 快速启动

### 前台运维模式（GUI）

**方式1：双击运行**
```
直接双击 HospitalTerminalOpt.exe
```

**方式2：命令行启动**
```bash
HospitalTerminalOpt.exe
```

**预期结果：**
- 窗体启动显示UI界面
- 自动创建config和logs目录
- 显示用户身份和系统状态
- 可点击按钮执行各项操作

### 后台静默模式（自动化）

**极速优化（快速清理）**
```bash
HospitalTerminalOpt.exe -silent -optimize quick
```

**深度优化（完整清理，夜间）**
```bash
HospitalTerminalOpt.exe -silent -optimize deep
```

**设备检查**
```bash
HospitalTerminalOpt.exe -silent -check-device
```

**查看帮助**
```bash
HospitalTerminalOpt.exe -help
```

## ✅ 系统要求检查

在运行前，请确保医疗终端满足以下条件：

### 操作系统
- [ ] Windows 7 SP1 或更高版本
- [ ] Windows 8.1
- [ ] Windows 10
- [ ] Windows 11

### 运行时环境
- [ ] .NET Framework 4.6.2+ 已安装

**检查.NET版本（运行以下命令）：**
```powershell
reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release
```

**如果提示404或数值<394802，需安装.NET Framework 4.6.2：**
- 下载地址：https://www.microsoft.com/zh-cn/download/details.aspx?id=53344
- 安装后重启电脑
- 重新检查版本号

### 权限要求
- [ ] 具有管理员权限（部分操作需要）
- [ ] 属于相应的Windows域组（可选，用于权限控制）

### 磁盘空间
- [ ] 至少50MB剩余空间（用于临时文件和日志）

## 📋 部署清单

### 部署前（Pre-Deployment）

- [ ] 验证.NET Framework 4.6.2+已安装
- [ ] 备份当前系统配置
- [ ] 检查磁盘空间充足
- [ ] 获得IT部门审批

### 部署时（Deployment）

- [ ] 将HospitalTerminalOpt.exe复制到目标位置
- [ ] 创建config和logs空目录
- [ ] 测试前台GUI模式
- [ ] 测试后台静默模式
- [ ] 配置Windows计划任务（可选）

### 部署后（Post-Deployment）

- [ ] 验证应用正常启动
- [ ] 检查医疗设备识别
- [ ] 验证权限管理生效
- [ ] 查看审计日志记录
- [ ] 向医护人员通知部署完成

## 🔧 常见问题快速解决

### Q1: 应用无法启动

**症状：** 双击exe无反应或显示错误信息

**解决方案：**
1. 检查.NET Framework版本（见上文）
2. 以管理员身份运行
3. 查看 `logs/` 目录中的错误日志
4. 尝试从命令行运行查看错误信息：
   ```bash
   HospitalTerminalOpt.exe
   ```

### Q2: 提示"缺少.NET Framework"

**解决方案：**
1. 安装.NET Framework 4.6.2运行时
2. 重启电脑
3. 重新运行应用

### Q3: 医疗设备无法识别

**症状：** 设备检查显示"检查失败"

**解决方案：**
1. 检查WMI服务是否运行：
   ```bash
   net start winmgmt
   ```
2. 重新连接医疗设备
3. 在设备管理器中检查驱动状态
4. 查看审计日志中的详细错误

### Q4: 权限提示"拒绝"

**解决方案：**
1. 确认当前用户属于相应的AD组
2. 重新登录Windows
3. 尝试以管理员身份运行

## 📝 配置说明

### 配置文件位置
```
HospitalTerminalOpt.exe 同目录/config/config.xml
```

### 首次运行自动配置
- 应用首次启动时自动生成默认config.xml
- 自动创建logs日志目录
- 无需手动配置即可使用

### 自定义配置（可选）
编辑 `config/config.xml` 可以：
- 修改业务高峰时段（startHour/endHour）
- 调整优化时间限制
- 更改日志保留天数

## 📊 日志和审计

### 日志位置
```
HospitalTerminalOpt.exe 同目录/logs/audit_YYYY-MM-DD.log
```

### 查看日志
1. **在GUI中查看：** 点击"审计日志"按钮
2. **手动查看：** 打开 `logs/` 目录下的日志文件
3. **命令行查看：**
   ```bash
   type logs\audit_2024-06-07.log
   ```

### 日志内容示例
```
[2024-06-07 22:30:45] | User: HOSPITAL\IT-ADMIN | Role: Operator | Operation: Deep Optimization | Status: SUCCESS | Details: 临时文件清理完成，共删除150个文件
```

## 🎯 后台定时优化（Windows计划任务）

### 配置夜间深度优化

**步骤1：打开任务计划程序**
```
控制面板 > 管理工具 > 任务计划程序
```

**步骤2：创建基本任务**
- 名称：医院终端夜间深度优化
- 描述：每晚22:30执行系统深度优化

**步骤3：设置触发器**
- 触发器类型：每天
- 时间：22:30
- 重复频率：每1天

**步骤4：设置操作**
```
程序或脚本：D:\HospitalTools\HospitalTerminalOpt.exe
添加参数：-silent -optimize deep
起始于：D:\HospitalTools\
```

**步骤5：完成**
- 勾选"仅当计算机使用电池电源时才启动任务"（可选）
- 勾选"如果任务正在运行，则不启动新实例"
- 设置优先级为"低"

### PowerShell快速配置

```powershell
$trigger = New-ScheduledTaskTrigger -Daily -At "22:30"
$action = New-ScheduledTaskAction -Execute "D:\HospitalTools\HospitalTerminalOpt.exe" -Argument "-silent -optimize deep"
$settings = New-ScheduledTaskSettingsSet -RunOnlyIfIdle
Register-ScheduledTask -TaskName "医院终端夜间深度优化" -Trigger $trigger -Action $action -Settings $settings -RunLevel Highest
```

## 🔐 安全提示

### 数据保护
- ✓ 患者病历完全保护（不会清理）
- ✓ 医学影像完全保护（不会删除）
- ✓ 医疗系统数据完全保护
- ✓ 医疗设备驱动完全保护

### 权限控制
- 运维人员（IT-ADMIN）：完全权限
- 医护人员：仅查看权限
- 所有操作自动审计记录

### 业务高峰保护
- 工作时间（7:00-22:00）自动禁用深度优化
- 打印服务在高峰期仅清理队列，不重启
- 医疗运行不受影响

## 📞 技术支持

### 获取帮助
1. 查看 `logs/` 目录中的错误日志
2. 参考本文档的"常见问题"部分
3. 运行 `HospitalTerminalOpt.exe -help` 显示命令行帮助
4. 联系IT运维部门

### 反馈问题时提供
- Windows版本和.NET Framework版本
- 完整的错误信息或日志内容
- 执行的具体操作步骤
- 医疗设备型号（如有）

## 📌 重要提示

1. **不要删除config和logs目录** - 这些是应用正常运行所必需的
2. **定期备份logs目录** - 审计日志很重要，定期备份保留
3. **不要修改exe文件** - 任何修改会导致应用无法运行
4. **保持.NET Framework更新** - 定期检查更新以获得安全补丁

## 版本信息

- **应用版本：** 1.0.0
- **发布日期：** 2024-06-07
- **最低要求：** Windows 7 SP1 + .NET Framework 4.6.2
- **文件大小：** < 10MB

---

祝部署顺利！如有任何问题，请参考上述文档或联系IT运维部门。
