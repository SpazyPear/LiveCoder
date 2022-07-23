using UnityEngine;
using MoonSharp.Interpreter;
class GlobalManager
{

    public Vector2Int vec2(int x, int y)
    {
        return new Vector2Int(x, y);

    }

    private void SetupPlayerHandler(Script script)
    {

        UserData.RegisterProxyType<CharacterHandlerProxy, Character>(r => new CharacterHandlerProxy(r));
        UserData.RegisterProxyType<GiantHandlerProxy, Giant>(r => new GiantHandlerProxy(r));
        UserData.RegisterProxyType<HealerHandlerProxy, Healer>(r => new HealerHandlerProxy(r));

        UserData.RegisterProxyType<TurretProxy, Turret>(r => new TurretProxy(r));
        UserData.RegisterProxyType<SoldierProxy, Soldier>(r => new SoldierProxy(r));
        UserData.RegisterProxyType<GiantProxy, Giant>(r => new GiantProxy(r));
        UserData.RegisterProxyType<HealerProxy, Healer>(r => new HealerProxy(r));

        UserData.RegisterProxyType<EntityProxy, Entity>(r => new EntityProxy(r));
        UserData.RegisterProxyType<OreDepositProxy, OreDeposit>(r => new OreDepositProxy(r));
        UserData.RegisterProxyType<WallProxy, Wall>(r => new WallProxy(r));
        UserData.RegisterProxyType<CoinStoreProxy, CoinStore>(r => new CoinStoreProxy(r));


        PlayerHandler handler = GameObject.FindObjectOfType<PlayerHandler>();

        script.Globals["selected"] = handler.selectedPlayer;

        // Get Enemy Reference -- only for passing into other methods (doesnt give access to alot)
        script.Globals["getEnemies"] = (System.Func<System.Collections.Generic.List<Character>>)handler.getEnemies;
        script.Globals["getOreDeposits"] = (System.Func<System.Collections.Generic.List<OreDeposit>>)handler.getOreDeposits;
        script.Globals["getEnemyTower"] = (System.Func<Entity>)handler.getEnemyTower;
        script.Globals["findClosestEntityOfType"] = (System.Func<Character, string, Entity>)handler.findClosestEntityOfType;


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


        script.Globals["vec2"] = (System.Func<int, int, Vector2Int>)vec2;
        script.Globals["dist"] = (System.Func<Vector2Int, Vector2Int, int>)dist;

    }

    public void OnScriptStart(Script script, Entity target = null)
    {

        script.Globals["print"] = (System.Action<DynValue>)DebugLog;
        script.Globals["len"] = (System.Func<DynValue, int>)len;
        script.Globals["printVec2"] = (System.Action<Vector2Int>)printVec2;

        

        SetupTypes(script);

        SetupPlayerHandler(script);

        SetupPathfinding(script);


        script.Globals["current"] = target;
    }

    public void OnScriptPreStep (Script script)
    {

    }

}