using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Character
{
   
    // Start is called before the first frame update
    void Start()
    {
        initializePlayer("ScriptableObjects/SoldierScriptableObject");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float attack(Character enemy)
    {
        if (checkForInRangeEnemies().Contains(enemy))
            return enemy.takeDamage(1);
        else
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("That enemy isn't in range."));
            return -1;
        }
    }
}
