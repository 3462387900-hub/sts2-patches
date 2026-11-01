using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

partial class Program
{
    static AbstractModel[] turnModels=[];
    static readonly ConditionalWeakTable<HookPlayerChoiceContext,Task> turnTasks=new();
    static readonly HashSet<HookPlayerChoiceContext> pausedContexts=[];
    static async Task TestTurn()
    {
        Patch(Method(typeof(Hook),"IterateCombatHookListeners"),nameof(TurnListeners));
        Patch(Method(typeof(HookPlayerChoiceContext),"AssignTaskAndWaitForPauseOrCompletion"),nameof(TurnAssign));
        Patch(Method(typeof(HookPlayerChoiceContext),"WaitForCompletion"),nameof(TurnCompletion));
        LocalContext.NetId=1;
        int host=await TurnOrder(false,false),client=await TurnOrder(true,false);
        Check(host==2&&client==3,"unpatched real dispatcher reproduces bomb 2/3 at identical choice checkpoint");
        TurnEndOrderFix.Entry.Apply(new Harmony("test.turn.production"));
        host=await TurnOrder(false,true);client=await TurnOrder(true,true);
        Check(host==3&&client==3,"patched real dispatcher produces matching bomb checkpoint");
        var s=new TurnState();s.Slow.SetResult();
        var planning=Make<Planning>();planning.S=s;var block=Make<EarlyBlock>();block.S=s;
        turnModels=[planning,block];
        var loop=Hook.BeforeSideTurnEnd(null!,CombatSide.Player,[]);
        s.Choice.SetResult();await s.Checkpoint.Task;
        Check(s.Block==0,"same-stage block does not run before choice checkpoint");
        Check(!loop.IsCompleted,"wait includes game-action completion after hook task");
        s.ActionCompleted.SetResult();await loop.WaitAsync(TimeSpan.FromSeconds(3));
        Check(s.Block==3,"same-stage block runs once after complete action");
        foreach(bool cancel in new[]{false,true})
        {
            s=new TurnState();planning=Make<Planning>();planning.S=s;block=Make<EarlyBlock>();block.S=s;turnModels=[planning,block];
            loop=Hook.BeforeSideTurnEnd(null!,CombatSide.Player,[]);
            if(cancel)s.Choice.SetCanceled();else s.Choice.SetException(new InvalidOperationException("fixture"));
            bool failed=false;try{await loop.WaitAsync(TimeSpan.FromSeconds(3));}catch(Exception e) when(e is not TimeoutException){failed=true;}
            Check(failed&&s.Block==0,$"{(cancel?"cancellation":"fault")} stops later listeners without hanging");
        }
        LocalContext.NetId=null;turnModels=[block];await Hook.BeforeSideTurnEnd(null!,CombatSide.Player,[]);
        Check(s.Block==0,"absent local context preserves original no-op");
    }
    static async Task<int> TurnOrder(bool slow,bool patched)
    {
        var s=new TurnState();var planning=Make<Planning>();planning.S=s;var delay=Make<NormalDelay>();delay.S=s;var bomb=Make<Bomb>();bomb.S=s;
        turnModels=[planning,delay,bomb];if(!slow)s.Slow.SetResult();
        var loop=Hook.BeforeSideTurnEnd(null!,CombatSide.Player,[]);
        Check(s.Bomb==(patched||slow?3:2),"expected pre-choice bomb count");
        s.Choice.SetResult();await s.Checkpoint.Task;int snapshot=s.Checkpoint.Task.Result;
        s.ActionCompleted.SetResult();if(slow)s.Slow.SetResult();
        await loop.WaitAsync(TimeSpan.FromSeconds(3));Check(s.Bomb==2,"bomb decremented exactly once");return snapshot;
    }
    static bool TurnListeners(ref IEnumerable<AbstractModel> __result){__result=turnModels;return false;}
    static bool TurnAssign(HookPlayerChoiceContext __instance,Task task,ref Task<bool> __result)
    {turnTasks.Add(__instance,task);if(__instance.Source is Planning&&!task.IsCompleted)pausedContexts.Add(__instance);__result=__instance.Source is Planning&&!task.IsCompleted?Task.FromResult(false):TurnFinish(task);return false;}
    static async Task<bool> TurnFinish(Task t){await t;return true;}
    static bool TurnCompletion(HookPlayerChoiceContext __instance,ref Task __result)
    {__result=TurnComplete(__instance);return false;}
    static async Task TurnComplete(HookPlayerChoiceContext context)
    {await turnTasks.GetValue(context,_=>throw new Exception());if(pausedContexts.Contains(context)&&context.Source is Planning p)await p.S.ActionCompleted.Task;}
    class TurnState{public int Bomb=3,Block;public TaskCompletionSource Choice=new(),Slow=new(),ActionCompleted=new();public TaskCompletionSource<int> Checkpoint=new();}
    abstract class TurnModel:AbstractModel{public TurnState S=null!;public override bool ShouldReceiveCombatHooks=>true;}
    class Planning:TurnModel{public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext c,CombatSide s,IEnumerable<Creature> p){await S.Choice.Task;S.Checkpoint.SetResult(S.Bomb);}}
    class NormalDelay:TurnModel{public override Task BeforeSideTurnEnd(PlayerChoiceContext c,CombatSide s,IEnumerable<Creature> p)=>S.Slow.Task;}
    class Bomb:TurnModel{public override Task BeforeSideTurnEnd(PlayerChoiceContext c,CombatSide s,IEnumerable<Creature> p){S.Bomb--;return Task.CompletedTask;}}
    class EarlyBlock:TurnModel{public override Task BeforeSideTurnEndEarly(PlayerChoiceContext c,CombatSide s,IEnumerable<Creature> p){S.Block+=3;return Task.CompletedTask;}}
}

