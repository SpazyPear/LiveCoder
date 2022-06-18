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
    

}
