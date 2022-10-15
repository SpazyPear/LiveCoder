using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlPoint : PlaceableObject
{
    public int maxHackIntegrity = 100;
    int hackIntegrity;
    int range = 3;
    int damage = 1;
    Unit attackingUnit;

    protected override void Awake()
    {
        base.Awake();
        hackIntegrity = maxHackIntegrity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveHack(PlayerManager player, int damage)
    {
        hackIntegrity -= damage;
        if (hackIntegrity <= 0)
        {
            hackIntegrity = maxHackIntegrity;
            ownerPlayer = player;
        }
    }

    override public void OnStep()
    {
        updateAttackingUnit();
        attackEnemy();
    }

    void updateAttackingUnit()
    {
        if (attackingUnit == null)
        {
            attackingUnit = GridManager.findClosest<Unit>(this, true);
        }
    }

    void attackEnemy()
    {
        if (attackingUnit)
        {
            attackingUnit.takeDamage(0, damage);
            if (!attackingUnit) { attackingUnit = null; }
        }
    }
}
