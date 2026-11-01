# 第四幕建筑家 BOSS 入场卡死修复

版本：1.0.0　|　草稿 · 不可自动安装

修正建筑家环境特效循环的等待，避免入场时持续创建特效而未响应。

## 出现什么问题

- 进入建筑家 BOSS 战后窗口无响应，尚未进入第一个玩家回合。
- 卡住期间内存持续增长；普通日志可能没有 ERROR。

## 什么时候可能遇到

- 已复现于当前模组组合、双人联机、第四幕 Brutal 的建筑家入场阶段。
- 是否在其他难度或单人模式同样触发，尚未验证。

## 涉及哪些模组

问题类型：单个模组问题

- 受影响模组：第四幕：终焉攀升（Act4FinalAscent）——已定位到建筑家自身的环境特效循环。
- 需联动处理的组件：联机兼容修复包（SpireTogetherFix）——若已安装，它的校验文件需要同步更新；这不是对它造成原始卡死的认定。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；Act4FinalAscent 0.1.3
- 配置 known-build-2：游戏 0.111.0；Act4FinalAscent 0.1.3

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 将特效循环的等待调用换成独立 Godot 定时器。
- 保留原有 1.1 / 0.8 / 0.55 秒阶段间隔及循环退出条件。

玩法影响：未修改 BOSS 数值、招式、奖励或存档；保留原有特效。

涉及文件：

- Act4FinalAscent/Act4FinalAscent.dll：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。本地特效在各自客户端运行，所有玩家应安装匹配修复；旧联机兼容补丁的校验文件也需同步。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。主机继续现有存档后已进入玩家回合，但尚未完成整场双人联机验证，不能据此承诺所有存档均兼容。

## 关联补丁

- 检测到 SpireTogetherFix 时，还需要 local.spiretogetherfix（至少 1.0.0）。

## 依据与验证

诊断结论：部分确认。两次线程采样定位到建筑家环境特效循环持续创建对象；循环等待在部分游戏状态下可立即返回。具体是哪项状态触发跳过等待，尚未直接读取确认。

- 两次未响应线程采样均包含 Act4ArchitectBoss.RunAmbientVfxLoopAsync，并处于特效实例化路径。
- 同次进程工作集由约 7.8 GB 增至 15.4 GB。
- 反编译核对了循环调用 Cmd.Wait 的行为及其跳过等待的条件。

- 代码审查：通过。2060 个现有方法体比较，仅环境循环一处调用改变，增加一个定时器辅助方法。（2026-11-02）
- 自动化检查：通过。编译与 JIT 检查、安装/回滚、中文空格路径及错误版本拒绝测试通过。（2026-11-02）
- 本机游戏测试：部分验证。主机修复后的日志确认进入建筑家玩家回合；尚未验证全部阶段。（2026-11-02）
- 完整联机测试：未执行。没有完整的双方修复后联机回归记录。

## 限制与未解决事项

- 本轮仅重新包装和补齐说明，未重跑整场 BOSS 联机。
- 只支持已核对的文件构建。
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

- Act4FinalAscent/Act4FinalAscent.dll：`1100CE4C1BDC5140373EFC4AA39C9F61941BC39B0362ED37B0FE3BEA595F96DA`

匹配配置：known-build-2

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- Act4FinalAscent/Act4FinalAscent.dll：`38F2FE4F89009113D7991F1FD47044B7FB2EBBD0F60836A6310C6103BE1D7276`

修复后文件特征：

- Act4FinalAscent/Act4FinalAscent.dll：`1100CE4C1BDC5140373EFC4AA39C9F61941BC39B0362ED37B0FE3BEA595F96DA`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
