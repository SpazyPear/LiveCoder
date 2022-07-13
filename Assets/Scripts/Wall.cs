using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class WallProxy
{
    Entity target;

    [MoonSharpHidden]
    public WallProxy(Wall p)
    {
        this.target = p;
    }
}

public class Wall : Entity
{
    async void Start()
    {
        await Task.Delay(200);

            if (State.GridContents[gridPos.x, gridPos.y + 1].Entity && State.GridContents[gridPos.x, gridPos.y + 1].Entity.GetComponentInChildren<Wall>() || State.GridContents[gridPos.x, gridPos.y - 1].Entity && State.GridContents[gridPos.x, gridPos.y - 1].Entity.GetComponentInChildren<Wall>())
                transform.parent.eulerAngles = new Vector3(0, 90, 0);
    }
}
