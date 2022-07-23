using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantProxy
{
    Giant target;

    [MoonSharp.Interpreter.MoonSharpHidden]
    public GiantProxy(Giant p)
    {
        this.target = p;
    }
}

public class Giant : Character
{
    public GameObject shield;
    public Transform shieldDownPoint;
    public Transform shieldUpPoint;
    public GiantData giantData
    {
       get { return characterData as GiantData; }
    }
    // Start is called before the first frame update
    
    public void deployShield(bool raised)
    {
        shield.SetActive(true);
        Transform transform = raised ? shieldUpPoint : shieldDownPoint;
        shield.transform.position = transform.position;
        shield.transform.rotation = transform.rotation;
    }

}
