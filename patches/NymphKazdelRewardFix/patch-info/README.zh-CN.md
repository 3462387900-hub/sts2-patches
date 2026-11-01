# 妮芙卡兹戴尔奖励克隆修复

版本：1.0.0　|　已发布

皇家争执等奖励抽到 GougeMemory 时触发 AssertCanonical。

## 出现什么问题

- 皇家争执等奖励抽到 GougeMemory 时触发 AssertCanonical。

## 什么时候可能遇到

- 奖励池使用妮芙卡兹戴尔池生成的变体模板。

## 涉及哪些模组

问题类型：游戏接口兼容

- 运行前置：BaseLib（BaseLib）——本构建运行前置，按文件哈希匹配。
- 受影响模组：CyanrainNymphBeta（CyanrainNymphBeta）——奖励池使用妮芙卡兹戴尔池生成的变体模板。
- 运行前置：STS2-RitsuLib（STS2-RitsuLib）——本构建运行前置，按文件哈希匹配。

## 适用版本

- 配置 known-0.111.0-build：游戏 0.111.0；BaseLib v3.4.7、CyanrainNymphBeta v0.0.0-beta+exhaustfix.20261029、STS2-RitsuLib 0.6.7

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 卡池返回已可变的 Kazdel 模板，而奖励工厂继续调用 ToMutable；仅登记该池生成的确切实例，并用 MutableClone 产生独立副本，保留变体字段。
- 运行时按内嵌基线校验依赖；应用失败撤销本补丁已注册的 hooks。

玩法影响：保留卡兹戴尔变体及深复制数据，不把奖励替换为普通卡，也不放宽其他可变卡的断言。

涉及文件：

- NymphKazdelRewardFix/NymphKazdelRewardFix.dll：独立修复程序集，含兼容基线。
- NymphKazdelRewardFix/NymphKazdelRewardFix.json：游戏 MOD 加载清单。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

- 必须安装：local.cyanrainnymphbeta，版本至少 1.0.0。

## 依据与验证

诊断结论：已确认。卡池返回已可变的 Kazdel 模板，而奖励工厂继续调用 ToMutable；仅登记该池生成的确切实例，并用 MutableClone 产生独立副本，保留变体字段。

- 真实 Kazdel pool 和 GougeMemory 类型重现双重 ToMutable 失败。
- 两份奖励的变体、独立性、模板不变及其他可变卡仍被拒绝均已验证。

- 代码审查：通过。对照实际 DLL、日志及调用链；现场复现边界见限制。（2026-11-02）
- 自动化检查：通过。本轮 7 项隔离行为检查通过，另有 14 项运行时守卫及六项共同初始化检查。引擎/UI/网络边界使用夹具。（2026-11-02）
- 本机游戏测试：未执行。未启动真实游戏，未执行真人联机。
- 完整联机测试：未执行。未启动真实游戏，未执行真人联机。

## 限制与未解决事项

- 仅适配列明的 0.111.0 文件构建；未知版本阻止安装。
- 未进行真人联机/完整对局回归；文件安装成功不代表原现场已解决。
- 本版针对已应用消耗接口修复的妮芙 DLL；管理器自动关联该修复。

## 安装与撤销

在管理器中选择自己的游戏目录，查看改动计划。文件与依赖核对通过后，再通过已审核的安装清单安装。

支持回滚。在支持本批补丁的管理器中检查最近一次回滚，再确认撤销该安装批次；未知后续文件或损坏备份会停止回滚。不更改存档。

## 更新记录

- 1.0.0 · 2026-11-02：卡池返回已可变的 Kazdel 模板，而奖励工厂继续调用 ToMutable；仅登记该池生成的确切实例，并用 MutableClone 产生独立副本，保留变体字段。；使用管理器选择安装，不提供独立 CMD 修复包。

<details>
<summary>技术详情：文件与校验信息</summary>


匹配配置：known-0.111.0-build

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- BaseLib/BaseLib.dll：`55863CA6ADC30A61A7544874E157A519FFE148CCF2B18025947609C4BFF813CE`

- CyanrainNymphBeta/CyanrainNymphBeta.dll：`D23EFDD37AAE216CCF9D780EF97D3EAD2CDF8F5A2CF4031E0C6823AE707C5ED5`

- STS2-RitsuLib/compat/0.111.0/STS2-RitsuLib.Runtime.dll：`7A77583AF80792213C8DCD7572040A342E7EA0DBED7BBBC8ED8E20776445DCCF`

- STS2-RitsuLib/shared/STS2-RitsuLib.Shared.dll：`6FEF35E9BD8F39852CC81E6A800128AF572DF312356B565815910B7A0BEF13D7`

修复后文件特征：

- NymphKazdelRewardFix/NymphKazdelRewardFix.dll：`E4F5B75B270A976488FE78EEDB82BD10D0DBD9759762965E2D9F88040A03D830`

- NymphKazdelRewardFix/NymphKazdelRewardFix.json：`A843B5FF9489E7BAF8984D614982B6CF5C780EC86BEB07D8392742211862AF5A`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
