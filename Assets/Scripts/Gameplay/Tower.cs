using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PythonProxies;
public class Tower : Entity
{
    public int towerHealth = 5;

    public override void die(object sender = null)
    {
        (sender as Entity).ownerPlayer.win();
        base.die(sender);
    }

    public override object CreateProxy()
    {
        return new EntityProxy(this);
    }
}
