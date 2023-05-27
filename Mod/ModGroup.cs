using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextFrameForYao.Next;

namespace NextFrameForYao.Mod;

public class ModGroup
{
    
    public ModGroup(DirectoryInfo directory, ModType type)
    {
        this.GroupKey = directory.Name;
        this.ModDir = directory;
        this.Type = type;
    }
    
    public ModType Type { get; set; }
    
    public string GroupKey { get; set; }
    
    public List<ModConfig> ModConfigs { get; set; } = new List<ModConfig>();
    
    public DirectoryInfo ModDir { get; set; }
    
    public void Init(bool resetModState, bool showLog)
    {
       
            this.ModConfigs.Clear();
            if (Directory.Exists(this.ModDir?.ToString() + "/plugins/Next/"))
            {
                foreach (DirectoryInfo directory in new DirectoryInfo(this.ModDir?.ToString() + "/plugins/Next/").GetDirectories("mod*", SearchOption.TopDirectoryOnly))
                {
                    ModConfig modConfig = ModManager.LoadModConfig(directory.FullName, showLog);
                 //   modConfig.SettingKey = this.GroupKey + "." + Path.GetFileNameWithoutExtension(modConfig.Path);
                    this.ModConfigs.Add(modConfig);
                }
            }
        
            //      this.SteamModInfo = ModManager.ReadConfig(this.ModDir.FullName);
            //      this.GroupName = this.SteamModInfo?.Title ?? string.Empty;
            this.ModConfigs = ModManager.SortMod((IEnumerable<ModConfig>) this.ModConfigs).ToList<ModConfig>();
    }
    
    
    
    
    
}