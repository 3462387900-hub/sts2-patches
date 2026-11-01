# 遗物预览兼容修复 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

Phrolova 提示补丁读取无玩家归属的标准模型 Owner；增加标准预览保护，保留实物遗物提示。

## 出现什么问题

- 海克斯组合遗物预览触发 Owner 异常。

## 什么时候可能遇到

- Phrolova 与海克斯遗物预览共同使用标准 ArchaicTooth 模型。

## 涉及哪些模组

问题类型：模组组合冲突

- 受影响模组：弗洛洛（Phrolova）——Phrolova 与海克斯遗物预览共同使用标准 ArchaicTooth 模型。
- 触发条件涉及的模组：ARAM: Mayhem / 海克斯大乱斗（HextechRunes）——Phrolova 与海克斯遗物预览共同使用标准 ArchaicTooth 模型。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；BaseLib v3.4.7、HextechRunes 0.9.8、Phrolova v1.1.0

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- Phrolova 提示补丁读取无玩家归属的标准模型 Owner；增加标准预览保护，保留实物遗物提示。

玩法影响：仅修正提示显示，保留有归属遗物的原有升级提示。

涉及文件：

- CanonicalRelicHoverFix/CanonicalRelicHoverFix.dll：既有已核对修复文件；本次保持字节不变。
- CanonicalRelicHoverFix/CanonicalRelicHoverFix.json：既有已核对修复文件；本次保持字节不变。
- CanonicalRelicHoverFix/runtime-baseline.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

无需保持相同安装状态。显示层修复无需同步战斗规则；需要修复本地显示问题的玩家各自安装。当前整合包仍建议队友统一版本。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。Phrolova 提示补丁读取无玩家归属的标准模型 Owner；增加标准预览保护，保留实物遗物提示。

- 既有补丁清单和整合包说明已核对。
- 随补丁保存的 patch-info/historical-validation.txt。

- 代码审查：部分验证。核对现有清单、整合说明及可用的历史证据，未重新审查所有源码。（2026-11-02）
- 自动化检查：通过。历史 10 项行为检查通过，使用真实 Phrolova 补丁与 ArchaicTooth getter，界面和初始化使用测试替身。
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

- HextechRunes/lib/0.111.0/HextechRunes.dll：`8C203D53C13A7D9CA89F5B9CA4498039D2CFB152E4685727497351FB60F11A2B`

- Phrolova/Phrolova.dll：`E2BABDADB9C424ABF47AAD68E03667FDEDAAF1CD032760337C1808ADB27C3F4D`

修复后文件特征：

- CanonicalRelicHoverFix/CanonicalRelicHoverFix.dll：`6E8D37F0D93070AEEAEE4684AE3EF6BF2C695F2D3C92CA49EDDF5F6314A7A763`

- CanonicalRelicHoverFix/CanonicalRelicHoverFix.json：`5462AE9932B5C7620F17EA6CD2EEA5B093E9793122A659B20F72BBD7152AA36A`

- CanonicalRelicHoverFix/runtime-baseline.json：`44D23CE6337B6DAAF98984BFB679AD9F6B1710B1F481002076C867AFF6987A37`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
