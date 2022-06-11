using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Character
{


    public Soldier()
    {
        maxEnergy = 5;
        currentEnergy = maxEnergy;
        playerSpeed = 2;
        range = 1;
    }
}
