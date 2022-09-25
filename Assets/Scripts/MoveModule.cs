using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PythonProxies;

public class MoveModule : Module
{

    // EXAMPLE
    // TODO: Implement
    public void Move(int x, int y)
    {
        GetComponent<Character>().replicatedMove(x, y);
    }

    public override object CreateProxy()
    {
        return new MoveModuleProxy(this);
    }

    public override string displayName()
    {
        return "moveModule";
    }

}
