using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MoonSharp.Interpreter;

public class SoldierProxy : CharacterHandlerProxy
{
    Soldier target;

    [MoonSharpHidden]
    public SoldierProxy(Soldier p) : base(p)
    {
        this.target = p;
    }

    public string soldierSpecific => target.soldierSpecific;


}


public class Soldier : Character
{
    public string soldierSpecific = "specificProperty";
    // Start is called before the first frame update
    void Start()
    {
        initializePlayer("Soldier");  
    }

    // Update is called once per frame
    void Update()
    {
        BaseUpdate();
    }

    public override float attack(Character enemy)
    {
        if (checkForInRangeEnemies().Contains(enemy))
        {
            float enemyHealthLeft = enemy.takeDamage(1);
            if (enemyHealthLeft == -1)
                enemy.die();
            return enemyHealthLeft;
        }

        else
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("That enemy isn't in range."));
            return -1;
        }
    }
}
