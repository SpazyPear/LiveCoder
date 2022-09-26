using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : ControlledMonoBehavour
{
    public Vector2Int gridPos;
    public PlayerManager ownerPlayer;
    public PhotonView photonView;
}
