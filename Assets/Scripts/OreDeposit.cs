using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreDeposit : Entity
{
    public OreDepositData data;

    private void Start()
    {
        data = Resources.Load("ScriptableObjects/OreDepositScriptableObject") as OreDepositData;
        currentHealth = data.health;
    }

    public override float takeDamage(int damage)
    {
        return base.takeDamage(damage);
    }
}
