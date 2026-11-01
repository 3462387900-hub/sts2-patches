# 六项独立修复源码

使用 .NET SDK 9。引用必须来自玩家合法安装的游戏/模组；仓库不提供这些第三方程序集。`baseline.json` 是编译进各补丁 DLL 的精确兼容基线，不会作为游戏 MOD 清单安装。

```powershell
dotnet build tests/Tests.csproj -c Release -p:GameRefsDir=<依赖DLL绝对目录>
dotnet tests/bin/Release/net9.0/Tests.dll <依赖DLL绝对目录> turn
```

行为测试入口：`turn`、`victory`、`elbow`、`nymph`、`snapshot`、`neow`、`guards`。测试目录还需 Sentry.dll、Sentry.Godot.dll 及对应 Mod 的运行依赖。Kakarot.dll 的程序集标识是 KakarotMod，测试引用目录需将其另存为 KakarotMod.dll，游戏安装本身不改名。

所有测试都在独立进程执行，引擎/UI/网络边界使用夹具。并非真人联机或完整 ActionExecutor 回归。源码中的哈希、最终 DLL 与 patch.json 指纹必须一同更新；不能只改变源码就沿用已发布下载校验值。

新补丁各自只有 DLL 和同名 MOD 清单需要安装。结构化说明和 schema 留在仓库及管理器缓存内，避免游戏将它们误识别成 MOD 清单。使用管理器选择安装；不生成 Install.cmd/Rollback.cmd。
