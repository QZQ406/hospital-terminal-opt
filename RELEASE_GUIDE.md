# 医院终端优化工具 - 发布和打包指南

## 发布清单

### 编译和打包前检查

- [ ] 所有代码已通过编译
- [ ] 无linter错误
- [ ] 功能测试通过
- [ ] 所有文档已更新
- [ ] 版本号已更新
- [ ] 审计日志清空（生产版本）

### 编译步骤

```bash
# 清理之前的编译
dotnet clean HospitalTerminalOpt.csproj

# Release模式编译（优化尺寸和性能）
dotnet build -c Release HospitalTerminalOpt.csproj

# 输出文件位置
# bin\Release\net462\HospitalTerminalOpt.exe
```

### 文件大小验证

编译完成后检查exe文件大小：

```bash
# Windows命令行
dir bin\Release\net462\HospitalTerminalOpt.exe

# 预期：< 10MB
```

---

## 发布包结构

### 单文件包（基础版本）

```
HospitalTerminalOpt-v1.0.zip
├─ HospitalTerminalOpt.exe          (主程序，< 10MB)
├─ README.md                         (快速开始指南)
├─ DEPLOYMENT_GUIDE.md               (详细部署指南)
├─ DESIGN.md                         (架构设计文档)
├─ TEST_GUIDE.md                     (测试指南)
└─ LICENSE.txt                       (许可证）
```

### U盘便携版本

```
医院终端优化工具-U盘版-v1.0.zip
│
└─ U盘使用说明.txt                  (关键步骤提示)
   │
   └─ [将以下内容复制到U盘根目录]
      ├─ HospitalTerminalOpt.exe
      ├─ config\                     (空目录)
      ├─ logs\                       (空目录)
      ├─ 快速开始.txt
      └─ 常见问题.txt
```

### 内网共享版本

```
医院终端优化工具-内网共享版-v1.0.zip
│
└─ 部署说明.txt
   │
   └─ [将以下内容复制到服务器共享目录]
      ├─ HospitalTerminalOpt.exe
      ├─ config\                     (所有终端共享)
      ├─ logs\                       (所有终端共享)
      └─ 权限配置说明.txt
```

---

## 版本信息管理

### 版本号规范

使用语义化版本 (Semantic Versioning)：`MAJOR.MINOR.PATCH`

- **MAJOR**：不兼容的API变更（如功能移除）
- **MINOR**：向后兼容的功能新增
- **PATCH**：向后兼容的bug修复

**当前版本**：1.0.0

### 版本历史模板

```
v1.0.0 (2024-06-07)
- 初始发布
- 核心功能完善
- 文档完整
- 单文件部署就绪

v1.1.0 (计划)
- 支持多语言界面
- 增加更多医疗设备类型
- 性能优化

v2.0.0 (计划)
- Web版本支持
- 云端配置同步
- 高级分析功能
```

---

## 构建脚本

### PowerShell发布脚本

```powershell
# release.ps1 - 自动化发布流程

param(
    [string]$Version = "1.0.0",
    [string]$OutputDir = ".\releases"
)

# 检查编译
Write-Host "清理并编译项目..." -ForegroundColor Cyan
dotnet clean -c Release
dotnet build -c Release

# 检查文件大小
$exePath = ".\bin\Release\net462\HospitalTerminalOpt.exe"
$fileSize = (Get-Item $exePath).Length / 1MB
Write-Host "Exe文件大小: $($fileSize.ToString('F2')) MB" -ForegroundColor Yellow

if ($fileSize -gt 10) {
    Write-Error "文件大小超过10MB限制！"
    exit 1
}

# 创建发布目录
if (!(Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir
}

# 复制文件
$releaseDir = "$OutputDir\HospitalTerminalOpt-v$Version"
if (Test-Path $releaseDir) {
    Remove-Item -Recurse $releaseDir
}
New-Item -ItemType Directory -Path $releaseDir

Copy-Item $exePath -Destination $releaseDir
Copy-Item "README.md", "DEPLOYMENT_GUIDE.md", "DESIGN.md", "TEST_GUIDE.md" -Destination $releaseDir

# 创建压缩包
$zipPath = "$OutputDir\HospitalTerminalOpt-v$Version.zip"
Compress-Archive -Path $releaseDir -DestinationPath $zipPath -Force

Write-Host "发布完成: $zipPath" -ForegroundColor Green
Write-Host "文件大小: $((Get-Item $zipPath).Length / 1MB)MB"
```

### Bash发布脚本

