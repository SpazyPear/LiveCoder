using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitScriptableObject", order = 1)]
public class UnitData : ScriptableObject
{
    public int maxEnergy = 2;
    public int energyRegenRate = 2;
    public int empResistance = 1;
}
