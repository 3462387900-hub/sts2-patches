using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using PatchSupport;

namespace KakarotElbowTargetFix;

[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.kakarot-elbow-target-fix.v1";
    static bool initialized;
    public static void Initialize()
    {
        if (initialized) return;
        RuntimeGuard.Verify(typeof(Entry).Assembly);
        RuntimeGuard.Install(Id, Apply);
        initialized = true;
    }
    public static void Apply(Harmony harmony) => harmony.Patch(
        RuntimeGuard.Method(RuntimeGuard.Type("KakarotMod", "KakarotMod.KakarotCode.Cards.Common.KakarotElbowStrike"), "OnPlay"),
        prefix: new HarmonyMethod(typeof(Entry), nameof(Before)));
    static bool Before(CardModel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        // Leave the original, explicitly targeted path byte-for-byte in charge.
        if (cardPlay.Target != null) return true;
        __result = PlayWithoutExplicitTarget(__instance, choiceContext, cardPlay);
        return false;
    }
    public static Creature[] Receivers(AttackCommand attack) => attack.Results.SelectMany(hit => hit)
        .Select(result => result.Receiver).Distinct().ToArray();
    public static async Task PlayWithoutExplicitTarget(CardModel card, PlayerChoiceContext context, CardPlay play)
    {
        var attack = await CommonActions.CardAttack(card, play).Execute(context);
        // Reuse actual hit results. Never reroll random targets or apply to an arbitrary enemy.
        foreach (var target in Receivers(attack))
        {
            await PowerCmd.Apply<VulnerablePower>(context, target, card.DynamicVars.Vulnerable.BaseValue, card.Owner.Creature, card);
            await PowerCmd.Apply<WeakPower>(context, target, card.DynamicVars.Weak.BaseValue, card.Owner.Creature, card);
        }
    }
}

