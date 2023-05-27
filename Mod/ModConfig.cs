using System;
using System.IO;
using BepInEx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NextFrameForYao.Mod;

public class ModConfig
{
    public string Name { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public string Path { get; set; }

    [JsonIgnore]
    public Exception Exception { get; set; }
    
    [JsonIgnore]
    public ModState State { get; set; }
    
    public static ModConfig Load(string dir, bool ignoreWarning = false)
    {
        ModConfig modConfig1 = (ModConfig) null;
        Main.LogInfo("hhh2");
        string path2 = Utility.CombinePaths(dir, "Config", "modConfig.json");
        if (File.Exists(path2))
        {
            Main.LogInfo("hhh6");
            Main.LogWarning("h2");
            modConfig1 = JObject.Parse(File.ReadAllText(path2)).ToObject<ModConfig>();
            Main.LogWarning("h3");
            Main.LogInfo("hhh7");
        }
        else
        {
            Main.LogInfo("hhh4");
            if (!ignoreWarning)
                Main.LogWarning((object) ("ModManager.ModConfigDontExist" + " dir : " + dir));
        }
        Main.LogInfo("hhh5");
        ModConfig modConfig2 = modConfig1 ?? new ModConfig();
        modConfig2.Path = dir;
        return modConfig2;
    }
    
     
    
    public string GetDataDir() => this.Path + "\\Data";

    public string GetScriptDir() => this.Path + "\\Script";

}