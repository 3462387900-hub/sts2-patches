# 联机兼容修复包 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

本次仅更新第四幕文件校验值；保留既有帮助药水、事件、终曲合声、秘密节点、换幕和能天使营火修复。既有多个子问题不能概括为同一 MOD 冲突。

## 出现什么问题

- 第四幕修复后主菜单出现版本不符红字，联机兼容修复未加载。

## 什么时候可能遇到

- Act4FinalAscent DLL 已修复，但旧 runtime-baseline 仍记载原哈希。

## 涉及哪些模组

问题类型：补丁间兼容

- 受影响模组：联机兼容修复包（SpireTogetherFix）——Act4FinalAscent DLL 已修复，但旧 runtime-baseline 仍记载原哈希。
- 需联动处理的组件：第四幕：终焉攀升（Act4FinalAscent）——需联动核对的组件，不表示导致原始问题。
- 运行前置：BaseLib（BaseLib）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：RitsuLib（STS2-RitsuLib）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：集成战略事件（IntegratedStrategyEvents）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：ARAM: Mayhem / 海克斯大乱斗（HextechRunes）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：海克斯大乱斗：额外拓展包（HextechRunesSponsorPack）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：GoldEnchantEvent 免费附魔事件（GoldEnchantEvent）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：无尽模式（EndlessMod）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：Neuvillette（Neuvillette）——清单声明的运行前置；不据此认定存在冲突。
- 运行前置：能天使（AK_Exusiai）——清单声明的运行前置；不据此认定存在冲突。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；AK_Exusiai 1.1.0、Act4FinalAscent 0.1.3、BaseLib v3.4.7、EndlessMod v1.24.1、GoldEnchantEvent v1.5.0、HextechRunes 0.9.8、HextechRunesSponsorPack 0.9.2、IntegratedStrategyEvents 0.5.6、Neuvillette 0.25.0-beta、STS2-RitsuLib 0.6.7

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 本次仅更新第四幕文件校验值；保留既有帮助药水、事件、终曲合声、秘密节点、换幕和能天使营火修复。既有多个子问题不能概括为同一 MOD 冲突。

玩法影响：本次校验值更新不改玩法；既有 DLL 包含事件、血量、奖励、站位和联机逻辑修复。

涉及文件：

- SpireTogetherFix/runtime-baseline.json：既有已核对修复文件；本次保持字节不变。
- SpireTogetherFix/SpireTogetherFix.dll：既有已核对修复文件；本次保持字节不变。
- SpireTogetherFix/SpireTogetherFix.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。本次仅更新第四幕文件校验值；保留既有帮助药水、事件、终曲合声、秘密节点、换幕和能天使营火修复。既有多个子问题不能概括为同一 MOD 冲突。

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

- AK_Exusiai/AK_Exusiai.dll：`AFFEB5A53BBF3E85F3CD6D8804CA644F14F6C431FB761A545D4933F27842F75E`

- AK_Exusiai/AK_Exusiai.pck：`1817D66C9EC3CDDE1F1E949F27AC663B686E37BEA42CA4A704682024C39FE82C`

- Act4FinalAscent/Act4FinalAscent.dll：`1100CE4C1BDC5140373EFC4AA39C9F61941BC39B0362ED37B0FE3BEA595F96DA`

- BaseLib/BaseLib.dll：`55863CA6ADC30A61A7544874E157A519FFE148CCF2B18025947609C4BFF813CE`

- EndlessMod/EndlessMod.dll：`6295B258D8E981B73774C7247395ECE525C03AA86E995CF146FE69E35F792F3D`

- GoldEnchantEvent/GoldEnchantEvent.dll：`D875EC7F99ACD01E39C20B1D4EB2891189D3F3062BF4341BB81AC0DC74FB71F7`

- HextechRunes/lib/0.111.0/HextechRunes.dll：`8C203D53C13A7D9CA89F5B9CA4498039D2CFB152E4685727497351FB60F11A2B`

- HextechRunesSponsorPack/lib/0.111.0/HextechRunesSponsorPack.dll：`68AE906B8B804B17855C3BB1476E115A70C4DAEAFBCF509E45556C9B8905B7A0`

- IntegratedStrategyEvents/lib/0.111.0/IntegratedStrategyEvents.dll：`0CADE386CB595311216DD7D69784B6901EDDF61AF2A127FE34127F49052ACAFF`

- Neuvillette/Neuvillette.dll：`0E0957685AB4035B4A473B4F922EB79C2F98CB8DDC67B7B30C72033657873C02`

- STS2-RitsuLib/compat/0.111.0/STS2-RitsuLib.Runtime.dll：`7A77583AF80792213C8DCD7572040A342E7EA0DBED7BBBC8ED8E20776445DCCF`

- STS2-RitsuLib/shared/STS2-RitsuLib.Shared.dll：`6FEF35E9BD8F39852CC81E6A800128AF572DF312356B565815910B7A0BEF13D7`

修复后文件特征：

- SpireTogetherFix/runtime-baseline.json：`4DB2DB714F5ED43C73F2D98DABF0483AAB0CD0461AA03854B5634CC1B2634CA6`

- SpireTogetherFix/SpireTogetherFix.dll：`27FCB7E8E35A7D86FAEC28280AFBFDFF93F1AA182EE2B760B9CA4D9ED1824698`

- SpireTogetherFix/SpireTogetherFix.json：`299979951081F08464415753BBA72E8CA1D4C970688F8BC44299025767EC1F3D`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
