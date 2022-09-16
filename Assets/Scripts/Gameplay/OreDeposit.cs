using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PythonProxies;

public class OreDeposit : Entity
{
    public OreDepositData data;

    private void Start()
    {
        data = Resources.Load("ScriptableObjects/OreDepositScriptableObject") as OreDepositData;
        currentHealth = data.health;
    }

    public override void die(object sender = null)
    {
       
        //sender.recieveOre(data);
        base.die();
    }

    public override object CreateProxy()
    {
        return new OreDepositProxy(this);
    }
}
