using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HealerScriptableObject", order = 1)]
public class HealerData : CharacterData
{
    public int healRate = 1;
}
