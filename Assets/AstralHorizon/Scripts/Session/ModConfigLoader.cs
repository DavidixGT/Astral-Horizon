using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using  AstralHorizon.Session.Serializable;

public class ModConfigLoader
{

    public string[] LoadModConfig()
    {
        var modConfig = File.ReadAllText(Application.persistentDataPath + "/ModConfig.txt");
        var gameData = JsonUtility.FromJson<ModConfigSerializable>(modConfig);
        return gameData.mods;
    }
    public void SaveModConfig(string[] _modConfig)
    {
        string jsonString = JsonUtility.ToJson(new ModConfigSerializable(_modConfig));
        Debug.Log("saved: " + jsonString + " : " + Application.persistentDataPath);
        File.WriteAllText(Application.persistentDataPath + "/ModConfig.txt", jsonString);
    }
}