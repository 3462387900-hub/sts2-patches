# 杀戮尖塔 2 补丁库

当前目录共 18 项修复。每项按 `patches/<组件>/patch-info/patch.json` 记录问题、适配文件哈希、联机要求、验证边界及回滚说明；同目录提供中文说明和 schema。

## 2026-11-05 新增莱伊联机修复

[莱伊联机结算兼容修复](patches/RayCoopCompatibilityFix/patch-info/README.zh-CN.md)提供结束战斗后的延迟抽牌防护，以及白鲸药水下莱伊攻击的同步目标兼容。首次牌堆／升级差异未定位，真人双机实战待复测。管理器 0.12.5 接入本条目，使用审核载荷 v2026.11.05.1；不要把 patch-info 复制进游戏。

## 2026-11-02 新增六项

| 组件 | 修复范围 |
| --- | --- |
| [TurnEndOrderFix](patches/TurnEndOrderFix/patch-info/README.zh-CN.md) | 回合结束选牌与后续钩子的执行顺序 |
| [VictoryBreedsArroganceSyncFix](patches/VictoryBreedsArroganceSyncFix/patch-info/README.zh-CN.md) | 胜兵必骄使用本地意图预览造成的联机分支差异 |
| [KakarotElbowTargetFix](patches/KakarotElbowTargetFix/patch-info/README.zh-CN.md) | 肘击随机/群体目标后的空引用 |
| [NymphKazdelRewardFix](patches/NymphKazdelRewardFix/patch-info/README.zh-CN.md) | 卡兹戴尔奖励模板重复 ToMutable |
| [Act3SnapshotSaveFix](patches/Act3SnapshotSaveFix/patch-info/README.zh-CN.md) | 第四幕私有第三幕快照缺少 JSON 元数据 |
| [NeowOptionOrderFix](patches/NeowOptionOrderFix/patch-info/README.zh-CN.md) | 涅奥模组候选追加顺序导致同索引不同选项 |

本批仅交付独立补丁数据及管理器安装配方，不制作 Install.cmd / Rollback.cmd 修复包。获取 [Release v2026.11.02.3](https://github.com/3462387900-hub/sts2-patches/releases/tag/v2026.11.02.3)。每项 ZIP 只有本组件 DLL 和 MOD 清单；SHA256 在各自 patch.json 与发布附件中。

**管理器 0.10.0 已接入全部 17 项，正式 manager-feed.json 已同步。** 请先在管理器「检查更新」升级，再刷新补丁源并重新扫描。下载 [尖塔修补站 0.10.0](https://github.com/3462387900-hub/sts2-patch-manager/releases/tag/v0.10.0)。新增六项可选择安装、参与联机差异补齐并按批次回滚；已有匹配文件跳过。不要将整个仓库或 patch-info JSON 复制进游戏 mods。

后续上传请遵循 [补丁发布指南](https://github.com/3462387900-hub/sts2-patch-manager/blob/main/补丁发布指南.md)：补丁说明与 index → 发布载荷 → 接入并发布管理器 → 校验并切换 manager-feed。仅上传文件不会自动扩大旧管理器的安装能力。提供 [默认只校验的发布脚本](https://github.com/3462387900-hub/sts2-patch-manager/blob/main/scripts/发布补丁源.ps1)。接入与验证边界见 [MANAGER-INTEGRATION.md](MANAGER-INTEGRATION.md)。

## 验证与适用范围

六项核心行为测试 59 项，守卫/共同初始化测试 14 项，安装配方与事务检查 18 项通过。使用当前 0.111.0 游戏及具体模组 DLL 构建；未知哈希拒绝安装或初始化。完整记录见 [SIX-FIXES-VALIDATION.md](SIX-FIXES-VALIDATION.md)。

测试在隔离环境完成，未修改真实游戏和存档，未进行真人联机或完整对局。涅奥追加顺序是有代码与日志支持的机制，尚不能断言是原局唯一原因。所有参与联机的玩家须使用一致补丁及适用模组配置。

## 维护

- src/、tests/：本批源码、内嵌兼容基线和隔离测试。构建方法见 [src/README.md](src/README.md)。
- index.json：18 项说明的路径索引。
- manager-plan.json：管理器数据载荷的文件配方；所有写入按哈希白名单执行。
- tools/build_manager_payload.py：校验元数据、关联、文件哈希并构建管理器缓存载荷，无 CMD 启动器。
- installer/、tools/build_bundle.py：历史 11 项整合包材料，保留供历史追溯；不用于本批新补丁交付。

旧 11 项补丁二进制与说明保持原样，未把历史测试冒充本轮回归。SpireTogetherFix 不属于本批修复或自动补装范围。

第三方游戏和模组的权利归原作者。新增源码和补丁不包含游戏本体、第三方引用库、存档、个人日志或账号信息。
