using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;

public static class State
{
    public static event EventHandler onLevelLoad;
    public static int EnergyRegen = 4;
    public static GameManager gameManager;

    public static readonly byte ReadyUpCode = 0;
    public static readonly byte EMPCode = 1;
    public static readonly byte CodeInjectionCode = 2;
    
    public static void initializeLevel()
    {
        onLevelLoad?.Invoke(null, EventArgs.Empty);
    }

    public static void CallRPC(PhotonView photonView, Action function, RpcTarget target, params object[] para)
    {
        
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(function.GetMethodInfo().Name, target, para);
        }
        else
        {
            function();
        }
    }

}
