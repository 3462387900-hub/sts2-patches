using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Exceptions;

partial class Program
{
    static CardModel[] nymphCards=[];
    static void TestNymph()
    {
        var poolType=Type("CyanrainNymphBeta","CyanrainNymph.CyanrainNymphCode.Character.CyanrainNymphKazdelCardPool");
        var cardType=Type("CyanrainNymphBeta","CyanrainNymph.CyanrainNymphCode.Cards.GougeMemory");
        var canonical=Card(cardType);nymphCards=[canonical];
        Patch(Method(poolType.BaseType!,"GenerateAllCards"),nameof(NymphBaseCards));
        var pool=(CardPoolModel)Make(poolType);
        var original=(CardModel[])Method(poolType,"GenerateAllCards").Invoke(pool,null)!;
        bool failed=false;try{original[0].ToMutable();}catch(MutableModelException){failed=true;}
        Check(failed,"actual Kazdel pool returns mutable template that original ToMutable rejects");
        NymphKazdelRewardFix.Entry.Apply(new Harmony("test.nymph.production"));
        var templates=(CardModel[])Method(poolType,"GenerateAllCards").Invoke(pool,null)!;
        var force=AccessTools.Property(cardType,"ForceKazdelVariant");
        var first=templates[0].ToMutable();var second=templates[0].ToMutable();
        Check((bool)force.GetValue(first)!&&(bool)force.GetValue(second)!,"pool adapter retains forced Kazdel variant on both copies");
        Check(!ReferenceEquals(first,second)&&!ReferenceEquals(first,templates[0]),"reward creation returns fresh independent mutable cards");
        var before=templates[0].DynamicVars["SelfNecrosis"].BaseValue;
        first.DynamicVars["SelfNecrosis"].BaseValue=99;
        Check(second.DynamicVars["SelfNecrosis"].BaseValue==before&&templates[0].DynamicVars["SelfNecrosis"].BaseValue==before,"deep copy keeps later rewards and template unmodified");
        Check(!(bool)force.GetValue(canonical)!&&canonical.IsCanonical,"normal ModelDb canonical remains unchanged");
        failed=false;try{first.ToMutable();}catch(MutableModelException){failed=true;}
        Check(failed,"arbitrary mutable card still fails original invariant");
        Check(canonical.ToMutable().IsMutable,"normal canonical cloning continues to work");
    }
    static bool NymphBaseCards(ref CardModel[] __result){__result=nymphCards;return false;}
}
