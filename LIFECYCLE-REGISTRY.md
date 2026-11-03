# 上游修复证据库

`lifecycle-registry.json` 初始为空。只有确认原作者修复了同一问题，才能添加 `fixed_upstream` 项。MOD 更新或版本变化本身不是修复证明。

每项必须包含 `patch_id`、`target_mod_id`、`relative_path`（DLL 相对路径）、`sha256`（实际验证的 64 位哈希）、`version`、`decision`、`review_status`、`note` 和 `sources`（具体更新记录或测试依据的 HTTPS 链接）。

`decision` 仅允许 `fixed_upstream`、`suspected_fixed`、`incompatible`；`review_status` 仅允许 `candidate`、`approved`。确认已修复需要 `fixed_upstream` 和 `approved` 同时成立，并且本机 DLL 哈希精确匹配。只描述“修复了一些 BUG”的日志不应标记为 `approved`。

请把证据绑定到原 MOD 的相关 DLL；如果是游戏本体问题，可以用 `target_mod_id` 为 `GAME`，路径为相对游戏根目录的 DLL。管理器只读取声明，不运行在线指令，不会自动删除文件。退役独立补丁需要用户确认，原 MOD 内置修复不自动替换。
