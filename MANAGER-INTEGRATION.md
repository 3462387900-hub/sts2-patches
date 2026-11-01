# 本批补丁的管理器接入

用户要求先上传补丁仓库，管理器后续自行操作。本次不改管理器源码或现有在线入口，不宣称管理器端到端安装已经通过。

## 可直接复用的数据

Release `v2026.11.02.3` 包含六份 `<组件>-1.0.0.zip`、对应 `.recipe.json`，以及供现有管理器协议接入的 `STS2-Manager-Payload-20261102-v3.zip`。后者是下载缓存，保留旧 11 项并增加本批 6 项；实际安装仍按用户选择执行。所有载荷均不含独立 CMD 启动器。

每项 `patch-info/patch.json` 的 distribution 给出单项附件与配方的精确哈希。单项配方沿用现有 `plan.json` 的 version、components、dependencies、retired 结构。当前配方版本是 `20261102.3`。单项数据包不携带可执行安装脚本，供管理器核对后写入。

新组件只安装两个文件：同名 DLL 和同名 MOD 清单。运行时基线编译进 DLL；结构化 patch-info 和 schema 只保留在仓库/缓存，避免被游戏递归扫描成 MOD 清单。

## 现有管理器需要接入的地方

核对时管理器 main 为 `c72ccc4d1454c45e17ffa591b7e3f91a57b1cdc3`（0.9.0）。后续操作应重新读取最新版本，勿覆盖其他更新。

1. 将 `index.json` 及 17 项完整 metadata 更新为内置目录快照；记录对应补丁仓库 commit。`Catalog.ReviewedBundle` 和 `SelectionPlanner` 不能仅凭在线发布状态跳过既有审核。
2. 将已下载载荷中的 `plan.json` 更新为 `catalog-plan.json`，保留旧文件白名单与关联条件。不要仅使用旧计划或只追加标题。
3. 更新 `BundleInstaller` 的文件名、顶层目录、长度、SHA256 和附件 URL。用 GitHub 返回的 digest 再核对一次附件，禁止只改 URL。
4. `InstallerBridge` 接受新的 `20261102.3` 备份版本，同时保留旧 `.1` / `.2` 的回滚识别。
5. 新增妮芙奖励修复关联 `local.cyanrainnymphbeta`，快照修复关联 `local.act4.ambient-timer-fix`；保持依赖闭包与预览 token 校验。
6. 完成真实管理器的目录加载、选择安装、依赖变更拒绝、重复 ID、损坏下载、中文空格路径、取消和最近批次回滚回归后，再发布管理器并切换正式 `manager-feed.json`。

本仓库的 `manager-feed-v3.json` 是接入候选：固定已上传的目录 commit 和附件指纹，install_mode 为 `manager_payload`。旧管理器只接受 `manual_bundle`，不可直接将候选替换为正式入口而不更新客户端。

## 已测与未测

已用同源 Common/SelectionBridge 在带标记、零字节游戏 EXE 的隔离目录检查六项独立配方、安装和回滚共 18 项。未执行用户当前管理器 UI 安装，未触碰真实游戏目录，也未发布管理器新版。具体边界见 SIX-FIXES-VALIDATION.md。
