using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : ControlledMonoBehavour
{
    public Vector2Int gridPos;
    public PlayerManager ownerPlayer;
    public PhotonView photonView;
    public int ViewID => PhotonNetwork.IsConnected ? photonView.ViewID : GetInstanceID();
    public bool AmOwner => PhotonNetwork.IsConnected ? photonView.AmOwner : true;

    protected virtual void Awake()
    {
        if (GetComponentInParent<PhotonView>() == null) { photonView = gameObject.AddComponent<PhotonView>(); }
        else photonView = GetComponentInParent<PhotonView>();
    }

    [PunRPC]
    public void replicatedTeleport(float x, float y, float z, float pitch, float yaw, float roll, int gridX, int gridY)
    {
        transform.position = new Vector3(x, y, z);
        transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        gridPos = new Vector2Int(gridX, gridY);
    }
}
