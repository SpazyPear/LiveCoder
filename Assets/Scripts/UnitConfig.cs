using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitConfig
{
    public List<string> moduleNames;
    public string codeContext;
    public string name;
    [System.NonSerialized] public int cost;
}

[System.Serializable]
public class SavedUnits
{
    public List<UnitConfig> units;
}
