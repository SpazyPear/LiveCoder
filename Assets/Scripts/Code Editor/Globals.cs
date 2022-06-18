using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;

class GlobalManager
{

    public static Dictionary<System.Type, System.Type> proxyMappings = new Dictionary<System.Type, System.Type>();

    public Vector2Int vec2(int x, int y)
    {
        return new Vector2Int(x, y);

    }

    public void RegisterProxy <T, U> (System.Func<U,T> creator) where T : class where U : class
    {
        UserData.RegisterProxyType<T, U>(r => creator(r));
        GlobalManager.proxyMappings.Add(typeof(U), typeof(T));
    }

    private void SetupPlayerHandler(Script script)
    {

        RegisterProxy<CharacterHandlerProxy, Character>(r => new CharacterHandlerProxy(r));
        RegisterProxy<SoldierProxy, Soldier>(r => new SoldierProxy(r));

        PlayerHandler handler = GameObject.FindObjectOfType<PlayerHandler>();

        script.Globals["selected"] = handler.selectedPlayer;

        // Get Enemy Reference -- only for passing into other methods (doesnt give access to alot)
        script.Globals["getEnemies"] = (System.Func<System.Collections.Generic.List<Character>>)handler.getEnemies;

    }

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

    public void OnScriptStart(Script script, Character target = null)
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