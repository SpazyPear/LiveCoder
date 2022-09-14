using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MoonSharp.Interpreter;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

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
   /* new void Start()
    {
        base.Start();
        photonView.RPC("attack", Photon.Pun.RpcTarget.All, new Entity());
    }*/

}
