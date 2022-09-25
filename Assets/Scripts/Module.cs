using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;


public class Module : MonoBehaviour
{
    public virtual string displayName()
    {
        return "module";
    }

    private Entity attachedEntity;

    private void Awake()
    {
        attachedEntity = GetComponent<Entity>();
    }

    public virtual object CreateProxy()
    {
        return new ModuleProxy(this);
    }

}
