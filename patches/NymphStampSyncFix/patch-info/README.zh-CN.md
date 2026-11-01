# 妮芙印花同步修复 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

查询避免不一致副作用，战斗副本按照关联牌恢复印花，保留正常染色玩法。

## 出现什么问题

- 妮芙印花查询、恢复结果在联机双方不一致。

## 什么时候可能遇到

- 印花查询产生写入，或战斗牌堆副本恢复印花。

## 涉及哪些模组

问题类型：单个模组问题

- 受影响模组：CyanrainNymph Beta（CyanrainNymphBeta）——印花查询产生写入，或战斗牌堆副本恢复印花。
- 运行前置：BaseLib（BaseLib）——清单声明的运行前置；不据此认定存在冲突。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；BaseLib v3.4.7、CyanrainNymphBeta v0.0.0-beta+exhaustfix.20261029

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 查询避免不一致副作用，战斗副本按照关联牌恢复印花，保留正常染色玩法。

玩法影响：调整印花状态同步和恢复行为，保留染色规则。

涉及文件：

- NymphStampSyncFix/NymphStampSyncFix.dll：既有已核对修复文件；本次保持字节不变。
- NymphStampSyncFix/NymphStampSyncFix.json：既有已核对修复文件；本次保持字节不变。
- NymphStampSyncFix/runtime-baseline.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

- 必须安装：local.cyanrainnymphbeta，版本至少 1.0.0。

## 依据与验证

诊断结论：部分确认。查询避免不一致副作用，战斗副本按照关联牌恢复印花，保留正常染色玩法。

- 既有补丁清单和整合包说明已核对。
- 随补丁保存的 patch-info/historical-validation.txt。

- 代码审查：部分验证。核对现有清单、整合说明及可用的历史证据，未重新审查所有源码。（2026-11-02）
- 自动化检查：通过。历史 41 项真实游戏/妮芙类型行为检查通过，未启动游戏。
- 本机游戏测试：未执行。本轮未重新执行该类测试。
- 完整联机测试：未执行。本轮未重新执行该类测试。

## 限制与未解决事项

- 本轮补齐随包信息，未进行完整真人联机回归。
- 适用范围按当前已核对构建保守限定，不表示所有前置都是冲突方。
- 本条元数据暂为 draft：已随内部包交付，但管理器独立下载安装协议尚未接入。安装使用整合包 Install.cmd。

## 安装与撤销

此条目尚未开放管理器独立安装。当前可使用整合包的 Install.cmd，选择包含 SlayTheSpire2.exe 的游戏目录；Check.cmd 只检查，Rollback.cmd 撤销整合包安装。

支持回滚。使用整合包 Rollback.cmd 自选游戏目录，恢复安装前状态并移除新增说明；文件或备份被后续修改时停止。当前不提供单项独立回滚。

## 更新记录

- 1.0.0 · 2026-11-02：新增随补丁保存的结构化说明及玩家文档；二进制沿用当前整合包。

<details>
<summary>技术详情：文件与校验信息</summary>


匹配配置：known-build-1

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- BaseLib/BaseLib.dll：`55863CA6ADC30A61A7544874E157A519FFE148CCF2B18025947609C4BFF813CE`

- CyanrainNymphBeta/CyanrainNymphBeta.dll：`D23EFDD37AAE216CCF9D780EF97D3EAD2CDF8F5A2CF4031E0C6823AE707C5ED5`

修复后文件特征：

- NymphStampSyncFix/NymphStampSyncFix.dll：`77380AF43B47B533D826DBB4E0843E5FBD4E1E613FCDEEF75C73B65A1A08D62C`

- NymphStampSyncFix/NymphStampSyncFix.json：`33145C6F265DB1BF43E58A2593413211A9E0EB81D67DF6E576093232F96D075A`

- NymphStampSyncFix/runtime-baseline.json：`39FA8A5BB23A0E6BD3B2D9BF9ED5DC92C71E53AE3479976072E8C389316A3BBC`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
