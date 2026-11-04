using System.Reflection;
using System.Reflection.Emit;
using MegaCrit.Sts2.Core.Models;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using RayCoopCompatibilityFix;
public static class Tests {
 static bool ended; static int powers;
 static bool Ending(ref bool __result){__result=ended;return false;}
 static bool Defer(ref bool __result){__result=true;return false;}
 static bool Power(ref Task<DrawCardsNextTurnPower?> __result){powers++;__result=Task.FromResult<DrawCardsNextTurnPower?>(null);return false;}
 static Task<DrawCardsNextTurnPower?> Capture(Creature c,decimal a,Creature? p,CardModel? s,bool silent){powers++;return Task.FromResult<DrawCardsNextTurnPower?>(null);}
 static IEnumerable<CodeInstruction> CaptureApply(IEnumerable<CodeInstruction> code){foreach(var i in code){if(i.operand is MethodInfo m && m.DeclaringType?.FullName=="HextechRunes.HextechPowerCmdCompat" && m.Name=="Apply")yield return new CodeInstruction(OpCodes.Call,AccessTools.Method(typeof(Tests),nameof(Capture)));else yield return i;}}
 public static void Run() {
  var a=Assembly.Load("HextechRunes");var t=a.GetType("HextechRunes.SpinToWinEnemyHex",true)!;
  var instance=RuntimeHelpers.GetUninitializedObject(t);var method=AccessTools.Method(t,"AfterPreventingDraw");
  var fixtures=new Harmony("tests.ray.fixtures");
  fixtures.Patch(AccessTools.PropertyGetter(typeof(CombatManager),"IsOverOrEnding"),prefix:new HarmonyMethod(typeof(Tests),nameof(Ending)));
  fixtures.Patch(AccessTools.Method(t,"ShouldDeferDraw"),prefix:new HarmonyMethod(typeof(Tests),nameof(Defer)));
  var apply=a.GetType("HextechRunes.HextechPowerCmdCompat",true)!.GetMethods().Single(m=>m.Name=="Apply"&&m.IsGenericMethod&&m.GetParameters().Length==5&&m.GetParameters()[0].ParameterType==typeof(Creature)).MakeGenericMethod(typeof(DrawCardsNextTurnPower));
  var stateMachine=method.GetCustomAttribute<AsyncStateMachineAttribute>()!.StateMachineType;
  fixtures.Patch(AccessTools.Method(stateMachine,"MoveNext"),transpiler:new HarmonyMethod(typeof(Tests),nameof(CaptureApply)));
  var player=(Player)RuntimeHelpers.GetUninitializedObject(typeof(Player));
  var creature=(Creature)RuntimeHelpers.GetUninitializedObject(typeof(Creature));
  AccessTools.Field(typeof(Player),"<Creature>k__BackingField").SetValue(player,creature);
  bool Invoke()=>((Task<bool>)method.Invoke(instance,[null,player,false,1])!).GetAwaiter().GetResult();
  ended=true; powers=0;
  var original=Invoke(); if(!original||powers!=1)throw new Exception($"Original reproduction failed result={original} powers={powers}");
  Console.WriteLine("PASS original Hextech enters the power command after combat ended (engine boundaries isolated)");
  Entry.Initialize();
  Console.WriteLine("PASS all runtime hash guards and actual Harmony patch installation");
  ended=true;powers=0;
  if(Invoke()||powers!=0)throw new Exception("Ending guard failed");
  ended=false;
  if(!Invoke()||powers!=1)throw new Exception("Normal combat draw changed");
  Console.WriteLine("PASS ended combat blocks power; ongoing combat retains original deferred draw");
  var creatures=Enumerable.Range(0,3).Select(_=>(Creature)RuntimeHelpers.GetUninitializedObject(typeof(Creature))).ToArray();
  var aRng=new Rng(20261105);var bRng=new Rng(20261105);
  for(int i=0;i<100;i++){
   var x=(CardPlay)RuntimeHelpers.GetUninitializedObject(typeof(CardPlay));var y=(CardPlay)RuntimeHelpers.GetUninitializedObject(typeof(CardPlay));
   var u=Entry.ChooseTarget(x,aRng,creatures);var v=Entry.ChooseTarget(y,bRng,creatures);
   if(!ReferenceEquals(u,v)||!ReferenceEquals(x.Target,u)||!ReferenceEquals(y.Target,v))throw new Exception("Target drift");
  }
  Console.WriteLine("PASS production target selection: 100 matching host/client choices and original CardPlay identity retained");
  var empty=(CardPlay)RuntimeHelpers.GetUninitializedObject(typeof(CardPlay));
  if(Entry.ChooseTarget(empty,aRng,[])!=null||empty.Target!=null)throw new Exception("Empty target failure");
  if(aRng.NextInt(10000)!=bRng.NextInt(10000))throw new Exception("Empty selection advanced RNG");
  Console.WriteLine("PASS no-enemy selection consumes no RNG");
  var ray=Assembly.Load("Ray").GetType("Ray.Scripts.Cards.Common.SandView",true)!;
  var card=(MegaCrit.Sts2.Core.Models.CardModel)RuntimeHelpers.GetUninitializedObject(ray);
  var explicitPlay=(CardPlay)RuntimeHelpers.GetUninitializedObject(typeof(CardPlay));Entry.SetTarget(explicitPlay,creatures[0]);Task result=Task.CompletedTask;
  if(!Entry.ResolveRayTarget(card,explicitPlay,ref result)||explicitPlay.Target!=creatures[0])throw new Exception("Explicit target changed");
  Console.WriteLine("PASS explicit Ray targets left intact");
 }
}
