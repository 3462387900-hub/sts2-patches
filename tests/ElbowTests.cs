using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

partial class Program
{
    static AttackCommand fakeAttack=null!;
    static List<(string Power,Creature Target,decimal Amount)> applied=[];
    static int attacks;
    static async Task TestElbow()
    {
        var type=Type("KakarotMod","KakarotMod.KakarotCode.Cards.Common.KakarotElbowStrike");
        var card=Card(type,Player(1));
        Patch(Method(typeof(CommonActions),"CardAttack",[typeof(CardModel),typeof(CardPlay),typeof(int),typeof(string),typeof(string),typeof(string)]),nameof(CardAttackFixture));
        Patch(Method(typeof(AttackCommand),"Execute"),nameof(ExecuteAttackFixture));
        var apply=typeof(PowerCmd).GetMethods().Single(m=>m.Name=="Apply"&&!m.IsGenericMethod);
        Patch(apply,nameof(ApplyPowerFixture));
        Patch(AccessTools.PropertyGetter(typeof(Creature),"CanReceivePowers"),nameof(CanReceiveFixture));
        foreach(var power in new PowerModel[]{new VulnerablePower(),new WeakPower()})
            ((Dictionary<ModelId,AbstractModel>)AccessTools.Field(typeof(ModelDb),"_contentById").GetValue(null)!)[power.Id]=power;
        var a=Make<Creature>();var b=Make<Creature>();
        Set(a,typeof(Creature),"_powers",new List<PowerModel>());Set(b,typeof(Creature),"_powers",new List<PowerModel>());        fakeAttack=new AttackCommand(6);fakeAttack.AddResultsInternal([new DamageResult(a,ValueProp.Move)]);
        var play=new CardPlay{Card=card,Player=card.Owner,Target=null,ResultPile=PileType.Discard,Resources=default,IsAutoPlay=false,PlayIndex=0,PlayCount=1};var onPlay=Method(type,"OnPlay");
        bool failed=false;
        try{await (Task)onPlay.Invoke(card,[new ThrowingPlayerChoiceContext(),play])!;}catch(NullReferenceException){failed=true;}
        Check(failed,"original elbow uses null CardPlay.Target after an actual hit");
        KakarotElbowTargetFix.Entry.Apply(new Harmony("test.elbow.production"));
        foreach(int count in new[]{0,1,2})
        {
            applied.Clear();attacks=0;fakeAttack=new AttackCommand(6);
            if(count>=1)fakeAttack.AddResultsInternal([new DamageResult(a,ValueProp.Move),new DamageResult(a,ValueProp.Move)]);
            if(count>=2)fakeAttack.AddResultsInternal([new DamageResult(b,ValueProp.Move)]);
            await (Task)onPlay.Invoke(card,[new ThrowingPlayerChoiceContext(),play])!;
            Check(attacks==1,"attack is executed once without rerolling RNG");
            Check(applied.Count==count*2,"empty/random/area targeting applies two effects once per unique receiver");
            Check(applied.All(x=>x.Target==a||x.Target==b),"effects target only actual receivers");
        }
        play=new CardPlay{Card=card,Player=card.Owner,Target=a,ResultPile=PileType.Discard,Resources=default,IsAutoPlay=false,PlayIndex=0,PlayCount=1};applied.Clear();attacks=0;
        await (Task)onPlay.Invoke(card,[new ThrowingPlayerChoiceContext(),play])!;
        Check(attacks==1&&applied.Count==2&&applied.All(x=>x.Target==a),"explicit target retains original card path");
    }
    static bool CanReceiveFixture(ref bool __result){__result=true;return false;}
    static bool CardAttackFixture(ref AttackCommand __result){__result=fakeAttack;return false;}
    static bool ExecuteAttackFixture(ref Task<AttackCommand> __result){attacks++;__result=Task.FromResult(fakeAttack);return false;}
    static bool ApplyPowerFixture(PowerModel power,Creature target,decimal amount,ref Task __result)
    {
        if(target==null)return true; // real PowerCmd reproduces its null-target failure
        applied.Add((power.GetType().Name,target,amount));__result=Task.CompletedTask;return false;
    }
}



