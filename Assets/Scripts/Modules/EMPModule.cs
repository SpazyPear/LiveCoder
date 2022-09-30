using Photon.Pun;
using PythonProxies;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EMPModule : Module
{
    public EMPData empData { get { return moduleData as EMPData; } private set { moduleData = value; } }

    protected override void Awake()
    {
        base.Awake();
    }
    
    public void EMP()
    {
        int[] inRange = GridManager.checkForInRangeEntities(owningUnit, empData.range, false, true).Select(x => x.ViewID).ToArray();
        owningUnit.photonView.RPC("replicatedEMP", RpcTarget.AllViaServer, inRange);
    }

    [PunRPC]
    public void replicatedEMP(int[] inRange)
    {
        foreach (int c in inRange)
        {
            if (PhotonView.Find(c).GetComponent<Unit>().ownerPlayer != owningUnit.ownerPlayer)
            {
                PhotonView.Find(c).GetComponent<Unit>().OnEMPDisable(empData.EMPDuration);
            }
        }
    }

    public override string displayName()
    {
        return "EMPModule";
    }

    public override object CreateProxy()
    {
        return new EMPModuleProxy(this);
    }

}
