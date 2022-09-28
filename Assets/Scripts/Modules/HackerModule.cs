using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Python3DMath;
using PythonProxies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IronPython.Runtime.Profiler;

public class HackerModule : Module
{
    public HackerData hackerData { get { return moduleData as HackerData; } private set { moduleData = value; } }
    bool planted;
    string codeOverride;
    
    public override string displayName()
    {
        return "hackerModule";
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        hackerData = Resources.Load("ModuleConfig/HackerScriptableObject") as HackerData;
        base.Awake();
    }

    public void hack(vector2 direction)
    {
        if (Mathf.Max(direction.x, direction.y) <= hackerData.range)
        {
            Unit target = GridManager.getEntityAtPos(owningUnit.gridPos + new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y)));
            if (target != null && owningUnit.currentEnergy > 0)
            {
                owningUnit.currentEnergy--;
                hackOnClient(target);
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

    void hackOnClient(Unit entity)
    {
        object[] data = new object[2];
        data[0] = codeOverride;
        data[1] = entity.ViewID;
        PhotonNetwork.RaiseEvent(3, (object)data, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (planted && codeOverride != string.Empty)
        {
            Unit entity = other.GetComponentInParent<Unit>();
            if (entity)
            {
                hackOnClient(entity); 
            }
        
        }
    }

    public override object CreateProxy()
    {
        return new HackerModuleProxy(this);
    }

    protected override void AddPrefab()
    {
        moduleObj = GridManager.InstantiateObject("Prefabs/Modules/HackerModule", transform.position, Quaternion.identity);
        moduleObj.transform.SetParent(transform);
    }
}
