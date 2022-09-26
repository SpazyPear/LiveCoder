using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;


public abstract class Module : Entity
{
    public ModuleData moduleData;

    public int lane;

    protected Unit owningUnit;
    
    public abstract string displayName();

    protected virtual void Start() { }

    protected virtual void Update() { }


    protected override void Awake()
    {
        currentHealth = moduleData.maxHealth;
        owningUnit = GetComponent<Unit>();
    }

    public abstract object CreateProxy();

    public virtual void OnEMPDisable(float strength) { }

    public virtual void EMPRecover() { }

    //public abstract void addVisuals();
}
