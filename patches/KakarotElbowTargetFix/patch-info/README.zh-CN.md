# 卡卡罗特肘击目标修复

版本：1.0.0　|　已发布

肘击伤害完成后施加易伤/虚弱时空引用。

## 出现什么问题

- 肘击伤害完成后施加易伤/虚弱时空引用。

## 什么时候可能遇到

- 肘击被改为随机/群体攻击，CardPlay.Target 为空。

## 涉及哪些模组

问题类型：单个模组问题

- 运行前置：BaseLib（BaseLib）——本构建运行前置，按文件哈希匹配。
- 受影响模组：Kakarot（Kakarot）——肘击被改为随机/群体攻击，CardPlay.Target 为空。

## 适用版本

- 配置 known-0.111.0-build：游戏 0.111.0；BaseLib v3.4.7、Kakarot 6.1.1

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 原卡把空 CardPlay.Target 交给 PowerCmd.Apply；空显式目标时执行原攻击一次，从实际 DamageResult.Receiver 去重取得目标，不重新抽取 RNG。
- 运行时按内嵌基线校验依赖；应用失败撤销本补丁已注册的 hooks。

玩法影响：对实际命中的目标各施加一次原数量的易伤和虚弱；显式单目标路径保持原卡逻辑。

涉及文件：

- KakarotElbowTargetFix/KakarotElbowTargetFix.dll：独立修复程序集，含兼容基线。
- KakarotElbowTargetFix/KakarotElbowTargetFix.json：游戏 MOD 加载清单。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：已确认。原卡把空 CardPlay.Target 交给 PowerCmd.Apply；空显式目标时执行原攻击一次，从实际 DamageResult.Receiver 去重取得目标，不重新抽取 RNG。

- 原 KakarotElbowStrike.OnPlay 与 KakarotPowerCmd 转发链核对。
- 真实 OnPlay 原始空目标异常重现；0/1/2 目标、重复命中与显式目标路径通过。

- 代码审查：通过。对照实际 DLL、日志及调用链；现场复现边界见限制。（2026-11-02）
- 自动化检查：通过。本轮 11 项隔离行为检查通过，另有 14 项运行时守卫及六项共同初始化检查。引擎/UI/网络边界使用夹具。（2026-11-02）
- 本机游戏测试：未执行。未启动真实游戏，未执行真人联机。
- 完整联机测试：未执行。未启动真实游戏，未执行真人联机。

## 限制与未解决事项

- 仅适配列明的 0.111.0 文件构建；未知版本阻止安装。
- 未进行真人联机/完整对局回归；文件安装成功不代表原现场已解决。
- 能天使的转发出现在堆栈中，但没有证据表明它是根因。

## 安装与撤销

在管理器中选择自己的游戏目录，查看改动计划。文件与依赖核对通过后，再通过已审核的安装清单安装。

支持回滚。在支持本批补丁的管理器中检查最近一次回滚，再确认撤销该安装批次；未知后续文件或损坏备份会停止回滚。不更改存档。

## 更新记录

- 1.0.0 · 2026-11-02：原卡把空 CardPlay.Target 交给 PowerCmd.Apply；空显式目标时执行原攻击一次，从实际 DamageResult.Receiver 去重取得目标，不重新抽取 RNG。；使用管理器选择安装，不提供独立 CMD 修复包。

<details>
<summary>技术详情：文件与校验信息</summary>


匹配配置：known-0.111.0-build

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- BaseLib/BaseLib.dll：`55863CA6ADC30A61A7544874E157A519FFE148CCF2B18025947609C4BFF813CE`

- Kakarot/Kakarot.dll：`839C486E78516A5D5A32995880848778F2CB0935E557CAE0CF3071CAA7DA4E40`

修复后文件特征：

- KakarotElbowTargetFix/KakarotElbowTargetFix.dll：`634947F0278B9B75381EF97381CFE06CB7331F13C4A81D933105E934DA9F9EC3`

- KakarotElbowTargetFix/KakarotElbowTargetFix.json：`6DC35A68A5B38C1F7B7D46DAD131AD793A6F2FFC7E90357C8BBEDBADD3882DB9`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
