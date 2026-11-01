using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using PatchSupport;

namespace VictoryBreedsArroganceSyncFix;

[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.victory-breeds-arrogance-sync-fix.v1";
    [ThreadStatic] static Player? explicitRecipient;
    static MethodInfo originalDamage = null!;
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
        var card = RuntimeGuard.Type("YuWanCard.Content", "YuWanCard.Cards.VictoryBreedsArrogance");
        originalDamage = RuntimeGuard.Method(RuntimeGuard.Type("YuWanCard.Content", "YuWanCard.Utils.IntentUtils"), "GetAttackIntentDamage");
        var body = RuntimeGuard.Method(card, "OnPlay").GetCustomAttribute<AsyncStateMachineAttribute>()!.StateMachineType;
        harmony.Patch(RuntimeGuard.Method(body, "MoveNext"), transpiler: new HarmonyMethod(typeof(Entry), nameof(Rewrite)));
        harmony.Patch(RuntimeGuard.Method(typeof(AttackIntent), nameof(AttackIntent.GetSingleDamage)),
            prefix: new HarmonyMethod(typeof(Entry), nameof(SingleDamage)));
    }
    public static IEnumerable<CodeInstruction> Rewrite(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
    {
        var self = AccessTools.Field(__originalMethod.DeclaringType, "<>4__this")
            ?? throw new MissingFieldException("Missing card state-machine owner.");
        int count = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(originalDamage))
            {
                var load = new CodeInstruction(OpCodes.Ldarg_0);
                load.labels.AddRange(instruction.labels); instruction.labels.Clear();
                load.blocks.AddRange(instruction.blocks); instruction.blocks.Clear();
                yield return load;
                yield return new CodeInstruction(OpCodes.Ldfld, self);
                yield return new CodeInstruction(OpCodes.Call, RuntimeGuard.Method(typeof(Entry), nameof(DamageForCard)));
                count++;
            }
            else yield return instruction;
        }
        if (count != 1) throw new InvalidOperationException($"Expected one damage branch, found {count}.");
    }
    public static int DamageForCard(Creature enemy, CardModel card) => DamageForPlayer(enemy, card.Owner);
    public static int DamageForPlayer(Creature enemy, Player player)
    {
        var previous = explicitRecipient;
        explicitRecipient = player;
        try { return (int)originalDamage.Invoke(null, [enemy])!; }
        finally { explicitRecipient = previous; }
    }
    static bool SingleDamage(AttackIntent __instance, Creature owner, ref int __result)
    {
        if (explicitRecipient is not { } player) return true;
        decimal amount = __instance.DamageCalc!();
        amount = Hook.ModifyDamage(player.RunState, player.Creature.CombatState, player.Creature, owner,
            amount, ValueProp.Move, null, null, ModifyDamageHookType.All, CardPreviewMode.None, out _);
        __result = Math.Max(0, (int)amount);
        return false;
    }
}
