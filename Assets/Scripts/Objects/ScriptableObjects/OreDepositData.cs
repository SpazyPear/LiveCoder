using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OreDepositScriptableObject", order = 1)]
public class OreDepositData : ScriptableObject
{
    public int health = 2;
    public int value = 5;
    public float duration = 10f;
}
