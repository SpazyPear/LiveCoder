using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Drawing.Printing;

public static class FileManager
{
    public static void WriteDefaults()
    {
        UnitConfig Soldier = new UnitConfig() { moduleNames = new List<string> { "MoveModule", "MeleeModule" }, name = "Soldier" };
        UnitConfig Giant = new UnitConfig() { moduleNames = new List<string> { "MoveModule", "ShieldModule" }, name = "Giant" };
        UnitConfig Healer = new UnitConfig() { moduleNames = new List<string> { "MoveModule", "HealerModule" }, name = "Healer" };
        UnitConfig EMP = new UnitConfig() { moduleNames = new List<string> { "MoveModule", "EMPModule" }, name = "EMP" };
        UnitConfig Hacker = new UnitConfig() { moduleNames = new List<string> { "MoveModule", "HackerModule" }, name = "Hacker" };
        UnitConfig Turret = new UnitConfig() { moduleNames = new List<string> { "MoveModule", "TurretModule" }, name = "Turret" };

        SavedUnits savedUnits = new SavedUnits() { units = new List<UnitConfig> { Soldier, Giant, Healer, EMP, Hacker, Turret } };
        
        File.WriteAllText(Application.streamingAssetsPath + "/UnitConfig/defaultUnits.json", JsonUtility.ToJson(savedUnits));
    }
    
    public static UnitConfig GetUnitConfig(string name) 
    {
        if (name == "Empty") { return new UnitConfig { cost = 0, name = "Empty", codeContext = "def OnStart():\n\ndef OnStep():" }; }

        string path = Application.streamingAssetsPath + "/UnitConfig/defaultUnits.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SavedUnits savedUnits = JsonUtility.FromJson<SavedUnits>(json);
            UnitConfig unitConfig = savedUnits.units.FirstOrDefault(x => x.name == name);
            if (unitConfig == null) return null;
            unitConfig.cost = unitConfig.moduleNames.Sum(x => (Resources.Load("ModuleConfig/" + x + "ScriptableObject") as ModuleData).cost);
            return unitConfig;
        }
        return null;
    }
}
