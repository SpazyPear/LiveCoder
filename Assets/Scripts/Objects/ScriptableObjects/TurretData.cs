using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TurretScriptableObject", order = 1)]
public class TurretData : ModuleData
{
    public float projectileAliveTime = 3f;
    public float damage = 2;
}
