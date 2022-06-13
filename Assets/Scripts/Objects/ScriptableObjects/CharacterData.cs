using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterScriptableObject", order = 1)]
public class CharacterData : ScriptableObject
{
    public int maxEnergy = 5;
    public float playerSpeed = 2;
    public int range = 1;
    public float maxHealth = 4;
}

