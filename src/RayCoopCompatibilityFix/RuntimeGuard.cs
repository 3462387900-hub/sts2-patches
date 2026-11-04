using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using HarmonyLib;

namespace PatchSupport;

internal static class RuntimeGuard
{
    public static void Verify(Assembly patch)
    {
        using var stream = patch.GetManifestResourceStream("PatchSupport.Baseline")
            ?? throw new InvalidOperationException("Missing embedded compatibility baseline.");
        using var document = JsonDocument.Parse(stream);
        VerifyDocument(patch, document);
    }
    internal static void VerifyDocument(Assembly patch, JsonDocument document)
    {
        foreach (var item in document.RootElement.GetProperty("assemblies").EnumerateArray())
        {
            var name = item.GetProperty("name").GetString()!;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == name)
                ?? Assembly.Load(new AssemblyName(name));
            using var stream = File.OpenRead(assembly.Location);
            var hash = Convert.ToHexString(SHA256.HashData(stream));
            if (!hash.Equals(item.GetProperty("sha256").GetString(), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"[{patch.GetName().Name}] Unsupported {name} build; no patches installed.");
        }
    }

    public static Type Type(string assembly, string name) => Assembly.Load(assembly).GetType(name, true)!;
    public static MethodInfo Method(Type type, string name, Type[]? args = null) =>
        AccessTools.Method(type, name, args) ?? throw new MissingMethodException(type.FullName, name);

    public static void Install(string id, Action<Harmony> apply)
    {
        var harmony = new Harmony(id);
        try { apply(harmony); }
        catch { harmony.UnpatchAll(id); throw; }
        Console.WriteLine($"[{id}] 1.0.0 initialized; guarded patches active.");
    }
}