```bash
#!/bin/bash
# release.sh - Linux/Mac发布脚本

VERSION=${1:-"1.0.0"}
OUTPUT_DIR="./releases"

echo "清理并编译项目..."
dotnet clean -c Release
dotnet build -c Release

EXE_PATH="./bin/Release/net462/HospitalTerminalOpt.exe"
FILE_SIZE=$(stat -f%z "$EXE_PATH" 2>/dev/null || stat -c%s "$EXE_PATH")
FILE_SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc)

echo "Exe文件大小: $FILE_SIZE_MB MB"

if (( $(echo "$FILE_SIZE_MB > 10" | bc -l) )); then
    echo "错误：文件大小超过10MB限制！"
    exit 1
fi

mkdir -p "$OUTPUT_DIR"
RELEASE_DIR="$OUTPUT_DIR/HospitalTerminalOpt-v$VERSION"

rm -rf "$RELEASE_DIR"
mkdir -p "$RELEASE_DIR"

cp "$EXE_PATH" "$RELEASE_DIR"
cp README.md DEPLOYMENT_GUIDE.md DESIGN.md TEST_GUIDE.md "$RELEASE_DIR"

cd "$OUTPUT_DIR"
zip -r "HospitalTerminalOpt-v$VERSION.zip" "HospitalTerminalOpt-v$VERSION"
cd ..

echo "发布完成: $OUTPUT_DIR/HospitalTerminalOpt-v$VERSION.zip"
```

---

## 发布渠道

### 1. 医院内部发布

**位置**：医院内网文件服务器

```
\\医院服务器\软件发布\HospitalTerminalOpt\
├─ v1.0.0\
│  ├─ HospitalTerminalOpt.exe
│  ├─ 文档\
│  └─ 部署说明.txt
└─ 历史版本\
```

**发布流程**：
1. 上传编译后的exe和文档
2. 更新版本记录
3. 发送公告给运维人员
4. 定期清理过期版本

### 2. U盘便携版本

**制作方式**：
1. 准备多个8GB或更大U盘
2. 格式化为NTFS
3. 复制HospitalTerminalOpt.exe和文档
4. 创建config、logs空目录
5. 标记版本号和日期

**交付方式**：
- 运维人员现场交付
- 快递发送到各科室
- 外出维护时携带

### 3. 远程部署

**使用组策略（AD）**：

```powershell
# 在域控制器上配置
# 组策略编辑器 > 计算机配置 > 软件设置 > 软件安装
# 配置package: HospitalTerminalOpt-v1.0.0.zip
# 分配给特定组织单位（OU）
```

**使用System Center Configuration Manager (SCCM)**：
- 创建应用程序
- 设置部署类型
- 指定目标集合
- 配置部署计划

---

## 升级和回滚

### 升级流程

**版本检查**：
```bash
# 查看当前版本信息
HospitalTerminalOpt.exe -version

# 或检查exe属性 > 详情 > 文件版本
```

**升级步骤**：
1. 备份当前版本：`copy HospitalTerminalOpt.exe HospitalTerminalOpt.exe.bak`
2. 备份配置：`xcopy config config.bak /E`
3. 替换exe文件：`copy 新版本\HospitalTerminalOpt.exe .`
4. 测试新版本
5. 更新审计日志记录

**升级验证**：
```bash
# 验证新版本
HospitalTerminalOpt.exe

# 检查功能
HospitalTerminalOpt.exe -silent -check-device

# 验证审计日志
type logs\audit_*.log | findstr "版本"
```

### 回滚流程

```bash
# 恢复备份的exe
copy HospitalTerminalOpt.exe.bak HospitalTerminalOpt.exe

# 恢复配置（如需要）
rmdir /s /q config
xcopy config.bak config /E

# 验证回滚
HospitalTerminalOpt.exe
```

---

## 质量保证

### 发布前测试清单

**功能测试**：
- [ ] GUI模式启动成功
- [ ] 所有按钮功能正常
- [ ] 权限管理正确
- [ ] 业务高峰保护生效
- [ ] 医疗数据保护有效
- [ ] 医疗设备识别正常

**静默模式测试**：
- [ ] -silent -optimize quick 成功
- [ ] -silent -optimize deep 成功
- [ ] -silent -check-device 成功
- [ ] -silent -repair-printer 成功
- [ ] -silent -repair-network 成功

**兼容性测试**：
- [ ] Windows 7 SP1 测试
- [ ] Windows 10 测试
- [ ] Windows 11 测试
- [ ] .NET Framework 4.6.2 验证

**性能测试**：
- [ ] 启动时间 < 1秒
- [ ] GUI模式内存 < 100MB
- [ ] 静默模式内存 < 50MB
- [ ] 文件大小 < 10MB

