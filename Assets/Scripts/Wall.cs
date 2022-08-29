using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class WallProxy : EntityProxy
{
    Entity target;

    [MoonSharpHidden]
    public WallProxy(Wall p) : base(p)
    {
        this.target = p;
    }
}

public class Wall : Entity
{
    bool rotChecked;
    
    async void Start()
    {
        await Task.Yield();
        CheckWallRotation(true);
            
    }
    
    public void CheckWallRotation(bool initiator)
    {

            
        if (wallAt(gridPos.x, gridPos.y + 1) || wallAt(gridPos.x, gridPos.y - 1))
        {
            transform.parent.eulerAngles = new Vector3(0, 90, 0);  
        }

        if (initiator)
            CheckSurroundingWallRotations();

        resetChecked();

        if (isCorner())
        {
            ReplaceWithCorner();
        }
        
    }

    void CheckSurroundingWallRotations()
    {
        if (wallAt(gridPos.x, gridPos.y + 1))
            wallAt(gridPos.x, gridPos.y + 1).CheckWallRotation(false);
        if (wallAt(gridPos.x, gridPos.y - 1))
            wallAt(gridPos.x, gridPos.y - 1).CheckWallRotation(false);
        if (wallAt(gridPos.x + 1, gridPos.y))
            wallAt(gridPos.x + 1, gridPos.y).CheckWallRotation(false);
        if (wallAt(gridPos.x - 1, gridPos.y))
            wallAt(gridPos.x - 1, gridPos.y).CheckWallRotation(false);
    }

    void ReplaceWithCorner()
    {
        GameManager.spawnOnGrid(Resources.Load("Prefabs/CornerWall") as GameObject, gridPos, null, true);
        Destroy(transform.parent.gameObject);
    }

    void resetChecked()
    {
        for (int x = gridPos.x - 1; x < gridPos.x + 1; x++)
        {
            for (int y = gridPos.y - 1; y < gridPos.y + 1; y++)
            {
                if (wallAt(x, y))
                    wallAt(x, y).rotChecked = false;
            }
        }
    }

    bool isCorner()
    {
        bool verticalFound = false;
        bool horizontalFound = false;
        if (wallAt(gridPos.x + 1, gridPos.y) || wallAt(gridPos.x - 1, gridPos.y))
            verticalFound = true;

        if (wallAt(gridPos.x, gridPos.y + 1) || wallAt(gridPos.x, gridPos.y - 1))
            horizontalFound = true;

        return verticalFound && horizontalFound;
    }

    Wall wallAt(int x, int y)
    {
        if (x < 0 || x >= State.GridContents.GetLength(0) || y < 0 || y >= State.GridContents.GetLength(1))
            return null;
        
        if (State.GridContents[x, y].Entity && State.GridContents[x, y].Entity.GetComponentInChildren<Wall>())
        {
            return State.GridContents[x, y].Entity.GetComponentInChildren<Wall>();
        }
        return null;
    }

}
