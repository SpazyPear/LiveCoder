using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GiantScriptableObject", order = 1)]
public class GiantData : CharacterData
{
    public float maxShieldHealth = 10f;
    public float shieldRegenRate = 1f;
}
