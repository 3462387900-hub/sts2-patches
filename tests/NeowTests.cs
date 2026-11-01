using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Events;

partial class Program
{
    static EventOption[] neowBase=[];
    static EventOption Option(string key)=>new(null!,()=>Task.CompletedTask,new LocString("events",key),new LocString("events",key),key,[]);
    static void TestNeow()
    {
        Patch(Method(typeof(EventOption),"AddLocVars"),nameof(Skip));
        var yui=new[]{"BANK","SPRAY","DOLL","YUIEXTRA-PLASTICINE"}.Select(Option).ToArray();
        var rain=Option("YUWANCARD-RAIN_DARK");
        var vanilla=new[]{Option("VANILLA_A"),Option("VANILLA_B")};
        var a=vanilla.Concat([rain]).Concat(yui).ToArray();var b=vanilla.Concat(yui).Concat([rain]).ToArray();
        Check(a[6].TextKey=="YUIEXTRA-PLASTICINE"&&b[6].TextKey=="YUWANCARD-RAIN_DARK","same RNG index maps to Plasticine/RainDark when append order differs");
        var getter=AccessTools.PropertyGetter(typeof(Neow),"PositiveOptions");
        Patch(getter,nameof(NeowGetter));
        NeowOptionOrderFix.Entry.Apply(new Harmony("test.neow.production"));
        var neow=Make<Neow>();
        neowBase=a;var first=((IEnumerable<EventOption>)getter.Invoke(neow,null)!).ToArray();
        neowBase=b;var second=((IEnumerable<EventOption>)getter.Invoke(neow,null)!).ToArray();
        Check(first.Select(x=>x.TextKey).SequenceEqual(second.Select(x=>x.TextKey)),"real patched candidate getter has equal order before RNG");
        Check(first.Length==a.Length&&first.All(a.Contains),"keeps every original option instance and callback");
        Check(Harmony.GetPatchInfo(getter)!.Postfixes.Last().priority==Priority.Last,"sort runs at final postfix priority");
        foreach(var culture in new[]{"zh-CN","en-US","tr-TR"})
        {System.Globalization.CultureInfo.CurrentCulture=new(culture);Check(NeowOptionOrderFix.Entry.Order(a).Select(x=>x.TextKey).SequenceEqual(first.Select(x=>x.TextKey)),"ordinal candidate order independent of "+culture);}
    }
    static bool NeowGetter(ref IEnumerable<EventOption> __result){__result=neowBase;return false;}
}

