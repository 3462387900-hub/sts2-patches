# 初始牌组调整 · 本地修复说明

版本：1.0.0　|　草稿 · 不可自动安装

将该构建的清单 has_pck 设为 false；DLL 不变。

## 出现什么问题

- 启动时尝试加载不存在的 PCK。

## 什么时候可能遇到

- 已核对的纯 DLL 初始牌组调整构建仍声明 has_pck=true。

## 涉及哪些模组

问题类型：单个模组问题

- 受影响模组：初始牌组调整（StartingDeckTweaks）——已核对的纯 DLL 初始牌组调整构建仍声明 has_pck=true。

## 适用版本

- 配置 known-build-1：游戏 0.111.0；StartingDeckTweaks 1.0.0
- 配置 known-build-2：游戏 0.111.0；StartingDeckTweaks 1.0.0

版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。

## 修了什么

- 将该构建的清单 has_pck 设为 false；DLL 不变。

玩法影响：只修改资源清单，不修改牌组调整 DLL。

涉及文件：

- StartingDeckTweaks/StartingDeckTweaks.json：既有已核对修复文件；本次保持字节不变。

## 我需要和队友一起装吗

所有玩家安装。内部组合统一使用相同修复文件；这是一致性要求，不等于完整联机回归已通过。

安装后需要重启游戏。

## 能继续已有存档吗

尚不能确认。本轮未验证所有已有存档和游戏场景。

## 关联补丁

尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。

## 依据与验证

诊断结论：部分确认。将该构建的清单 has_pck 设为 false；DLL 不变。

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

- StartingDeckTweaks/StartingDeckTweaks.dll：`497B4719E7D6F0E8BC173069FAE72BF81C139F80D7521DE4B2BB7D78F30CE275`

- StartingDeckTweaks/StartingDeckTweaks.json：`0B5F46C8A14AE68846F357269B83813437423F83267B60C7ADFBB3445E809B65`

匹配配置：known-build-2

- 游戏/data_sts2_windows_x86_64/sts2.dll：`0861BFA1DF347538D932F22D580E75420F08082792EB914E53B4882764ACDBE9`

- 游戏/data_sts2_windows_x86_64/0Harmony.dll：`EF1898322C9F5C86DC1B0758B272A9C440823B4A41CA9A0B82A3AA6B3D206387`

- 游戏/data_sts2_windows_x86_64/GodotSharp.dll：`0E4897ECDFB31456A97C7D8028DFB8D7DBDC632E2F73FC9B438D7B266A139289`

- StartingDeckTweaks/StartingDeckTweaks.dll：`497B4719E7D6F0E8BC173069FAE72BF81C139F80D7521DE4B2BB7D78F30CE275`

- StartingDeckTweaks/StartingDeckTweaks.json：`575BAC6AE6CD178F1D1EEF7C58ABAD0038E7A2A1B45ABEE577CD0D60E0DF1E7F`

修复后文件特征：

- StartingDeckTweaks/StartingDeckTweaks.json：`0B5F46C8A14AE68846F357269B83813437423F83267B60C7ADFBB3445E809B65`

文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。

</details>
