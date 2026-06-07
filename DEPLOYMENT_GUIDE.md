# 医院终端优化工具 - 单文件部署指南

## 部署概述

医院终端优化工具已优化为轻量级单文件可执行程序，支持三种部署方案，满足不同医疗机构的需求。

**核心特性：**
- ✓ 单个exe文件（< 10MB）
- ✓ 免安装绿色软件
- ✓ U盘便携运行
- ✓ 内网共享部署
- ✓ 前台手动运维 + 后台定时优化双模式
- ✓ 零依赖（仅需.NET Framework 4.6.2+）

---

## 部署方案

### 方案A：U盘便携部署

**适用场景：** 需要在多台医疗终端间切换使用，便于维护人员携带。

**部署步骤：**

1. **准备U盘**
   - 准备一个8GB或更大容量的U盘
   - 格式化为NTFS格式

2. **创建目录结构**
   ```
   U盘根目录\
   ├─ HospitalTerminalOpt.exe
   ├─ config\                    (空目录，首次运行自动生成配置)
   ├─ logs\                      (空目录，自动生成审计日志)
   └─ 部署说明.txt
   ```

3. **复制文件**
   - 将编译后的 `HospitalTerminalOpt.exe` 复制到U盘根目录
   - 创建 `config` 和 `logs` 空目录

4. **首次运行**
   ```bash
   # 连接U盘到医疗终端
   # 双击运行 HospitalTerminalOpt.exe
   # 系统自动创建配置文件和日志目录
   ```

5. **后续使用**
   - 直接在U盘上运行exe
   - 配置和日志保存在U盘上
   - 下次插入自动读取之前的配置和日志

**优点：**
- 便携性强，一个U盘可服务多台终端
- 配置和日志随身携带
- 无需在终端安装软件

**注意事项：**
- U盘写入速度可能较慢，深度优化耗时会增加
- 确保U盘定期备份日志
- 避免在优化过程中拔出U盘

---

### 方案B：内网共享部署

**适用场景：** 医院内网有统一的工具共享服务器，多台终端共享使用。

**部署步骤：**

1. **服务器配置**
   ```
   \\医院服务器\工具共享\HospitalTerminalOpt\
   ├─ HospitalTerminalOpt.exe
   ├─ config\
   ├─ logs\
   └─ 部署说明.txt
   ```

2. **共享文件夹权限设置**
   - 管理员：完全控制
   - 普通用户：读取 + 执行
   - 所有用户可读写 `config/` 和 `logs/` 目录

3. **终端用户使用**
   ```bash
   # 方式1：直接访问网络路径
   \\医院服务器\工具共享\HospitalTerminalOpt\HospitalTerminalOpt.exe
   
   # 方式2：映射网络驱动器后使用
   Z:\HospitalTerminalOpt.exe
   ```

4. **配置同步**
   - 所有终端共享同一个config.xml
   - 所有审计日志保存在服务器的logs目录

**优点：**
- 集中管理，配置统一
- 审计日志集中存储
- 便于IT管理员监控和维护
- 节省本地存储空间

**缺点：**
- 依赖网络连接
- 网络延迟可能影响性能
- 需要配置网络共享权限

**权限配置示例（Windows Server）：**
```powershell
# 创建共享
New-SmbShare -Name "HospitalTools" -Path "D:\工具共享"

# 设置ACL权限
$acl = Get-Acl "D:\工具共享"
$ace = New-Object System.Security.AccessControl.FileSystemAccessRule(
    "HOSPITAL\医护人员", 
    "ReadAndExecute", 
    "ContainerInherit,ObjectInherit", 
    "None", 
    "Allow"
)
$acl.SetAccessRule($ace)
Set-Acl "D:\工具共享" $acl
```

---

### 方案C：本地安装部署

**适用场景：** 医疗终端数量有限，IT人员可逐台安装配置。

**部署步骤：**

1. **本地创建目录**
   ```
   每台终端的D:\HospitalTools\
   ├─ HospitalTerminalOpt.exe
   ├─ config\
   └─ logs\
   ```

2. **使用组策略分发（可选）**
   - 通过Active Directory组策略自动分发exe文件
   - 配置统一的GPO设置

3. **创建快捷方式**
   - 在桌面或开始菜单创建快捷方式
   - 快捷方式指向 `D:\HospitalTools\HospitalTerminalOpt.exe`

4. **创建计划任务**（后台自动优化）
   ```
   参考下文：计划任务配置
   ```

**优点：**
- 本地运行速度快
- 配置和日志本地存储
- 无网络依赖
- 可与其他软件集成

**缺点：**
- 需要逐台部署
- 配置管理分散
- 审计日志分散在各终端

---

## 后台定时优化配置

### Windows计划任务配置

**场景1：夜间深度优化（推荐）**

1. **打开任务计划程序**
   ```
   Windows 7/8/10/11: 控制面板 > 管理工具 > 任务计划程序
   ```

2. **创建基本任务**
   - 名称：`医院终端夜间深度优化`
   - 描述：`每天夜间10:30执行系统深度优化`

