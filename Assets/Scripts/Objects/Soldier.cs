using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using PythonProxies;

public class Soldier : Character
{

    public SoldierData soldierData
    {
        get
        {
            return characterData as SoldierData;
        }
    }

    public override object CreateProxy()
    {
        return new SoldierProxy(this);
    }

    // Start is called before the first frame update
   /* new void Start()
    {
        base.Start();
        photonView.RPC("attack", Photon.Pun.RpcTarget.All, new Entity());
    }*/

}
