using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

partial class Program
{
    static readonly Harmony Fixtures = new("tests.six-fixes.fixtures");
    static int checks;
}
class Bootstrap {
    static string refs = "";
    static void Main(string[] args)
    {
        refs = Path.GetFullPath(args[0]);
        AssemblyLoadContext.Default.Resolving += (_, name) => {
            var path = Path.Combine(refs, name.Name + ".dll");
            return File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : null;
        };
        Program.Run(args[1]).GetAwaiter().GetResult();
    }
}
partial class Program {
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static async Task Run(string suite)
    {
        switch (suite)
        {
            case "guards": TestGuards(); break;
            case "turn": await TestTurn(); break;
            case "victory": await TestVictory(); break;
            case "elbow": await TestElbow(); break;
            case "nymph": TestNymph(); break;
            case "snapshot": TestSnapshot(); break;
            case "neow": TestNeow(); break;
            default: throw new ArgumentException(suite);
        }
        Console.WriteLine($"PASS {suite}: {checks} checks. Actual production patch methods; engine/UI/network boundaries isolated. No live multiplayer claim.");
    }
    static void Patch(MethodBase m, string name) => Fixtures.Patch(m, prefix: new HarmonyMethod(typeof(Program), name));
    static MethodInfo Method(Type t, string n, Type[]? p = null) => AccessTools.Method(t,n,p) ?? throw new MissingMethodException(t.Name,n);
    static void Check(bool ok, string message) { if (!ok) throw new Exception("FAIL " + message); checks++; Console.WriteLine("PASS " + message); }
    static T Make<T>() => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    static object Make(Type type) => RuntimeHelpers.GetUninitializedObject(type);
    static void Set(object obj, Type type, string name, object? value) => (AccessTools.Field(type,name) ?? throw new MissingFieldException(type.FullName,name)).SetValue(obj,value);
    static bool Skip() => false;
    static Type Type(string assembly,string name) => Assembly.Load(assembly).GetType(name,true)!;
    static Player Player(ulong id)
    {
        var p=Make<Player>(); var c=Make<Creature>(); var run=Make<RunState>();
        Set(p,typeof(Player),"<NetId>k__BackingField",id);
        Set(p,typeof(Player),"<Creature>k__BackingField",c);
        Set(p,typeof(Player),"_runState",run);
        Set(c,typeof(Creature),"<Player>k__BackingField",p);
        Set(run,typeof(RunState),"_players",new List<Player>{p});
        return p;
    }
    static CardModel Card(Type type, Player? owner=null)
    {
        var c=(CardModel)Activator.CreateInstance(type)!;
        var map=(Dictionary<ModelId,AbstractModel>)AccessTools.Field(typeof(ModelDb),"_contentById").GetValue(null)!;
        map[c.Id]=c;
        if(owner==null) return c;
        var copy=c.ToMutable(); Set(copy,typeof(CardModel),"_owner",owner); return copy;
    }
}

