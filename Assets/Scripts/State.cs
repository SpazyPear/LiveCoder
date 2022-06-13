using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp;

public static class State
{
    public static event EventHandler onLevelLoad;
    public static Tile[,] GridContents;
    public static int EnergyRegen = 4;


    public static void initializeLevel()
    {

        onLevelLoad?.Invoke(null, EventArgs.Empty);
    }
}
