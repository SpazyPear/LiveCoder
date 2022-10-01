using Photon.Pun;
using PythonProxies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;


public class Unit : PlaceableObject
{
    public Tweener tweener;
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
    public float nextModuleY;

    protected override void Awake()
    {
        base.Awake();   
        if (ownerPlayer)
            ownerPlayer.units.Remove(this);
        tweener = GetComponent<Tweener>();
        codeContext.unit = this;
        GameObject.FindObjectOfType<CodeExecutor>().codeContexts.Add(codeContext);
        PythonInterpreter.AddContext(codeContext);
    }

    public void InitializeUnit(List<string> moduleNames, string code, string name)
    {
        moduleNames.ForEach(x => addModule(x));
        print(moduleNames.Count + "pre added");
        codeContext.source = code;
        currentEnergy = unitData.maxEnergy;
        this.name = name;
    }

    public override void OnStart()
    {
        executing = true;
    }

    public override void OnStep()
    {
        energyRegen();
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
        foreach (Module module in attachedModules) { module.EMPRecover(); }
        codeContext.shouldExecute = false;
    }

    public virtual void OnEMPDisable(float strength)
    {
        if (!codeContext.shouldExecute)
        {
            return;
        }
        codeContext.shouldExecute = true;
        foreach (Module module in attachedModules) { module.OnEMPDisable(strength); }
        EMPTimer(strength);
    }
    
    public void addModule(string moduleName)
    {
        GameManager.CallRPC(this, "replicatedAddModule", RpcTarget.All, moduleName);
    }

    [PunRPC]
    void replicatedAddModule(string moduleName)
    {
        Type moduleType = Type.GetType(moduleName);
        GameObject moduleObj = Instantiate(Resources.Load("Prefabs/Modules/" + moduleName) as GameObject, transform.position, transform.rotation);
        moduleObj.transform.SetParent(transform);
        var copyMethod = typeof(Unit).GetMethod("CopyComponent");
        var genericCopy = copyMethod.MakeGenericMethod(moduleType);
        Module module = genericCopy.Invoke(this, new object[] { moduleObj.GetComponent(moduleType), gameObject }) as Module;
        module.lane = attachedModules.Count;
        module.height = moduleObj.GetComponentInChildren<Renderer>().bounds.extents.y;
        float nextModuleY = module.height + attachedModules.Sum(x => x.height * 2);
        moduleObj.transform.localPosition = new Vector3(0, nextModuleY, 0);
        module.moduleObj = moduleObj;
        attachedModules.Add(module);
    }

    public void removeModule(int lane)
    {
        if (lane < attachedModules.Count)
        {
            Module toRemove = attachedModules[lane];
            nextModuleY -= attachedModules[lane].height;
            List<Module> toMoveDown = attachedModules.Where(x => x.lane > lane).ToList();
            foreach (Module module in toMoveDown)
            {
                module.moduleObj.transform.localPosition -= new Vector3(0, toRemove.height * 2, 0);
                module.lane -= 1;
            }
            GridManager.DestroyObject(attachedModules[lane].moduleObj);
            attachedModules.RemoveAt(lane);
            Destroy(toRemove);
        }
        else
            print("No such module to remove");
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
        GameManager.CallRPC(this, "replicatedSelfDestruct", RpcTarget.All);
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
                            GridManager.GridContents[gridPos.x + x, gridPos.y + y].OccupyingObject.GetComponentInChildren<Unit>().attachedModules.ForEach(x => x.takeDamage(selfDestructDamage));
                    }
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        Camera.main.gameObject.GetComponent<CameraShake>().shakeCamera();
        Instantiate(Resources.Load("PS/PS_Explosion_Rocket") as GameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        var fields = type.GetFields();
        foreach (var field in fields) field.SetValue(copy, field.GetValue(original));
        if (copy is Behaviour)
            (copy as Behaviour).enabled = true;
        return copy as T;
    }


    public PythonProxyObject CreateProxy()
    {
        return new UnitProxy(this);
    }
}
