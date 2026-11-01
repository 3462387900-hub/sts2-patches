# 本批补丁的管理器接入

管理器 [0.10.0](https://github.com/3462387900-hub/sts2-patch-manager/releases/tag/v0.10.0) 已完成接入，正式 `manager-feed.json` 已指向 17 项目录及 v3 载荷。玩家先更新管理器，再刷新补丁源、重新扫描；联机双方重新导出清单。

后续发布见 [补丁发布指南](https://github.com/3462387900-hub/sts2-patch-manager/blob/main/补丁发布指南.md) 和 [发布校验脚本](https://github.com/3462387900-hub/sts2-patch-manager/blob/main/scripts/发布补丁源.ps1)。脚本默认只校验，增加 `-Publish` 才更新正式入口。补丁的预发布标记与历史说明状态保持原样，不把接入测试当成真人联机验证。

## 可直接复用的数据

Release `v2026.11.02.3` 包含六份 `<组件>-1.0.0.zip`、对应 `.recipe.json`，以及供现有管理器协议接入的 `STS2-Manager-Payload-20261102-v3.zip`。后者是下载缓存，保留旧 11 项并增加本批 6 项；实际安装仍按用户选择执行。所有载荷均不含独立 CMD 启动器。

每项 `patch-info/patch.json` 的 distribution 给出单项附件与配方的精确哈希。单项配方沿用现有 `plan.json` 的 version、components、dependencies、retired 结构。当前配方版本是 `20261102.3`。单项数据包不携带可执行安装脚本，供管理器核对后写入。

新组件只安装两个文件：同名 DLL 和同名 MOD 清单。运行时基线编译进 DLL；结构化 patch-info 和 schema 只保留在仓库/缓存，避免被游戏递归扫描成 MOD 清单。

## 本次已完成的管理器接入

接入前版本为 0.9.0；0.10.0 实现提交为 `5219bb06beb44ec6c69261ba02d43303cb73d576`，目录固定为补丁仓库 `f487b2bfa90fd3741221197b9185b9ba272add16`。以下清单作为后续接入检查表。每次操作前仍应重新读取最新版本。

1. 将 `index.json` 及 17 项完整 metadata 更新为内置目录快照；记录对应补丁仓库 commit。`Catalog.ReviewedBundle` 和 `SelectionPlanner` 不能仅凭在线发布状态跳过既有审核。
2. 将已下载载荷中的 `plan.json` 更新为 `catalog-plan.json`，保留旧文件白名单与关联条件。不要仅使用旧计划或只追加标题。
3. 更新 `BundleInstaller` 的文件名、顶层目录、长度、SHA256 和附件 URL。用 GitHub 返回的 digest 再核对一次附件，禁止只改 URL。
4. `InstallerBridge` 接受新的 `20261102.3` 备份版本，同时保留旧 `.1` / `.2` 的回滚识别。
5. 新增妮芙奖励修复关联 `local.cyanrainnymphbeta`，快照修复关联 `local.act4.ambient-timer-fix`；保持依赖闭包与预览 token 校验。
6. 完成真实管理器的目录加载、选择安装、依赖变更拒绝、重复 ID、损坏下载、中文空格路径、取消和最近批次回滚回归后，再发布管理器并切换正式 `manager-feed.json`。

本仓库的 `manager-feed-v3.json` 是本次候选记录，已核对并同步到正式 `manager-feed.json`，install_mode 为 `manager_payload`。0.9.0 及更早管理器不支持该协议，请通过独立的「检查更新」入口升级。

## 已测与未测

补丁制作阶段的行为与配方验证记录见 [SIX-FIXES-VALIDATION.md](SIX-FIXES-VALIDATION.md)。

管理器接入阶段另完成：新增六项逐项与组合事务 43 项、原有选装回归 20 项、完整安装回归 25 项；WPF 实际按钮检查 17 项可见、必要关联、预览/安装/回滚及仅补说明流程。发布自检涵盖推荐、下载、程序更新、联机规则与目录配方一致性。旧 .1/.2 备份版本兼容以模拟日志验证，不冒充所有历史备份实测。

游戏写入只发生在有标记、零字节游戏 EXE 的隔离目录。真实游戏、MOD 和存档未修改；未新增真人联机或完整对局验证。程序 Release 已发布，匿名更新下载校验通过。
