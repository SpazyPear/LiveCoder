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
    public UnitData unitData;
    public int currentEnergy;
    public List<Module> attachedModules = new List<Module>();
    public int cost => attachedModules.Sum(x => x.moduleData.cost);
    public CodeContext codeContext;
    public int viewID => photonView.ViewID;
    public bool executing = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (ownerPlayer)
            ownerPlayer.units.Remove(this);
        
        codeContext.unit = this;
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        PythonInterpreter.AddContext(codeContext);
        GameManager.unitInstances.Add(viewID, this);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void addModule(Module module)
    {
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

    public PythonProxyObject CreateProxy()
    {
        return new UnitProxy(this);
    }
}
