using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using PatchSupport;

namespace TurnEndOrderFix;

[ModInitializer(nameof(Initialize))]
public static class Entry
{
    const string Id = "local.turn-end-order-fix.v1";
    static readonly MethodInfo Listeners = RuntimeGuard.Method(typeof(Hook), "IterateCombatHookListeners");
    static bool initialized;
    public static void Initialize()
    {
        if (initialized) return;
        RuntimeGuard.Verify(typeof(Entry).Assembly);
        RuntimeGuard.Install(Id, Apply);
        initialized = true;
    }
    public static void Apply(Harmony harmony) => harmony.Patch(
        RuntimeGuard.Method(typeof(Hook), nameof(Hook.BeforeSideTurnEnd)),
        prefix: new HarmonyMethod(typeof(Entry), nameof(Before)));

    static bool Before(ICombatState combatState, CombatSide side, IEnumerable<Creature> participants, ref Task __result)
    {
        __result = Dispatch(combatState, side, participants);
        return false;
    }

    public static async Task Dispatch(ICombatState combatState, CombatSide side, IEnumerable<Creature> participants)
    {
        var netId = LocalContext.NetId;
        if (!netId.HasValue) return;
        for (int stage = 0; stage < 3; stage++)
        {
            // Re-enumerate at each original phase; earlier effects may alter listeners.
            var listeners = (IEnumerable<AbstractModel>)Listeners.Invoke(null, [combatState])!;
            foreach (var model in listeners)
            {
                var context = new HookPlayerChoiceContext(model, netId.Value, combatState, GameActionType.Combat);
                Task task = stage switch
                {
                    0 => model.BeforeSideTurnEndVeryEarly(context, side, participants),
                    1 => model.BeforeSideTurnEndEarly(context, side, participants),
                    _ => model.BeforeSideTurnEnd(context, side, participants)
                };
                await context.AssignTaskAndWaitForPauseOrCompletion(task);
                // Includes GenericHookGameAction.CompletionTask: its checksum must precede
                // any subsequent listener's state mutation, even within the same phase.
                await context.WaitForCompletion();
            }
        }
    }
}
