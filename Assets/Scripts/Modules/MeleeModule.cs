using Photon.Pun;
using PythonProxies;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeModule : Module
{
    public MeleeData meleeData { get { return moduleData as MeleeData; } private set { moduleData = value; } }
    

    protected override void Awake()
    {
        base.Awake();
    }

    public void attack(int x, int y)
    {
        if (Mathf.Max(x, y) <= meleeData.range)
        {
            Unit target = GridManager.getEntityAtPos(owningUnit.gridPos + new Vector2Int(x, y));
            if (target != null && owningUnit.currentEnergy > 0)
            {
                owningUnit.currentEnergy--; //todo check if exist
                GameManager.CallRPC(this, "replicatedAttack", RpcTarget.All, target.ViewID, lane);
            }
            else
            {
                ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("That target isn't in range."));
            }
        }
        else
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "attack", playerId = gameObject.name }, new Error("Attack position out of range for this unit."));
        }
    }
    
    [PunRPC]
    public virtual void replicatedAttack(int targetInstance, int lane)
    {
        (GridManager.GetObjectInstance(targetInstance) as Unit).attachedModules[lane].takeDamage(1, this);
    }

    public override string displayName()
    {
        return "meleeModule";
    }

    public override object CreateProxy()
    {
        return new MeleeModuleProxy(this);
    }

}
