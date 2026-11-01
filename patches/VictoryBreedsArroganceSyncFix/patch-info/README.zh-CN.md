# 猪猪胜兵必骄联机判定修复

版本：1.0.0　|　已发布

同次出牌在不同客户端判断是否获得力量的结果不同。

## 出现什么问题

- 同次出牌在不同客户端判断是否获得力量的结果不同。

## 什么时候可能遇到

- 胜兵必骄比较敌方意图伤害，双方玩家受伤修正不同。

## 涉及哪些模组

问题类型：单个模组问题

- 受影响模组：YuWanCard（YuWanCard）——胜兵必骄比较敌方意图伤害，双方玩家受伤修正不同。

## 适用版本

- 配置 known-0.111.0-build：游戏 0.111.0；YuWanCard v0.5.12

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 该卡调用的意图工具最终读取 LocalContext.GetMe；只在该卡同步计算范围内显式使用出牌者作为受伤方，保留多段意图和原阈值判断。
- 运行时按内嵌基线校验依赖；应用失败撤销本补丁已注册的 hooks。

玩法影响：此卡按出牌者防御修正计算；其他 UI 意图预览仍依照本地玩家。

涉及文件：

- VictoryBreedsArroganceSyncFix/VictoryBreedsArroganceSyncFix.dll：独立修复程序集，含兼容基线。
- VictoryBreedsArroganceSyncFix/VictoryBreedsArroganceSyncFix.json：游戏 MOD 加载清单。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：已确认。该卡调用的意图工具最终读取 LocalContext.GetMe；只在该卡同步计算范围内显式使用出牌者作为受伤方，保留多段意图和原阈值判断。

- 真实卡 OnPlay 的 async MoveNext 中找到唯一意图工具调用。
- 真实 AttackIntent/Hook 边界测试原来 60/20，补丁后两端一致；异常后恢复作用域。

- 代码审查：通过。对照实际 DLL、日志及调用链；现场复现边界见限制。（2026-11-02）
- 自动化检查：通过。本轮 10 项隔离行为检查通过，另有 14 项运行时守卫及六项共同初始化检查。引擎/UI/网络边界使用夹具。（2026-11-02）
- 本机游戏测试：未执行。未启动真实游戏，未执行真人联机。
- 完整联机测试：未执行。未启动真实游戏，未执行真人联机。

## 限制与未解决事项

- 仅适配列明的 0.111.0 文件构建；未知版本阻止安装。
- 未进行真人联机/完整对局回归；文件安装成功不代表原现场已解决。

## 安装与撤销

在管理器中选择自己的游戏目录，查看改动计划。文件与依赖核对通过后，再通过已审核的安装清单安装。

支持回滚。在支持本批补丁的管理器中检查最近一次回滚，再确认撤销该安装批次；未知后续文件或损坏备份会停止回滚。不更改存档。

## 更新记录

- 1.0.0 · 2026-11-02：该卡调用的意图工具最终读取 LocalContext.GetMe；只在该卡同步计算范围内显式使用出牌者作为受伤方，保留多段意图和原阈值判断。；使用管理器选择安装，不提供独立 CMD 修复包。

<details>
<summary>技术详情：文件与校验信息</summary>


匹配配置：known-0.111.0-build

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- YuWanCard/lib/0.111.0/YuWanCard.Content.dll：`3D03979E0B951E113D50C3E154D220276608B020158E0D60EBB1E55A7491BE2F`

修复后文件特征：

- VictoryBreedsArroganceSyncFix/VictoryBreedsArroganceSyncFix.dll：`7EDA9F2B76F871ADD8B9FF278040FBDE17E72B5F68C77892BC1136AE89A91EF5`

- VictoryBreedsArroganceSyncFix/VictoryBreedsArroganceSyncFix.json：`0469F397DEC693707597A71A49A066A0918E5224821C316E07CA434710DB29A0`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
