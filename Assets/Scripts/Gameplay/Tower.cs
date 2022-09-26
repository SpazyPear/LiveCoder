using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PythonProxies;
public class Tower : Entity
{

    public override void die(object sender = null)
    {
        (sender as Unit).ownerPlayer.win();
        base.die(sender);
    }

}
