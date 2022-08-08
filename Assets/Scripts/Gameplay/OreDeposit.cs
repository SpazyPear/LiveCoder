using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDepositProxy : EntityProxy
{
    OreDeposit target;

    [MoonSharpHidden]
    public OreDepositProxy(OreDeposit p) : base (p)
    {
        this.target = p;
    }
}

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
}
