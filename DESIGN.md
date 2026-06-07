# 医院终端系统优化工具 - 核心设计原则实现文档

## 项目概述
医院终端系统优化工具是一个专为医疗业务场景设计的系统维护工具，严格遵循零侵入、高稳定、数据安全、外设兼容、权限可控的五大核心原则。

---

## 核心设计原则实现

### 1. 零侵入 ✓
**原则**: 不篡改医疗业务系统、数据库、HIS/LIS/PACS 核心程序，仅优化系统底层。

**实现方案**:
- **BusinessDataProtection.cs**: 
  - 维护3个保护清单：受保护路径、受保护进程、受保护文件类型
  - 清理任何文件前都检查是否在保护清单中
  - HIS、LIS、PACS数据目录全面保护
  
- **CommonHelper.cs - SystemCleaner**:
  - 仅清理 `%TEMP%` 和 `Prefetch` 目录
  - 每个文件都经过双重检查（路径检查 + 扩展名检查）
  
- **CommonHelper.cs - PrinterTool**:
  - 打印队列清理但不卸载驱动
  - 业务高峰仅清理队列，不重启服务
  
**验证方式**: 
```
清理前检查: IsPathProtected() + IsFileProtected()
清理后日志: 记录保护的文件数量
```

---

### 2. 高稳定 ✓
**原则**: 禁止自动重启、强制更新、后台弹窗，业务高峰不执行深度优化。

**实现方案**:

#### 2.1 禁止系统变更
- **ConfigurationManager.cs**:
  ```
  <AutoRestart enabled="false" reason="禁止自动重启"/>
  <ForcedUpdate enabled="false" reason="禁止强制更新"/>
  <BackgroundPopups enabled="false" reason="禁止后台弹窗"/>
  ```

#### 2.2 业务高峰保护
- **TimeSecurityLock** (时间安全锁):
  - 业务高峰: 7:00 - 22:00
  - 高峰时段禁止深度优化（btnDeepOpt.Enabled = false）
  - 高峰时段打印服务不重启（仅清理队列）

- **主窗体检测流程**:
  ```csharp
  if (TimeSecurityLock.IsBusinessPeakTime())
  {
      btnDeepOpt.Enabled = false;    // 深度优化被锁定
      AppendLog("深度优化功能已锁定（保护医疗业务）");
  }
  ```

#### 2.3 操作时间限制
- **ConfigurationManager.cs**:
  - QuickOpt (极速优化): 最多 300 秒
  - DeepOpt (深度优化): 最多 1800 秒
  - 确保不会长时间占用系统资源

**验证方式**:
- 在业务高峰时段(7:00-22:00)无法执行深度优化
- 系统启动时检查并禁用所有不稳定操作

---

### 3. 数据安全 ✓
**原则**: 不清理患者病历、影像、缴费日志等业务数据，仅清理系统垃圾。

**实现方案**:

**BusinessDataProtection.cs - 三层保护**:

```
第一层 - 路径保护 (13个关键目录)
├─ 医疗系统目录
│  ├─ D:\HIS, D:\LIS, D:\PACS
│  ├─ E:\病历数据, E:\影像存档
│  └─ C:\医疗系统
├─ 程序安装目录
│  ├─ C:\Program Files\HIS
│  ├─ C:\Program Files (x86)\PACS
│  └─ C:\ProgramData\医疗保险
└─ 数据库配置
   ├─ C:\ProgramData\HIS
   └─ C:\ProgramData\LIS

第二层 - 进程保护 (13个关键进程)
├─ HIS/LIS/PACS: hisclient, hisserver, lisclient...
├─ 医保系统: medicalinsure
├─ 医疗外设: cardreader, printerdriver, scannerdriver
├─ 数据库: sqlserver, oracle, mysql
└─ 系统服务: spool (打印服务)

第三层 - 文件类型保护 (9个关键扩展名)
├─ 医学影像: .dcm (DICOM格式)
├─ 数据库: .db, .mdb, .sqlite
├─ 业务数据: .csv, .xml, .ini, .config
└─ 业务日志: .log
```

**执行流程**:
1. 清理前: `IsPathProtected()` → 是否在保护清单
2. 清理前: `IsFileProtected()` → 文件扩展名是否保护
3. 清理前: `IsProcessProtected()` → 进程是否可终止
4. 清理后: 日志记录保护数量

