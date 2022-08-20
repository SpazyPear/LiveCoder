using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CoinStoreScriptableObject", order = 1)]
public class CoinStoreData : EntityData
{
    public int capacity = 200;
}
