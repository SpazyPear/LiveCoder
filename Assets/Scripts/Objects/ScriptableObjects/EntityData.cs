using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EntityScriptableObject", order = 1)]
public class EntityData : ScriptableObject
{
    public int maxHealth;
    public int cost;
    public float empResistance = 1;
}
