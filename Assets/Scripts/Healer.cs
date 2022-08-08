using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProxy
{
    Healer target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public HealerProxy(Healer p)
    {
        this.target = p;
    }
}

public class Healer : Character
{

    public HealerData healerData
    {
        get { return characterData as HealerData; }
    }
    // Start is called before the first frame update

    public void heal()
    {
        if (!isDisabled)
        {
            List<Character> inRangeCharacters = checkForInRangeEntities<Character>();
            foreach (Character c in inRangeCharacters)
            {
                try
                {
                    c.currentHealth += healerData.healRate;
                }
                catch (System.Exception e) { }
            }
        }
    }

    public void EMP()
    {
        if (!isDisabled)
        {
            List<Entity> inRange = checkForInRangeEntities<Entity>();
            foreach (Entity c in inRange)
            {
                if (c.ownerPlayer != ownerPlayer)
                {
                    c.OnEMPDisable(healerData.EMPStrength);
                }
            }
        }
    }

}
