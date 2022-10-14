using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using System.Drawing.Printing;
using Microsoft.Scripting.Utils;
using UnityEngine.Events;

public static class State
{
    public static UnityEvent onLevelLoad = new UnityEvent();
    public static int EnergyRegen = 4;
    public static GameManager gameManager;

    public static readonly byte ReadyUpCode = 0;
    public static readonly byte EMPCode = 1;
    public static readonly byte CodeInjectionCode = 2;
    
    public static void initializeLevel()
    {
        onLevelLoad.Invoke();
    }

}