**验证方式**:
- 清理日志中输出: "保护X个医疗数据文件"
- 审计日志记录每次清理的详细信息
- 医疗数据目录的文件数量保持不变

---

### 4. 外设兼容 ✓
**原则**: 优先保障打印机、身份证读卡器、扫码枪、医疗外接设备驱动正常。

**实现方案**:

**MedicalDeviceManager.cs - 6类医疗外设监控**:

```
设备类型 1: 身份证读卡器 (挂号、缴费)
├─ 驱动: usbser, ftdibus, cardreader, usb_ser_enum

设备类型 2: 打印机 (处方、报表、检查单)
├─ 驱动: usbprint, printer, lptenum, hpZebraPort

设备类型 3: 扫码枪 (检验流程、药房)
├─ 驱动: usbhid, hidusb, scanner, barcode

设备类型 4: 超声设备 (影像诊断)
├─ 驱动: simt64, ultrasound, meddevice

设备类型 5: 心电图仪 (患者监测)
├─ 驱动: ecg, cardiograph, meddevice

设备类型 6: 血氧仪 (患者监测)
├─ 驱动: oximeter, pulse, biodevice
```

**功能**:
- `CheckMedicalDeviceStatus()`: 启动时检查所有医疗设备
- `IsMedialDeviceDriver()`: 识别是否为医疗设备驱动
- 自动报警: 异常设备显示错误码并记录

**UI集成**:
- "设备检查"按钮: 医护人员和运维人员都可查看
- 启动时自动运行一次检查
- 检查结果在日志中显示设备状态

---

### 5. 权限可控 ✓
**原则**: 仅运维人员可操作，医护人员仅查看状态，无修改权限。

**实现方案**:

**PermissionManager.cs - 基于Windows域的角色管理**:

```
运维人员 (Operator) - AD域组
├─ HOSPITAL\IT-ADMIN
├─ HOSPITAL\运维组
└─ Administrators

医护人员 (Viewer) - AD域组
├─ HOSPITAL\医护人员
├─ HOSPITAL\门诊医生
└─ HOSPITAL\急诊科

未知身份 (Unknown)
└─ 系统受限模式
```

**权限矩阵**:

| 操作 | 运维人员 | 医护人员 | 未知身份 |
|------|--------|--------|--------|
| 极速优化 | ✓ | ✗ | ✗ |
| 深度优化 | ✓ | ✗ | ✗ |
| 打印修复 | ✓ | ✗ | ✗ |
| 网络修复 | ✓ | ✗ | ✗ |
| 查看配置 | ✓ | ✓ | ✗ |
| 设备检查 | ✓ | ✓ | ✗ |
| 查看审计日志 | ✓ | ✓ | ✗ |
| 修改配置 | ✓ | ✗ | ✗ |

**UI动态显示**:
```csharp
// 医护人员启动时的UI状态
btnQuickOpt.Enabled = false;      // 灰显
btnDeepOpt.Enabled = false;       // 灰显
btnRepairPrint.Enabled = false;   // 灰显
btnRepairNet.Enabled = false;     // 灰显
btnViewConfig.Enabled = true;     // 启用
btnDeviceCheck.Enabled = true;    // 启用
btnViewAuditLog.Enabled = true;   // 启用
```

---

## 审计与监控系统

### AuditLogger.cs - 完整的操作审计

**审计内容**:
```
[2024-06-07 10:30:45] | User: HOSPITAL\IT-ADMIN | Role: Operator | 
Operation: 极速优化 | Status: SUCCESS | Details: 清理了150个临时文件
```

**日志位置**: `%APPDATA%\HospitalTerminalOpt\Logs\audit_YYYY-MM-DD.log`

**保留期**: 90天（可配置）

**功能**:
- 每次操作自动记录用户、角色、操作类型、执行结果
- 操作失败时记录失败原因
- 权限拒绝时记录拒绝原因
- 查看审计日志按钮可显示最近7天的日志

---

## ConfigurationManager.cs - 集中配置管理

**配置文件位置**: `%APPDATA%\HospitalTerminalOpt\config.xml`

