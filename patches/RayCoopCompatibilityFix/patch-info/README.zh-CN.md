# 莱伊联机结算兼容修复

版本：1.0.0　|　已发布

防止结束战斗后补发延迟抽牌；兼容白鲸药水下莱伊攻击的空目标。

## 出现什么问题

- 莱伊／空弦联机局中，8 次不同步均为队友端多出 1 层下回合抽牌。
- 乌萨斯白鲸生效时 SandView、Entrance 单体攻击报空引用。

## 什么时候可能遇到

- 敌方 SpinToWin 将非起手抽牌延迟到下回合，战斗结束边界仍触发转换。
- 能天使白鲸药水把单体攻击改为随机攻击，莱伊卡牌仍读取空 CardPlay.Target。

## 涉及哪些模组

问题类型：模组组合冲突

- 受影响模组：莱伊（Ray）——白鲸改为随机攻击后仍读取显式目标。
- 触发条件涉及的模组：能天使（AK_Exusiai）——白鲸药水改变单体攻击目标类型。
- 受影响模组：海克斯大乱斗（HextechRunes）——敌方延迟抽牌转换在结束战斗边界需要保护。
- 运行前置：RitsuLib（STS2-RitsuLib）——出牌钩子入口与次要资源数据。
- 运行前置：BaseLib（BaseLib）——当前组合的运行前置。

## 适用版本

- 配置 ray-0.1.0-game-0.111.0：游戏 0.111.0；Ray 0.1.0、AK_Exusiai 1.1.0、BaseLib v3.4.7、HextechRunes 0.9.8、STS2-RitsuLib 0.6.7

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 延迟抽牌转换入口按 IsOverOrEnding 拒绝结束后的状态补发，正常战斗保留原逻辑。
- 白鲸下无显式目标的莱伊攻击使用同步 CombatTargets 随机流选择目标，保留原 CardPlay 对象。
- 无敌人不进行目标抽取；已有目标、其他角色不走本分支。
- 运行前校验内嵌精确哈希，安装失败撤销本补丁钩子。

玩法影响：保留正常抽牌转换；莱伊与白鲸组合从空引用改为一次同步随机目标选择。不会屏蔽不同步校验。

涉及文件：

- RayCoopCompatibilityFix/RayCoopCompatibilityFix.dll：独立修复程序集及兼容基线。
- RayCoopCompatibilityFix/RayCoopCompatibilityFix.json：游戏模组加载清单。

## 我需要和队友一起装吗

所有玩家安装。所有联机玩家安装同一补丁与匹配原模组，并重启后再联机。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。白鲸空目标路径已由实际 DLL 与异常堆栈确认；海克斯转换入口缺少 IsOverOrEnding 防护，现补齐结束边界。重复不同步日志符合此风险，但未记录完整异步加状态调用栈，不宣称全部根因已证实。

- 9 次独立不同步中 8 次为同一层延迟抽牌差异；后 5 份双方快照已无敌人。
- 历史记录第三幕敌方海克斯含 SpinToWin（150）。
- PowerCmd 使用 IsEnding，战斗退出后该值为 false；转换入口新增 IsOverOrEnding。
- SandView、Entrance 的 Targeting(null) 堆栈与 BelugaRandomTargetPatch 对照。

- 代码审查：通过。实际 DLL、9 次差异记录及空目标堆栈核对；异步根因仅部分确认。（2026-11-05）
- 自动化检查：通过。真实 DLL 的 Harmony 安装及哈希守卫、结束/正常战斗分支、100 次同种子选目标、空目标零 RNG、已有目标保留；引擎边界隔离。（2026-11-05）
- 本机游戏测试：部分验证。当前整套模组无界面限帧启动，新增补丁初始化成功；未进入战斗。（2026-11-05）
- 完整联机测试：未执行。未进行修复后的真人双机战斗。

## 限制与未解决事项

- 只支持列明的文件哈希；版本号相同但 DLL 不同仍拒绝安装。
- 首次 #697 的 OverMountain 牌堆／升级差异未定位，不在已解决范围。
- 8 次延迟抽牌差异的完整异步调用链未被日志捕获，此项是针对性边界防护。
- 未完成真人双机和完整对局验证；启动或文件匹配不代表全部不同步已经解决。

## 安装与撤销

在管理器中选择自己的游戏目录，查看改动计划。文件与依赖核对通过后，再通过已审核的安装清单安装。

支持回滚。在支持本批补丁的管理器中检查最近一次回滚，再确认撤销该安装批次；未知后续文件或损坏备份会停止回滚。不更改存档。

## 更新记录

- 1.0.0 · 2026-11-05：新增结束战斗延迟抽牌防护及莱伊白鲸目标兼容。；通过管理器选择安装、联机对齐和按批次回滚，不提供独立 CMD 安装器。

<details>
<summary>技术详情：文件与校验信息</summary>


匹配配置：ray-0.1.0-game-0.111.0

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- Ray/Ray.dll：`D4960AE64B535691861A101DB4E780D6BB92868485BB52692C483146155F92A1`

- AK_Exusiai/AK_Exusiai.dll：`AFFEB5A53BBF3E85F3CD6D8804CA644F14F6C431FB761A545D4933F27842F75E`

- BaseLib/BaseLib.dll：`55863CA6ADC30A61A7544874E157A519FFE148CCF2B18025947609C4BFF813CE`

- HextechRunes/lib/0.111.0/HextechRunes.dll：`8C203D53C13A7D9CA89F5B9CA4498039D2CFB152E4685727497351FB60F11A2B`

- STS2-RitsuLib/compat/0.111.0/STS2-RitsuLib.dll：`DB62D08E44DF951F1CA5A10FC720EDCF2102401583EA04B8C493AD4EEECA2427`

- STS2-RitsuLib/compat/0.111.0/STS2-RitsuLib.Runtime.dll：`7A77583AF80792213C8DCD7572040A342E7EA0DBED7BBBC8ED8E20776445DCCF`

- STS2-RitsuLib/shared/STS2-RitsuLib.Shared.dll：`6FEF35E9BD8F39852CC81E6A800128AF572DF312356B565815910B7A0BEF13D7`

- STS2-RitsuLib/shared/STS2-RitsuLib.Settings.dll：`3A48B5A62E0FFA01C8527686B40AB16FF4B0B87D969AC6A20A3141BC7C7867E4`

- STS2-RitsuLib/shared/STS2-RitsuLib.Ui.dll：`AC02253F9F4ACEAC0DEFAFF27C8AA7B10201A8B164FB8FEBE6CDA1C43294D19E`

修复后文件特征：

- RayCoopCompatibilityFix/RayCoopCompatibilityFix.dll：`A4316DD36F711E559FA13B683F1FF3E3981480F99870771D33B61AE612EDE3DC`

- RayCoopCompatibilityFix/RayCoopCompatibilityFix.json：`7382AB9A8A69C876F09777F551F1FDC24629795992ACD975B858A211EDD077FD`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
