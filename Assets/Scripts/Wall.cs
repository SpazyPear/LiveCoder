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
        await Task.Delay(100);

        CheckWallRotation();
            
    }
    
    public void CheckWallRotation()
    {
        if (isCorner())
        {
            GetComponent<MeshFilter>().mesh = (Resources.Load("Prefabs/CornerWall") as GameObject).GetComponentInChildren<MeshFilter>().mesh;
            GetComponent<MeshRenderer>().material = (Resources.Load("Prefabs/CornerWall") as GameObject).GetComponentInChildren<MeshRenderer>().material;
            return;
        }

        if (State.GridContents[gridPos.x, gridPos.y + 1].Entity && State.GridContents[gridPos.x, gridPos.y + 1].Entity.GetComponentInChildren<Wall>() || State.GridContents[gridPos.x, gridPos.y - 1].Entity && State.GridContents[gridPos.x, gridPos.y - 1].Entity.GetComponentInChildren<Wall>())
        {
            
            transform.parent.eulerAngles = new Vector3(0, 90, 0);
            if (State.GridContents[gridPos.x, gridPos.y + 1].Entity)
                State.GridContents[gridPos.x, gridPos.y + 1].Entity.SendMessage("CheckWallRotation");
            if (State.GridContents[gridPos.x, gridPos.y - 1].Entity)
                State.GridContents[gridPos.x, gridPos.y - 1].Entity.SendMessage("CheckWallRotation");


        }
    }

    bool isCorner()
    {
        bool verticalFound = false;
        bool horizontalFound = false;
        if (containsWall(gridPos.x + 1, gridPos.y) || containsWall(gridPos.x - 1, gridPos.y))
            verticalFound = true;

        if (containsWall(gridPos.x, gridPos.y + 1) || containsWall(gridPos.x, gridPos.y - 1))
            horizontalFound = true;

        return verticalFound && horizontalFound;
    }

    bool containsWall(int x, int y)
    {
        if (State.GridContents[x, y].Entity && State.GridContents[x, y].Entity.GetComponentInChildren<Wall>())
        {
            return true;
        }
        return false;
    }

    
}
