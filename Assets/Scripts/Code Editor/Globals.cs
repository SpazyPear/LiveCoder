using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Reflection;

class GlobalManager
{

    public static Dictionary<System.Type, System.Type> proxyMappings = new Dictionary<System.Type, System.Type>();
    public static Dictionary<string, MethodInfo> globalFunctionMappings = new Dictionary<string, MethodInfo>();


    public Vector2Int vec2(int x, int y)
    {
        return new Vector2Int(x, y);

    }


    public void RegisterProxy<T, U>(System.Func<U, T> creator) where T : class where U : class
    {
        UserData.RegisterProxyType<T, U>(r => creator(r));


        if (!GlobalManager.proxyMappings.ContainsKey(typeof(U)))
            GlobalManager.proxyMappings.Add(typeof(U), typeof(T));
    }

    public void RegisterGlobalFunction<T> (Script script, string functionName, T value, MethodInfo info)
    {
        script.Globals[functionName] = (T)value;

        if (!GlobalManager.globalFunctionMappings.ContainsKey(functionName))
            GlobalManager.globalFunctionMappings.Add(functionName, info);
    }

    private void SetupPlayerHandler(Script script)
    {

        RegisterProxy<CharacterHandlerProxy, Character>(r => new CharacterHandlerProxy(r));
        RegisterProxy<GiantHandlerProxy, Giant>(r => new GiantHandlerProxy(r));
        RegisterProxy<HealerHandlerProxy, Healer>(r => new HealerHandlerProxy(r));

        RegisterProxy<TurretProxy, Turret>(r => new TurretProxy(r));
        RegisterProxy<SoldierProxy, Soldier>(r => new SoldierProxy(r));
        RegisterProxy<GiantProxy, Giant>(r => new GiantProxy(r));
        RegisterProxy<HealerProxy, Healer>(r => new HealerProxy(r));

        RegisterProxy<EntityProxy, Entity>(r => new EntityProxy(r));
        RegisterProxy<OreDepositProxy, OreDeposit>(r => new OreDepositProxy(r));
        RegisterProxy<WallProxy, Wall>(r => new WallProxy(r));
        RegisterProxy<CoinStoreProxy, CoinStore>(r => new CoinStoreProxy(r));


        PlayerHandler handler = GameObject.FindObjectOfType<PlayerHandler>();

        RegisterGlobalFunction<System.Func<System.Collections.Generic.List<Character>>>(script, "getEnemies", handler.getEnemies, ((System.Func<System.Collections.Generic.List<Character>>)handler.getEnemies).Method);

        RegisterGlobalFunction <System.Func<System.Collections.Generic.List<OreDeposit>>> (script, "getOreDeposits", handler.getOreDeposits, ((System.Func<System.Collections.Generic.List<OreDeposit>>)handler.getOreDeposits).Method);

        RegisterGlobalFunction<System.Func<Entity>>(script, "getEnemyTower", handler.getEnemyTower, ((System.Func<Entity>)handler.getEnemyTower).Method);

        RegisterGlobalFunction<System.Func<Character, string, Entity>>(script, "findClosest", handler.findClosestEntityOfType, ((System.Func<Character, string, Entity>)handler.findClosestEntityOfType).Method);

        // Get Enemy Reference -- only for passing into other methods (doesnt give access to alot)
        /*script.Globals["getEnemies"] = (System.Func<System.Collections.Generic.List<Character>>)handler.getEnemies;
        script.Globals["getOreDeposits"] = (System.Func<System.Collections.Generic.List<OreDeposit>>)handler.getOreDeposits;
        script.Globals["getEnemyTower"] = (System.Func<Entity>)handler.getEnemyTower;
        script.Globals["findClosestEntityOfType"] = (System.Func<Character, string, Entity>)handler.findClosestEntityOfType;
*/

    }

    //current.MoveToEntity(current.findClosestEntityOfType(current, "Wall"))
      //current.Attack(current.findClosestEntityOfType(current, "Wall"))

    public void DebugLog(DynValue debug)
    {
        if (debug.Type == DataType.Table)
        {
            Debug.Log("Table values");
            foreach (DynValue val in debug.Table.Values)
            {
                Debug.Log(val.ToDebugPrintString());
            }
        }
        else
        {
            Debug.Log(debug.ToString());
        }
    }

    public int len (DynValue val)
    {
        return val.Table.Length;
    }

    public int dist (Vector2Int a, Vector2Int b)
    {
        return Mathf.RoundToInt(Mathf.Floor(Vector2Int.Distance(a, b)));
    }

    public void printVec2(Vector2Int vec)
    {
        Debug.Log($"Vec2: {vec.x},{vec.y}");
    }

    private void SetupPathfinding(Script script)
    {
        Pathfinder pathfinder = GameObject.FindObjectOfType<Pathfinder>();
        script.Globals["FindPath"] = (System.Func<Vector2Int, Vector2Int, System.Collections.Generic.List<Vector2Int>>)pathfinder.FindPath;
    }

    private void SetupTypes(Script script)
    {
        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector2Int),
            dynVal =>
            {
                Table table = dynVal.Table;
                int x = (int)((System.Double)table[1]);
                int y = (int)((System.Double)table[2]);
                return new Vector2Int(x, y);
            }
        );
        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector2Int>(
            (script, vector) =>
            {
                DynValue x = DynValue.NewNumber((int)vector.x);
                DynValue y = DynValue.NewNumber((int)vector.y);
                DynValue dynVal = DynValue.NewTable(script, new DynValue[] { x, y });
                return dynVal;
            }
        );

/*
        script.Globals["vec2"] = (System.Func<int, int, Vector2Int>)vec2;
*/
        RegisterGlobalFunction<System.Func<int, int, Vector2Int>>(script, "vec2",vec2, ((System.Func<int, int, Vector2Int>)vec2).Method);
/*
        script.Globals["dist"] = (System.Func<Vector2Int, Vector2Int, int>)dist;
*/
        RegisterGlobalFunction<System.Func<Vector2Int, Vector2Int, int>>(script, "dist", dist, ((System.Func<Vector2Int, Vector2Int, int>)dist).Method);

    }

    public void OnScriptStart(Script script, Entity target = null)
    {

        RegisterGlobalFunction<System.Action<DynValue>>(script, "print", DebugLog, ((System.Action<DynValue>)DebugLog).Method);

        RegisterGlobalFunction<System.Func<DynValue, int>>(script, "len", len, ((System.Func<DynValue, int>)len).Method);

        RegisterGlobalFunction<System.Action<Vector2Int>>(script, "printVec2", printVec2, ((System.Action<Vector2Int>)printVec2).Method);


        SetupTypes(script);

        SetupPlayerHandler(script);

        SetupPathfinding(script);


        script.Globals["current"] = target;
    }

    public void OnScriptPreStep (Script script)
    {

    }

}