3. **设置触发器**
   - 触发器类型：`每天`
   - 时间：`22:30`（下午10时30分）
   - 重复频率：`每1天`

4. **设置操作**
   ```
   程序或脚本: D:\HospitalTools\HospitalTerminalOpt.exe
   添加参数: -silent -optimize deep
   起始于: D:\HospitalTools\
   ```

5. **设置条件**
   - [ ] 仅当计算机使用电池电源时才启动任务
   - [x] 如果任务正在运行，则不启动新实例
   - [x] 任务完成后自动删除

6. **设置设置**
   - 优先级：低
   - 允许按需运行任务：是
   - 运行权限：SYSTEM（或指定运维账户）

**场景2：日间极速优化（可选）**

相同配置，但：
- 时间：`09:00`
- 参数：`-silent -optimize quick`

**使用PowerShell创建任务：**

```powershell
# 夜间深度优化
$trigger = New-ScheduledTaskTrigger -Daily -At "22:30"
$action = New-ScheduledTaskAction -Execute "D:\HospitalTools\HospitalTerminalOpt.exe" -Argument "-silent -optimize deep"
$settings = New-ScheduledTaskSettingsSet -RunOnlyIfIdle -IdleDuration 0:05:00 -StartWhenAvailable
Register-ScheduledTask -TaskName "医院终端夜间深度优化" -Trigger $trigger -Action $action -Settings $settings -RunLevel Highest
```

---

## 命令行参数参考

### GUI模式（前台运维）

```bash
HospitalTerminalOpt.exe
```
- 启动图形用户界面
- 显示所有功能按钮
- 实时显示执行结果
- 完整的权限检查

### 静默模式参数

#### 极速优化
```bash
HospitalTerminalOpt.exe -silent -optimize quick
```
- 执行时间：约5分钟
- 适用场景：业务时段快速优化
- 操作内容：清理临时文件、打印队列

#### 深度优化
```bash
HospitalTerminalOpt.exe -silent -optimize deep
```
- 执行时间：约30分钟
- 适用场景：夜间自动优化
- 操作内容：完整系统优化、服务重启
- 业务高峰自动跳过

#### 设备检查
```bash
HospitalTerminalOpt.exe -silent -check-device
```
- 检查医疗外设驱动状态
- 输出到审计日志
- 无UI显示

#### 打印修复
```bash
HospitalTerminalOpt.exe -silent -repair-printer
```
- 修复打印服务
- 清理打印队列
- 支持业务高峰保护

#### 网络修复
```bash
HospitalTerminalOpt.exe -silent -repair-network
```
- 刷新DNS缓存
- 重新配置IP地址
- 重置网络栈

#### 显示帮助
```bash
HospitalTerminalOpt.exe -help
```
- 显示完整帮助信息
- 列出所有命令行参数

---

## 配置文件说明

### 配置文件位置
```
exe同目录/config/config.xml
```

### 默认配置内容

```xml
<?xml version="1.0"?>
<HospitalTerminalConfig>
  <!-- 业务高峰时段保护 -->
  <BusinessPeakHours startHour="7" endHour="22" disableDeepOpt="true"/>
  
  <!-- 禁止自动重启 -->
  <AutoRestart enabled="false"/>
  
  <!-- 禁止强制更新 -->
  <ForcedUpdate enabled="false"/>
  
  <!-- 禁止后台弹窗 -->
  <BackgroundPopups enabled="false"/>
  
  <!-- 优化时间限制 -->
  <OptimizationPolicy>
    <QuickOpt maxDurationSeconds="300" allowDuringPeakHours="true"/>
    <DeepOpt maxDurationSeconds="1800" allowDuringPeakHours="false"/>
  </OptimizationPolicy>
  
  <!-- 数据保护 -->
  <DataProtection protectPatientData="true" protectMedicalImages="true"/>
  
  <!-- 日志策略 -->
  <NotificationPolicy logRetentionDays="90"/>
</HospitalTerminalConfig>
```

### 自定义配置

#### 修改业务高峰时段
```xml
<BusinessPeakHours startHour="8" endHour="18" disableDeepOpt="true"/>
```

#### 调整优化时间限制
```xml
<QuickOpt maxDurationSeconds="600" allowDuringPeakHours="true"/>
<DeepOpt maxDurationSeconds="2400" allowDuringPeakHours="false"/>
```

#### 更改日志保留天数
```xml
<NotificationPolicy logRetentionDays="180"/>
```

---

## 审计日志管理

### 日志位置
```
exe同目录/logs/audit_YYYY-MM-DD.log
```

### 日志格式
```
[2024-06-07 22:30:45] | User: HOSPITAL\SYSTEM | Role: Operator | Operation: Deep Optimization | Status: SUCCESS | Details: 临时文件清理完成，共删除 150 个文件
```

### 日志查看

**在GUI中查看：**
- 点击"审计日志"按钮
- 显示最近7天的所有操作

