# 构建与验证

.NET SDK 9；引用来自本机合法安装，不随仓库分发。

```powershell
dotnet build src/RayCoopCompatibilityFix/Fix.csproj -c Release -p:GameRefsDir="<含 sts2.dll、0Harmony.dll、GodotSharp.dll 的目录>"
dotnet run --project tests/RayCoopCompatibilityFix -c Release -p:GameRefsDir="<游戏程序集目录>" -- "<本地程序集映射.json>"
```

映射 JSON 为 `{"程序集名称":"本机DLL绝对路径"}`，覆盖 baseline.json 内名称；Ritsu 和海克斯选 0.111.0 对应运行库。不要提交该本地路径映射或第三方 DLL。

禁用自动附加 Git SHA 到 InformationalVersion，因此迁入仓库重新构建仍与已安装 1.0.0 的 A4316DD36F711E559FA13B683F1FF3E3981480F99870771D33B61AE612EDE3DC 完全一致。

测试使用真实程序集与 Harmony，隔离战斗状态和加状态命令边界；不等于实际双机战斗复现。
