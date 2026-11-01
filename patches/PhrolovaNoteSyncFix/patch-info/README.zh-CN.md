# 弗洛洛音符同步修复 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

隔离顺序状态并修正满音符替换扣除，保留正常替换顺序和彩乐优先保护。

## 出现什么问题

- 定音顺序共享或残留，满音符替换时可能漏扣。

## 什么时候可能遇到

- 多个实例或重复处理满音符替换。

## 涉及哪些模组

问题类型：单个模组问题

- 受影响模组：弗洛洛（Phrolova）——多个实例或重复处理满音符替换。
- 运行前置：BaseLib（BaseLib）——清单声明的运行前置；不据此认定存在冲突。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；BaseLib v3.4.7、Phrolova v1.1.0

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 隔离顺序状态并修正满音符替换扣除，保留正常替换顺序和彩乐优先保护。

玩法影响：修正音符结算状态，属于玩法逻辑修复。

涉及文件：

- PhrolovaNoteSyncFix/PhrolovaNoteSyncFix.dll：既有已核对修复文件；本次保持字节不变。
- PhrolovaNoteSyncFix/PhrolovaNoteSyncFix.json：既有已核对修复文件；本次保持字节不变。
- PhrolovaNoteSyncFix/runtime-baseline.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。隔离顺序状态并修正满音符替换扣除，保留正常替换顺序和彩乐优先保护。

- 既有补丁清单和整合包说明已核对。

- 代码审查：部分验证。核对现有清单、整合说明及可用的历史证据，未重新审查所有源码。（2026-11-02）
- 自动化检查：未执行。本轮未重新执行该类测试。
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

- Phrolova/Phrolova.dll：`E2BABDADB9C424ABF47AAD68E03667FDEDAAF1CD032760337C1808ADB27C3F4D`

修复后文件特征：

- PhrolovaNoteSyncFix/PhrolovaNoteSyncFix.dll：`FD22C9811781282D63493B66CE6D279DF35A8E446858AC32F07EC3AD2A102E42`

- PhrolovaNoteSyncFix/PhrolovaNoteSyncFix.json：`CB820FAA7A3D5CFDE2361322424AED8CE07C94D2DA03658792915F626DB7CE24`

- PhrolovaNoteSyncFix/runtime-baseline.json：`44D23CE6337B6DAAF98984BFB679AD9F6B1710B1F481002076C867AFF6987A37`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