**安全测试**：
- [ ] 审计日志完整记录
- [ ] 权限拒绝正常处理
- [ ] 医疗数据完全保护
- [ ] 配置文件权限正确

### 发布后监控

**第一周**：
- 每日检查医院IT部门反馈
- 监控崩溃和错误报告
- 收集用户意见
- 准备热修复方案

**持续监控**：
- 定期审计日志分析
- 性能指标收集
- 使用统计汇总
- 问题趋势分析

---

## 文档发布

### 文档版本控制

```
docs/
├─ v1.0.0/
│  ├─ README.md
│  ├─ DEPLOYMENT_GUIDE.md
│  ├─ DESIGN.md
│  ├─ TEST_GUIDE.md
│  └─ RELEASE_NOTES.md
└─ latest/ -> v1.0.0/
```

### 发布说明（Release Notes）模板

```markdown
# 版本 1.0.0 发布说明

## 发布日期
2024年6月7日

## 主要功能
- 轻量级单文件exe（<10MB）
- 前台GUI + 后台静默双模式
- 完整的权限管理和审计
- 医疗数据三层保护
- 支持U盘、内网共享、本地部署

## 核心特性
✓ 零侵入系统优化
✓ 业务高峰自动保护
✓ 医疗设备驱动保护
✓ 完整操作审计追踪
✓ 支持Windows 7-11

## 已知限制
- 需要.NET Framework 4.6.2+
- WMI需要启用（设备检查功能）
- 某些操作需要管理员权限

## 安装说明
详见 DEPLOYMENT_GUIDE.md

## 测试验证
详见 TEST_GUIDE.md

## 反馈和支持
联系IT运维部门
```

---

## 许可证和合规

### 许可证信息

```
医院终端系统优化工具
Copyright (c) 2024 [医院名称]
All Rights Reserved

本软件为医院内部专属工具。
未经授权，禁止商业使用、复制或传播。
```

### 合规检查

- [ ] 所有医疗相关数据已验证保护
- [ ] 审计日志满足合规要求
- [ ] 隐私保护措施已落实
- [ ] 安全审查已完成
- [ ] 医院审批已取得

---

## 故障恢复

### 紧急修复流程

**发现严重bug**：
1. 立即通知IT运维团队
2. 准备hotfix版本
3. 发布紧急补丁（v1.0.1）
4. 更新所有已部署实例
5. 验证修复有效性

**紧急回滚**：
```bash
# 快速恢复到上个版本
copy HospitalTerminalOpt.exe.prev HospitalTerminalOpt.exe
# 重启所有受影响的终端
```

---

## 发布记录模板

```
发布日期: 2024-06-07
版本: v1.0.0
发布者: IT运维部门
审核人: 信息部主任

发布内容:
- HospitalTerminalOpt.exe (5.3MB)
- 完整文档
- 测试验证通过

部署范围:
- 医院内网共享服务器
- U盘便携版发放
- 运维工作站预装

反馈收集:
- 自 2024-06-07 开始
- 持续监控至 2024-06-14
- 收集运维和医护人员反馈

已知问题:
- 无

下一步计划:
- 定期维护和监控
- 收集用户反馈改进
- 计划v1.1发布（2024-09月）
```

---

## 检查清单 - 发布前最终检查

发布前必须完成以下所有项目：

**代码质量**
- [ ] 无编译错误
- [ ] 无linter警告
- [ ] 代码审查通过
- [ ] 所有测试通过

**性能**
- [ ] 启动时间 < 1秒
- [ ] 内存占用正常
- [ ] 文件大小 < 10MB
- [ ] 无内存泄漏

**安全性**
- [ ] 权限管理测试通过
- [ ] 审计日志完整
- [ ] 数据保护验证
- [ ] 安全审查通过

**文档**
- [ ] README.md 完善
- [ ] DEPLOYMENT_GUIDE.md 完善
- [ ] DESIGN.md 完善
- [ ] TEST_GUIDE.md 完善
- [ ] RELEASE_NOTES.md 编写完成
- [ ] LICENSE.txt 准备完成

**兼容性**
- [ ] Windows 7 SP1 测试通过
- [ ] Windows 10 测试通过
- [ ] Windows 11 测试通过
- [ ] .NET 4.6.2+ 验证

**部署包**
- [ ] 单文件包准备完成
- [ ] U盘版本准备完成
- [ ] 内网共享版准备完成
- [ ] 版本号统一

**最终检查**
- [ ] 医院审批确认
- [ ] IT部门确认
- [ ] 没有遗漏任何待办事项
- [ ] 准备好发布公告
