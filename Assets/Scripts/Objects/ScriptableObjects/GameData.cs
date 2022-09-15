using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameDataScriptableObject", order = 1)]
public class GameData : ScriptableObject
{
    public int initialGold;
    public int tankKillReward;
    public int brawlerKillReward;
    public int supportKillReward;
}
