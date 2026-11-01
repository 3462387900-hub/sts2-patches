using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

partial class Program
{
    static Player view=null!,caster=null!;
    static AttackIntent[] intents=[];
    static bool damageFault;
    static Task TestVictory()
    {
        caster=Player(1);var other=Player(2);var enemy=Make<Creature>();
        Patch(Method(typeof(LocalContext),"GetMe",[typeof(ICombatState)]),nameof(LocalView));
        Patch(Method(typeof(Hook),"ModifyDamage"),nameof(ModifyIntentDamage));
        var util=Type("YuWanCard.Content","YuWanCard.Utils.IntentUtils");
        Patch(Method(util,"GetAttackIntentDamage"),nameof(IntentSum));
        intents=[new SingleAttackIntent(60)];view=caster;
        int host=intents[0].GetTotalDamage([],enemy);view=other;int client=intents[0].GetTotalDamage([],enemy);
        Check(host==60&&client==20,"original intent preview depends on local player's defenses");
        VictoryBreedsArroganceSyncFix.Entry.Apply(new Harmony("test.victory.production"));
        foreach(var list in new[]{new AttackIntent[]{new SingleAttackIntent(60)},new AttackIntent[]{new MultiAttackIntent(10,3)},new AttackIntent[]{new SingleAttackIntent(2),new MultiAttackIntent(6,2)}})
        {
            intents=list;view=caster;host=VictoryBreedsArroganceSyncFix.Entry.DamageForPlayer(enemy,caster);
            view=other;client=VictoryBreedsArroganceSyncFix.Entry.DamageForPlayer(enemy,caster);
            Check(host==client,"single/multiple/mixed intents use explicit caster for both peers");
            Check((host>36)==(client>36),"strict HP threshold branch matches");
        }
        view=other;Check(new SingleAttackIntent(60).GetTotalDamage([],enemy)==20,"ordinary UI preview remains local after scoped calculation");
        damageFault=true;try{VictoryBreedsArroganceSyncFix.Entry.DamageForPlayer(enemy,caster);}catch(TargetInvocationException){}
        damageFault=false;Check(new SingleAttackIntent(60).GetTotalDamage([],enemy)==20,"exception restores explicit-player scope");
        var card=Type("YuWanCard.Content","YuWanCard.Cards.VictoryBreedsArrogance");
        var body=Method(card,"OnPlay").GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>()!.StateMachineType;
        Check(Harmony.GetPatchInfo(Method(body,"MoveNext"))!.Transpilers.Count>0,"production patch installed into real card async body");
        return Task.CompletedTask;
    }
    static bool LocalView(ref Player __result){__result=view;return false;}
    static bool ModifyIntentDamage(Creature? target,decimal damage,ref decimal __result,ref IEnumerable<AbstractModel> modifiers)
    {if(damageFault)throw new InvalidOperationException("fixture");modifiers=[];__result=ReferenceEquals(target,caster.Creature)?damage:damage/3;return false;}
    static bool IntentSum(Creature creature,ref int __result){__result=intents.Sum(i=>i.GetTotalDamage([],creature));return false;}
}

