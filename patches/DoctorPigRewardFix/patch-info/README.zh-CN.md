# 博士猪猪奖励兼容修复 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

奖励生成需尊重 Tiphera 已解锁牌池；保持随机数推进、升级和取消行为。

## 出现什么问题

- 博士猪猪事件或类猪体奖励选牌异常。

## 什么时候可能遇到

- YuWanCard 奖励逻辑遇到 Tiphera 受限可用牌池。

## 涉及哪些模组

问题类型：模组组合冲突

- 受影响模组：猪猪（YuWanCard）——YuWanCard 奖励逻辑遇到 Tiphera 受限可用牌池。
- 触发条件涉及的模组：CznTipheraCharacter（CznTipheraCharacter）——YuWanCard 奖励逻辑遇到 Tiphera 受限可用牌池。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；CznTipheraCharacter 2.0.1-beta、YuWanCard v0.5.12

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 奖励生成需尊重 Tiphera 已解锁牌池；保持随机数推进、升级和取消行为。

玩法影响：改变异常奖励的生成和过滤，使其符合角色牌池；并非纯显示修复。

涉及文件：

- DoctorPigRewardFix/DoctorPigRewardFix.dll：既有已核对修复文件；本次保持字节不变。
- DoctorPigRewardFix/DoctorPigRewardFix.json：既有已核对修复文件；本次保持字节不变。
- DoctorPigRewardFix/runtime-baseline.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。奖励生成需尊重 Tiphera 已解锁牌池；保持随机数推进、升级和取消行为。

- 既有补丁清单和整合包说明已核对。
- 随补丁保存的 patch-info/historical-validation.txt。

- 代码审查：部分验证。核对现有清单、整合说明及可用的历史证据，未重新审查所有源码。（2026-11-02）
- 自动化检查：通过。历史 27 项行为检查通过；真实事件、随机数与牌池过滤，界面/存档/网络使用测试替身。
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

- CznTipheraCharacter/CznTipheraCharacter.dll：`0F072823C66DC790564607A98A2F6C639322F8957E6A8F9F108F743CDC691943`

- YuWanCard/lib/0.111.0/YuWanCard.Content.dll：`3D03979E0B951E113D50C3E154D220276608B020158E0D60EBB1E55A7491BE2F`

修复后文件特征：

- DoctorPigRewardFix/DoctorPigRewardFix.dll：`1D429BCD263AB679D4561C0ED84CCD7ACA77CD501E529C27E8AE3172F69F7942`

- DoctorPigRewardFix/DoctorPigRewardFix.json：`28DF6FF350807EDC2CA1586A77D09B08969B8EE5739844A60757A14DD29A725C`

- DoctorPigRewardFix/runtime-baseline.json：`B6FCB18337E25F1792DA309635A588E9B88217AD5823EDF9FA8413765282D69D`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
