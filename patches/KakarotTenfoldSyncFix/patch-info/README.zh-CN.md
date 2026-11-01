# 十倍龟派气功联机同步修复 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

使用出牌前手牌快照计算邻接条件，避免读取不同步的 UI 缓存。

## 出现什么问题

- 十倍龟派气功追加攻击次数在主客机间不同。

## 什么时候可能遇到

- 出牌时双方 UI 邻接手牌缓存刷新时机不同。

## 涉及哪些模组

问题类型：单个模组问题

- 受影响模组：Kakarot（Kakarot）——出牌时双方 UI 邻接手牌缓存刷新时机不同。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；Kakarot 6.1.1

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 使用出牌前手牌快照计算邻接条件，避免读取不同步的 UI 缓存。

玩法影响：统一追加攻击次数的计算来源，保留原有邻接规则。

涉及文件：

- KakarotTenfoldSyncFix/KakarotTenfoldSyncFix.dll：既有已核对修复文件；本次保持字节不变。
- KakarotTenfoldSyncFix/KakarotTenfoldSyncFix.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。使用出牌前手牌快照计算邻接条件，避免读取不同步的 UI 缓存。

- 既有补丁清单和整合包说明已核对。
- 随补丁保存的 patch-info/historical-validation.txt。

- 代码审查：部分验证。核对现有清单、整合说明及可用的历史证据，未重新审查所有源码。（2026-11-02）
- 自动化检查：通过。历史 37 项断言通过，含不同 UI 缓存下主客机差异复现和修复验证。
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

- Kakarot/Kakarot.dll：`839C486E78516A5D5A32995880848778F2CB0935E557CAE0CF3071CAA7DA4E40`

修复后文件特征：

- KakarotTenfoldSyncFix/KakarotTenfoldSyncFix.dll：`4D276DDF94AAEDA1F9B62FB758A3C42AF863723F3B356CC0C3BCADB354E75DD2`

- KakarotTenfoldSyncFix/KakarotTenfoldSyncFix.json：`E03D9F1E1D3AAEB7640BBC91B66203EDEE3CFB0B415CBC1E10CA01DF2CCD4404`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
