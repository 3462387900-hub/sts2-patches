using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.Events;
using PatchSupport;

namespace NeowOptionOrderFix;

[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.neow-option-order-fix.v1";
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
        foreach (var name in new[] { "PositiveOptions", "CurseOptions" })
            harmony.Patch(AccessTools.PropertyGetter(typeof(Neow), name), postfix:
                new HarmonyMethod(typeof(Entry), nameof(SortOptions)) { priority = Priority.Last });
    }
    public static void SortOptions(ref IEnumerable<EventOption> __result) => __result = Order(__result);
    public static EventOption[] Order(IEnumerable<EventOption> options) =>
        options.OrderBy(o => o.TextKey, StringComparer.Ordinal).ThenBy(o => o.Relic?.Id.ToString() ?? "", StringComparer.Ordinal).ToArray();
}
