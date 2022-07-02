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

}


public class Soldier : Character
{

    public SoldierData soldierData
    {
        get
        {
            return characterData as SoldierData;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame


}
