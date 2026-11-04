using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Random;
using PatchSupport;

namespace RayCoopCompatibilityFix;
[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.ray-coop-compatibility-fix.v1";
    static bool initialized;
    static readonly MethodInfo TargetSetter = typeof(CardPlay).GetProperty(nameof(CardPlay.Target))!.SetMethod!;
    public static void Initialize()
    {
        if (initialized) return;
        RuntimeGuard.Verify(typeof(Entry).Assembly);
        RuntimeGuard.Install(Id, Apply);
        initialized = true;
    }
    public static void Apply(Harmony harmony)
    {
        var spin = RuntimeGuard.Type("HextechRunes", "HextechRunes.SpinToWinEnemyHex");
        harmony.Patch(RuntimeGuard.Method(spin, "AfterPreventingDraw"), prefix: new HarmonyMethod(typeof(Entry), nameof(GuardDeferredDraw)));
        var hooks = RuntimeGuard.Type("STS2-RitsuLib.Runtime", "STS2RitsuLib.Cards.CardOnPlayHook");
        harmony.Patch(RuntimeGuard.Method(hooks, "RunCardOnPlayHooks"), prefix: new HarmonyMethod(typeof(Entry), nameof(ResolveRayTarget)));
    }
    // The original Hextech hook can be reached by other mods' draw replacements.
    // Match vanilla DrawInternal: never apply a next-turn draw after combat ends.
    public static bool GuardDeferredDraw(ref Task<bool> __result)
    {
        if (!CombatManager.Instance.IsOverOrEnding) return true;
        __result = Task.FromResult(false);
        Console.WriteLine("[RayCoopCompatibilityFix] Suppressed deferred draw after combat ended.");
        return false;
    }
    public static bool ResolveRayTarget(CardModel card, CardPlay cardPlay, ref Task __result)
    {
        if (card.GetType().Assembly.GetName().Name != "Ray" || cardPlay.Target != null || card.Type != CardType.Attack
            || card.TargetType != TargetType.RandomEnemy
            || !card.Owner.Creature.Powers.Any(p => p.GetType().FullName == "AK_Exusiai.Powers.BelugaRandomTargetPower")) return true;
        var state = card.CombatState ?? card.Owner.Creature.CombatState;
        var targets = state?.HittableEnemies.ToArray() ?? [];
        if (targets.Length == 0)
        {
            __result = Task.CompletedTask;
            return false;
        }
        // Use the same synchronized RNG as vanilla autoplay. Update the original
        // CardPlay so Ritsu secondary-resource metadata and history keep their identity.
        var target = ChooseTarget(cardPlay, card.Owner.RunState.Rng.CombatTargets, targets);
        Console.WriteLine($"[RayCoopCompatibilityFix] Resolved Beluga target: {card.Id} -> {target!.CombatId}");
        return true;
    }
    public static Creature? ChooseTarget(CardPlay play, Rng rng, IEnumerable<Creature> targets)
    {
        var list = targets.ToArray();
        if (list.Length == 0) return null;
        var target = rng.NextItem(list);
        SetTarget(play, target!);
        return target;
    }
    public static void SetTarget(CardPlay play, Creature target) => TargetSetter.Invoke(play, [target]);
}
