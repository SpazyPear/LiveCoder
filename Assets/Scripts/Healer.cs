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
