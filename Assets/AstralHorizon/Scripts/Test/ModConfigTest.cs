using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModConfigTest : MonoBehaviour
{
    private void Start()
    {
        var modLoader = new ModConfigLoader();
        modLoader.SaveModConfig(new string[] { "hui", "haha", "kalovaya massa"});
        var mods = modLoader.LoadModConfig();

        foreach (var mod in mods)
            Debug.Log(mod);
    }
}
