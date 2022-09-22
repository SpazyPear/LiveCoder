using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class State
{
    public static event EventHandler onLevelLoad;
    public static int EnergyRegen = 4;
    public static GameManager gameManager;

    public static void initializeLevel()
    {
        onLevelLoad?.Invoke(null, EventArgs.Empty);
    }



}
