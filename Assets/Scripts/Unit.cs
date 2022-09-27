using Photon.Pun;
using PythonProxies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : PlaceableObject
{
    public int selfDestructRange = 2;
    public int selfDestructDamage = 2;
    public bool isDisabled;
    public UnitData unitData;
    public int currentEnergy;
    public List<Module> attachedModules = new List<Module>();
    public int cost => attachedModules.Sum(x => x.moduleData.cost);
    public CodeContext codeContext;
    public bool executing = false;
    public new string name;
    
    protected virtual void Awake()
    {
        if (ownerPlayer)
            ownerPlayer.units.Remove(this);
        
        codeContext.unit = this;
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        PythonInterpreter.AddContext(codeContext);
        GameManager.unitInstances.Add(ViewID, this);
    }

    public void InitializeUnit(List<string> moduleNames, string code, string name)
    {
        moduleNames.ForEach(x => addModule(x));
        codeContext.source = code;
        this.name = name;
    }

    public override void OnStart()
    {
        executing = true;
    }

    async void EMPTimer(float strength)
    {
        if (!codeContext.shouldExecute) return;

        float timer = unitData.empResistance * strength;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            await Task.Yield();
        }
        EMPRecover();
    }

    public virtual void EMPRecover()
    {
        attachedModules.ForEach(delegate (Module module) { module.EMPRecover(); });
        codeContext.shouldExecute = false;
    }

    public virtual void OnEMPDisable(float strength)
    {
        if (!codeContext.shouldExecute)
        {
            return;
        }
        codeContext.shouldExecute = true;
        attachedModules.ForEach(delegate (Module module) { module.OnEMPDisable(strength); });
        EMPTimer(strength);
    }
    
    public void addModule(string moduleName)
    {
        Module module = gameObject.AddComponent(Type.GetType(moduleName)) as Module;
        attachedModules.Add(module);
        module.lane = attachedModules.Count;
    }
    
    public void energyRegen()
    {
        currentEnergy = Mathf.Clamp(currentEnergy + 1, 0, unitData.maxEnergy);
    }

    // TODO Change TypeName to enum for ease of use
    public Unit findClosestEntityOfType(Unit sender, string typeName)
    {
        Unit closest = null;
        float minDistance = Mathf.Infinity;
        Type type = Type.GetType(typeName);

        if (type == null)
        {
            ErrorManager.instance.PushError(new ErrorSource { function = "findClosestEntityOfType", playerId = gameObject.name }, new Error("Incorrect type name."));
            return null;
        }

        foreach (Unit c in GameObject.FindObjectsOfType(type))
        {
            if (c == sender)
                continue;

            if (Vector2Int.Distance(c.gridPos, sender.gridPos) < minDistance)
            {
                closest = c;
                minDistance = Vector2Int.Distance(c.gridPos, sender.gridPos);
            }
        }

        return closest;
    }

    public virtual void selfDestruct()
    {
        photonView.RPC("replicatedSelfDestruct", RpcTarget.All);
    }

    [PunRPC]
    public void replicatedSelfDestruct()
    {
        for (int x = -selfDestructRange; x <= selfDestructRange; x++)
        {
            for (int y = -selfDestructRange; y <= selfDestructRange; y++)
            {
                try
                {
                    if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject && GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>() != gameObject.GetComponentInChildren<Unit>())
                    {
                        if (GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>() != null)
                            GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>().attachedModules.ForEach(x => x.takeDamage(2));
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        Camera.main.gameObject.GetComponent<CameraShake>().shakeCamera();
        Instantiate(Resources.Load("PS/PS_Explosion_Rocket") as GameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    
    public PythonProxyObject CreateProxy()
    {
        return new UnitProxy(this);
    }
}