**核心配置项**:
```xml
<BusinessPeakHours startHour="7" endHour="22" disableDeepOpt="true"/>
<AutoRestart enabled="false"/>
<ForcedUpdate enabled="false"/>
<BackgroundPopups enabled="false"/>
<OptimizationPolicy>
  <QuickOpt maxDurationSeconds="300" allowDuringPeakHours="true"/>
  <DeepOpt maxDurationSeconds="1800" allowDuringPeakHours="false"/>
</OptimizationPolicy>
<DataProtection 
  allowSystemCleanup="true"
  protectPatientData="true"
  protectMedicalImages="true"/>
```

---

## 启动流程图

```
应用启动
    ↓
[1] 识别用户身份 (GetCurrentUserRole)
    ├─ 查询Windows域组
    ├─ 匹配运维人员/医护人员/未知
    └─ 设置权限级别
    ↓
[2] 加载系统配置 (ConfigurationManager)
    ├─ 检查禁止项状态 (自动重启/强制更新/弹窗)
    ├─ 读取业务高峰时段
    └─ 读取操作时间限制
    ↓
[3] 检查业务状态 (TimeSecurityLock)
    ├─ 获取当前时间
    ├─ 判断是否在高峰时段
    └─ 禁用/启用深度优化按钮
    ↓
[4] 检查医疗外设 (MedicalDeviceManager)
    ├─ 枚举所有设备驱动
    ├─ 识别医疗设备状态
    └─ 报警异常设备
    ↓
[5] 显示UI界面
    ├─ 根据用户角色显示/隐藏按钮
    ├─ 显示当前权限和状态
    └─ 等待用户操作
    ↓
[6] 执行操作时审计
    ├─ 检查权限 (CanExecuteOperation)
    ├─ 记录审计日志 (AuditLogger)
    └─ 执行操作 + 三层保护检查
```

---

## 关键文件说明

| 文件 | 大小 | 功能 | 核心原则 |
|------|------|------|--------|
| `PermissionManager.cs` | 77行 | 权限管理 + Windows域认证 | 权限可控 |
| `AuditLogger.cs` | 67行 | 审计日志系统 | 权限可控 + 数据安全 |
| `BusinessDataProtection.cs` | 119行 | 三层保护机制 | 零侵入 + 数据安全 |
| `MedicalDeviceManager.cs` | 159行 | 医疗外设监控 | 外设兼容 |
| `ConfigurationManager.cs` | 170行 | 集中配置管理 | 高稳定 |
| `CommonHelper.cs` | 165行 | 清理/修复工具（更新） | 零侵入 + 高稳定 |
| `MainForm.cs` | 180行 | 主UI窗体（更新） | 权限可控 |
| `MainForm.Designer.cs` | 140行 | UI控件设计（更新） | 权限可控 |

---

## 测试场景

### 场景1: 运维人员在业务低峰期
✓ 所有功能可用
✓ 可执行深度优化
✓ 医疗数据受保护
✓ 所有操作被审计

### 场景2: 医护人员在业务高峰期
✓ 所有修改操作禁用
✓ 可查看配置和设备状态
✓ 可查看审计日志
✓ 无误操作风险

### 场景3: 业务高峰期的运维人员
✓ 仍可执行极速优化
✗ 深度优化按钮被禁用
✓ 打印服务不重启（仅清理队列）

### 场景4: 医疗设备异常
✓ 启动时自动检测并报警
✓ 异常设备显示错误码
✓ 系统继续正常运行（不中断）

---

## 部署检查清单

- [ ] 安装到医院运维管理服务器
- [ ] 配置Windows域组（管理员创建）
  - `HOSPITAL\IT-ADMIN`
  - `HOSPITAL\医护人员`
- [ ] 生成初始配置文件 (`config.xml`)
- [ ] 创建审计日志目录权限
- [ ] 运维人员验证功能权限
- [ ] 医护人员验证查看权限
- [ ] 医疗设备连接测试
- [ ] 业务高峰时段禁用测试
- [ ] 审计日志正常记录验证

---

## 遵循原则总结

| 原则 | 实现类 | 验证方式 |
|------|--------|--------|
| 零侵入 | `BusinessDataProtection` | 医疗数据目录文件数不变 |
| 高稳定 | `ConfigurationManager` + `TimeSecurityLock` | 高峰时段无深度优化 |
| 数据安全 | `BusinessDataProtection` | 三层保护清单+审计日志 |
| 外设兼容 | `MedicalDeviceManager` | 设备状态报告+驱动保护 |
| 权限可控 | `PermissionManager` + `AuditLogger` | 基于Windows域+操作审计 |