**手动查看：**
```powershell
# 查看今天的日志
Get-Content "D:\HospitalTools\logs\audit_2024-06-07.log"

# 查看最近7天的日志
Get-ChildItem "D:\HospitalTools\logs\audit_*.log" -PipelineVariable logfile | 
  Where-Object {(Get-Date) - $_.LastWriteTime -le [TimeSpan]"7 days"} |
  Foreach-Object {Get-Content $_}

# 搜索特定操作
Select-String "Deep Optimization" "D:\HospitalTools\logs\audit_*.log"
```

### 日志备份

**定期备份脚本：**
```powershell
# 每周一备份日志
$sourceDir = "D:\HospitalTools\logs"
$backupDir = "\\医院服务器\日志备份\终端优化工具\$env:COMPUTERNAME"
$timestamp = (Get-Date).ToString("yyyyMMdd_HHmmss")

if (!(Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir -Force
}

Copy-Item "$sourceDir\*" -Destination "$backupDir\backup_$timestamp" -Recurse
```

---

## 故障排除

### 应用无法启动

**症状：** 双击exe后无反应

**解决方案：**
1. 检查.NET Framework版本
   ```bash
   # 检查已安装的.NET Framework版本
   reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP"
   ```
   需要4.6.2或更高版本

2. 以管理员身份运行
   - 右键点击exe
   - 选择"以管理员身份运行"

3. 查看审计日志
   - 检查 `logs/` 目录下的日志文件
   - 查找错误信息

### 医疗设备无法识别

**症状：** 设备检查显示"检查失败"

**解决方案：**
1. 检查WMI服务
   ```bash
   # Windows命令行
   net start winmgmt
   ```

2. 重新安装设备驱动
   - 设备管理器 > 找到异常设备
   - 右键 > 更新驱动程序

3. 检查审计日志中的具体错误

### 权限识别错误

**症状：** 显示"未知身份"或权限不匹配

**解决方案：**
1. 确认Windows域组配置
   ```powershell
   # 检查当前用户所属的组
   whoami /groups
   ```

2. 确认AD组成员关系
   - 用户需加入以下组之一：
     - `HOSPITAL\IT-ADMIN` (运维人员)
     - `HOSPITAL\医护人员` (医护人员)
     - `Administrators` (本地管理员)

3. 重新登录Windows

### 配置文件错误

**症状：** 应用启动时配置加载失败

**解决方案：**
1. 删除并重新生成配置
   ```bash
   # 删除config目录
   rmdir /s /q config
   # 重新运行应用
   HospitalTerminalOpt.exe
   ```

2. 检查目录权限
   ```bash
   # 确保用户对config和logs目录有读写权限
   icacls "config" /grant "%USERNAME%:(OI)(CI)F"
   icacls "logs" /grant "%USERNAME%:(OI)(CI)F"
   ```

### 静默模式无输出

**症状：** 执行 `-silent` 命令但无反应

**解决方案：**
1. 检查审计日志
   ```bash
   type logs\audit_YYYY-MM-DD.log
   ```

2. 使用管理员命令行
   ```bash
   # 以管理员身份打开cmd或PowerShell
   HospitalTerminalOpt.exe -silent -optimize quick
   ```

---

## 安全性考虑

### 用户权限

**操作权限：**
- 极速优化：需要Operator角色
- 深度优化：需要Operator角色 + 非业务高峰
- 设备检查：所有用户可查看
- 审计日志：所有用户可查看

### 数据保护

**受保护的医疗数据：**
- 患者病历目录：`D:\HIS`, `E:\病历数据`
- 医学影像：`D:\PACS`, `E:\影像存档`
- 医疗系统程序
- 业务进程：HIS、LIS、PACS相关进程

### 审计与监控

- 所有操作自动记录审计日志
- 包含用户身份、操作类型、执行结果
- 日志保留90天（可配置）

---

## 最佳实践

### 日常维护

1. **日间（7:00-22:00）**
   - 运行"极速优化"清理临时文件
   - 频率：可根据需要手动执行
   - 时间：业务空隙执行

2. **夜间（22:00-7:00）**
   - 配置计划任务运行"深度优化"
   - 时间：22:30（业务结束后）
   - 操作：自动执行，无需人工干预

### 审计日志管理

- 每月备份一次审计日志
- 定期检查日志中的异常记录
- 保留至少3个月的日志

### 配置版本控制

- 备份原始的config.xml
- 修改配置前创建副本
- 记录配置变更历史

### 监控和告警

- 定期检查设备异常告警
- 及时处理驱动程序错误
- 监控系统资源占用情况

---

## 技术规格

| 项目 | 规格 |
|------|------|
| 文件大小 | < 10MB |
| 启动时间 | < 1秒 |
| 内存占用（GUI模式） | < 100MB |
| 内存占用（静默模式） | < 50MB |
| 极速优化耗时 | 约5分钟 |
| 深度优化耗时 | 约30分钟 |
| 目标OS | Windows 7 SP1+ |
| 依赖项 | .NET Framework 4.6.2+ |
| 部署方式 | 绿色免安装 |

---

## 联系与支持

若遇到部署问题，请：

1. 查看审计日志获取详细错误信息
2. 参考故障排除部分
3. 联系IT运维部门
4. 保存完整的日志文件供技术分析
