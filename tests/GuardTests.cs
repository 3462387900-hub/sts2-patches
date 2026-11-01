using System.Reflection;
using System.Text.Json;
using HarmonyLib;

partial class Program
{
    static void TestGuards()
    {
        var entries=new[]{typeof(TurnEndOrderFix.Entry),typeof(VictoryBreedsArroganceSyncFix.Entry),typeof(KakarotElbowTargetFix.Entry),typeof(NymphKazdelRewardFix.Entry),typeof(Act3SnapshotSaveFix.Entry),typeof(NeowOptionOrderFix.Entry)};
        foreach(var entry in entries)
        {
            var guard=entry.Assembly.GetType("PatchSupport.RuntimeGuard")!;
            var verify=Method(guard,"VerifyDocument");
            using var bad=JsonDocument.Parse("{\"assemblies\":[{\"name\":\"sts2\",\"sha256\":\"BAD\"}]}");
            bool rejected=false;
            try{verify.Invoke(null,[entry.Assembly,bad]);}catch(TargetInvocationException e) when(e.InnerException is InvalidOperationException){rejected=true;}
            Check(rejected,entry.Name+" rejects unknown dependency hash");
            Method(entry,"Initialize").Invoke(null,null);
            int before=Harmony.GetAllPatchedMethods().Count();
            Method(entry,"Initialize").Invoke(null,null);
            Check(before==Harmony.GetAllPatchedMethods().Count(),entry.Assembly.GetName().Name+" initializes with actual baselines and is idempotent");
        }
        var g=entries[0].Assembly.GetType("PatchSupport.RuntimeGuard")!;
        string id="test.guard.partial.failure";
        Action<Harmony> failing=h=>{h.Patch(Method(typeof(Program),nameof(GuardTarget)),prefix:new HarmonyMethod(typeof(Program),nameof(Skip)));throw new InvalidOperationException("fixture");};
        try{Method(g,"Install").Invoke(null,[id,failing]);}catch(TargetInvocationException e) when(e.InnerException is InvalidOperationException){}
        Check(!(Harmony.GetPatchInfo(Method(typeof(Program),nameof(GuardTarget)))?.Owners.Contains(id)??false),"partial installation rolls back all hooks for its owner");
        Check(entries.All(e=>Harmony.GetAllPatchedMethods().Any(m=>Harmony.GetPatchInfo(m)!.Owners.Contains((string)e.GetField("Id",BindingFlags.NonPublic|BindingFlags.Static)!.GetRawConstantValue()!))),"all six production patch owners coexist");
    }
    static void GuardTarget(){}
}
