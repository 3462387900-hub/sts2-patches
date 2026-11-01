using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves;
using PatchSupport;

namespace Act3SnapshotSaveFix;

[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.act3-snapshot-save-fix.v1";
    static bool initialized;
    public static void Initialize()
    {
        if (initialized) return;
        RuntimeGuard.Verify(typeof(Entry).Assembly);
        RuntimeGuard.Install(Id, Apply);
        initialized = true;
    }
    public static void Apply(Harmony harmony)
    {
        var type = RuntimeGuard.Type("Act4FinalAscent", "Act4Placeholder.ModSupport");
        foreach (var name in new[] { "PersistAct3Snapshots", "EnsureAct3SnapshotsLoaded" })
            harmony.Patch(RuntimeGuard.Method(type, name), transpiler: new HarmonyMethod(typeof(Entry), nameof(Rewrite)));
    }
    public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> instructions)
    {
        int count = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.operand is MethodInfo m && m.DeclaringType == typeof(SaveManager) && m.IsGenericMethod &&
                m.GetGenericArguments()[0].FullName == "Act4Placeholder.ModSupport+Act3SnapshotStore" && m.Name is "ToJson" or "FromJson")
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = RuntimeGuard.Method(typeof(Entry), m.Name).MakeGenericMethod(m.GetGenericArguments());
                count++;
            }
            yield return instruction;
        }
        if (count != 1) throw new InvalidOperationException($"Expected one snapshot serialization call, found {count}.");
    }
    public static JsonSerializerOptions SnapshotOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializationUtility.Options);
        // Preserve game's converters and generated metadata for nested RunHistory.
        // Reflection fallback exists only in this mod-owned store operation.
        options.TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolver!, new DefaultJsonTypeInfoResolver());
        return options;
    }
    public static string ToJson<T>(T obj) where T : ISaveSchema => JsonSerializer.Serialize(obj, SnapshotOptions());
    public static ReadSaveResult<T> FromJson<T>(string json) where T : ISaveSchema, new()
    {
        var value = JsonSerializer.Deserialize<T>(json, SnapshotOptions()) ?? throw new JsonException("Empty Act 3 snapshot store.");
        if (value.SchemaVersion != 1) throw new JsonException("Unsupported Act 3 snapshot schema.");
        return new ReadSaveResult<T>(value);
    }
}
