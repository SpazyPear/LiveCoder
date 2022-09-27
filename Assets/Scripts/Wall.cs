
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Photon.Pun;

using PythonProxies;
public class WallProxy : EntityProxy
{
    Wall target;

    public WallProxy(Wall p) : base(p)
    {
        this.target = p;
    }
}

public class Wall : Entity
{
    [SerializeField] bool rotationInitiator = true;
    
    async public override void Start()
    {
        await Task.Yield();
        if (photonView.IsMine && rotationInitiator)
            CheckWallRotation(rotationInitiator);
            
    }
    
    public void CheckWallRotation(bool initiator)
    {
        if (wallAt(gridPos.x, gridPos.y + 1) || wallAt(gridPos.x, gridPos.y - 1))
        {
            photonView.RPC("RotateWall", RpcTarget.All);
        }

        if (initiator)
            CheckSurroundingWallRotations();

        if (isCorner())
        {
            ReplaceWithCorner();
        }
        
    }

    [PunRPC]
    public void RotateWall()
    {
        transform.eulerAngles = new Vector3(0, 90, 0);
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
        GridManager.spawnOnGrid("CornerWall", gridPos, true);
        PhotonNetwork.Destroy(transform.gameObject);
    }
/*
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
    }*/

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
        if (x < 0 || x >= GridManager.GridContents.GetLength(0) || y < 0 || y >= GridManager.GridContents.GetLength(1))
            return null;
        
        if (GridManager.GridContents[x, y].OccupyingObject && GridManager.GridContents[x, y].OccupyingObject.GetComponentInChildren<Wall>())
        {
            return GridManager.GridContents[x, y].OccupyingObject.GetComponentInChildren<Wall>();
        }
        return null;
    }

/*    public override string displayName()
    {
        return "wall";
    }

    public override object CreateProxy()
    {
        throw new NotImplementedException();
    }*/
}
