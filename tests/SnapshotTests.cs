using System.Collections;
using System.Reflection;
using System.Text.Json;
using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

partial class Program
{
    static void TestSnapshot()
    {
        var support=Type("Act4FinalAscent","Act4Placeholder.ModSupport");
        var storeType=support.GetNestedType("Act3SnapshotStore",BindingFlags.NonPublic)!;
        var entryType=support.GetNestedType("Act3SnapshotEntry",BindingFlags.NonPublic)!;
        var store=Activator.CreateInstance(storeType,true)!;
        var entry=Activator.CreateInstance(entryType,true)!;
        entryType.GetProperty("Key")!.SetValue(entry,"123|seed|10|2");
        var history=new RunHistory{StartTime=123,Seed="中文-seed",Ascension=10};
        entryType.GetProperty("Snapshot")!.SetValue(entry,history);
        ((IList)storeType.GetProperty("Entries")!.GetValue(store)!).Add(entry);
        var original=typeof(SaveManager).GetMethod("ToJson")!.MakeGenericMethod(storeType);
        bool failed=false;try{original.Invoke(null,[store]);}catch(TargetInvocationException e) when(e.InnerException is NotSupportedException){failed=true;}
        Check(failed,"real SaveManager serializer rejects unregistered private snapshot store");
        var before=JsonSerializationUtility.Options.TypeInfoResolver;
        Act3SnapshotSaveFix.Entry.Apply(new Harmony("test.snapshot.production"));
        var write=typeof(Act3SnapshotSaveFix.Entry).GetMethod("ToJson")!.MakeGenericMethod(storeType);
        var read=typeof(Act3SnapshotSaveFix.Entry).GetMethod("FromJson")!.MakeGenericMethod(storeType);
        string json=(string)write.Invoke(null,[store])!;
        Check(json.Contains("run_history")&&json.Contains("schema_version"),"writes original field names and schema");
        var result=read.Invoke(null,[json])!;var loaded=result.GetType().GetProperty("SaveData")!.GetValue(result)!;
        var entries=(IList)storeType.GetProperty("Entries")!.GetValue(loaded)!;
        var roundtrip=(RunHistory)entryType.GetProperty("Snapshot")!.GetValue(entries[0])!;
        Check(roundtrip.StartTime==123&&roundtrip.Seed=="中文-seed"&&roundtrip.Ascension==10,"real nested RunHistory metadata round-trips Unicode and values");
        Check(ReferenceEquals(before,JsonSerializationUtility.Options.TypeInfoResolver),"game-wide serializer is unchanged");
        foreach(var bad in new[]{"{broken",System.Text.RegularExpressions.Regex.Replace(json,"(\"schema_version\"\\s*:\\s*)1","${1}99"),"null"})
        {
            failed=false;try{read.Invoke(null,[bad]);}catch(TargetInvocationException e) when(e.InnerException is JsonException){failed=true;}
            Check(failed,"corrupt, future schema, or null snapshot is rejected");
        }
        Check(Harmony.GetPatchInfo(Method(support,"PersistAct3Snapshots"))!.Transpilers.Count>0&&Harmony.GetPatchInfo(Method(support,"EnsureAct3SnapshotsLoaded"))!.Transpilers.Count>0,"both real persistence and reload paths patched");
    }
}



