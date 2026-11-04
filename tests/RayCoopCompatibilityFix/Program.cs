using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
var map=JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText(args[0]))!;
map["RayCoopCompatibilityFix"]=Path.Combine(AppContext.BaseDirectory,"RayCoopCompatibilityFix.dll");
foreach(var p in Directory.GetFiles(Path.GetDirectoryName(map["sts2"])!,"*.dll")) map.TryAdd(Path.GetFileNameWithoutExtension(p),p);
AssemblyLoadContext.Default.Resolving+=(c,n)=>map.TryGetValue(n.Name!,out var p)?c.LoadFromAssemblyPath(Path.GetFullPath(p)):null;
Tests.Run();
