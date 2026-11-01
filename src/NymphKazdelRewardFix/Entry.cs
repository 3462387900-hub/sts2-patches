using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using PatchSupport;

namespace NymphKazdelRewardFix;

[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.nymph-kazdel-reward-fix.v1";
    static readonly ConditionalWeakTable<CardModel, object> templates = new();
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
        var pool = RuntimeGuard.Type("CyanrainNymphBeta", "CyanrainNymph.CyanrainNymphCode.Character.CyanrainNymphKazdelCardPool");
        harmony.Patch(RuntimeGuard.Method(pool, "GenerateAllCards"), postfix: new HarmonyMethod(typeof(Entry), nameof(RegisterTemplates)));
        harmony.Patch(RuntimeGuard.Method(typeof(CardModel), nameof(CardModel.ToMutable)), prefix: new HarmonyMethod(typeof(Entry), nameof(CloneTemplate)));
    }
    public static void RegisterTemplates(CardModel[] __result)
    {
        foreach (var card in __result)
        {
            if (!card.IsMutable) throw new InvalidOperationException("Unexpected canonical Kazdel template; update this compatibility patch.");
            templates.GetValue(card, _ => new object());
        }
    }
    static bool CloneTemplate(CardModel __instance, ref CardModel __result)
    {
        if (!templates.TryGetValue(__instance, out _)) return true;
        // Only exact instances created by the known Kazdel pool are prototypes.
        // Retain ForceKazdelVariant and deep-copy variables; unrelated mutable cards
        // still reach the game's AssertCanonical and fail normally.
        __result = (CardModel)__instance.MutableClone();
        return false;
    }
}
