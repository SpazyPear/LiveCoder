using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ShieldScriptableObject", order = 1)]
public class ShieldData : ModuleData
{
    public float maxShieldHealth = 10f;
    public float shieldRegenRate = 1f;
}
