# 第四幕第三幕快照存取修复

版本：1.0.0　|　已发布

第三幕历史快照写入或重载时报 JSON 类型元数据缺失。

## 出现什么问题

- 第三幕历史快照写入或重载时报 JSON 类型元数据缺失。

## 什么时候可能遇到

- 第四幕调用 SaveManager 序列化私有 Act3SnapshotStore。

## 涉及哪些模组

问题类型：游戏接口兼容

- 受影响模组：Act4FinalAscent（Act4FinalAscent）——第四幕调用 SaveManager 序列化私有 Act3SnapshotStore。

## 适用版本

- 配置 known-0.111.0-build：游戏 0.111.0；Act4FinalAscent 0.1.3

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 游戏源生成 resolver 没有 Mod 私有存储类型；仅替换这两处读写调用，在复制的序列化选项中组合反射 fallback，保留原转换器和 RunHistory 元数据。
- 运行时按内嵌基线校验依赖；应用失败撤销本补丁已注册的 hooks。

玩法影响：只改变第四幕私有快照 JSON 编解码；原文件路径和读写流程保持，拒绝损坏数据及未知 schema。

涉及文件：

- Act3SnapshotSaveFix/Act3SnapshotSaveFix.dll：独立修复程序集，含兼容基线。
- Act3SnapshotSaveFix/Act3SnapshotSaveFix.json：游戏 MOD 加载清单。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

- 必须安装：local.act4.ambient-timer-fix，版本至少 1.0.0。

## 依据与验证

诊断结论：已确认。游戏源生成 resolver 没有 Mod 私有存储类型；仅替换这两处读写调用，在复制的序列化选项中组合反射 fallback，保留原转换器和 RunHistory 元数据。

- 实际私有存储类型通过原 SaveManager 时重现 NotSupportedException。
- 私有类型和嵌套 RunHistory 中文数据往返、坏 JSON/schema/null 拒绝、全局 resolver 不变通过。

- 代码审查：通过。对照实际 DLL、日志及调用链；现场复现边界见限制。（2026-11-02）
- 自动化检查：通过。本轮 8 项隔离行为检查通过，另有 14 项运行时守卫及六项共同初始化检查。引擎/UI/网络边界使用夹具。（2026-11-02）
- 本机游戏测试：未执行。未启动真实游戏，未执行真人联机。
- 完整联机测试：未执行。未启动真实游戏，未执行真人联机。

## 限制与未解决事项

- 仅适配列明的 0.111.0 文件构建；未知版本阻止安装。
- 未进行真人联机/完整对局回归；文件安装成功不代表原现场已解决。
- 不自动重建此前写入失败而缺失的快照；针对已有建筑家修复的第四幕 DLL。

## 安装与撤销

在管理器中选择自己的游戏目录，查看改动计划。文件与依赖核对通过后，再通过已审核的安装清单安装。

支持回滚。在支持本批补丁的管理器中检查最近一次回滚，再确认撤销该安装批次；未知后续文件或损坏备份会停止回滚。不更改存档。

## 更新记录

- 1.0.0 · 2026-11-02：游戏源生成 resolver 没有 Mod 私有存储类型；仅替换这两处读写调用，在复制的序列化选项中组合反射 fallback，保留原转换器和 RunHistory 元数据。；使用管理器选择安装，不提供独立 CMD 修复包。

<details>
<summary>技术详情：文件与校验信息</summary>


匹配配置：known-0.111.0-build

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- Act4FinalAscent/Act4FinalAscent.dll：`1100CE4C1BDC5140373EFC4AA39C9F61941BC39B0362ED37B0FE3BEA595F96DA`

修复后文件特征：

- Act3SnapshotSaveFix/Act3SnapshotSaveFix.dll：`15CCBC5400C799E106448486B6C0151FF578737252EC6486943BC9222C839F65`

- Act3SnapshotSaveFix/Act3SnapshotSaveFix.json：`D794C3F149AD8153EC8E958AED3CB2CB57E7F3518C022C0ED004AB5143130227`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
