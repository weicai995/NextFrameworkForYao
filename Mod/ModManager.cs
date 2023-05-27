using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DeckBuildingGame;
using HarmonyLib;
using MonoMod.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NextFrameForYao.WorkShop;
using Spine;
using Debug = UnityEngine.Debug;

namespace NextFrameForYao.Mod;

public class ModManager
{
    
    public static List<ModGroup> modGroups = new List<ModGroup>();
    
    /// <summary>Mod排序</summary>
    /// <param name="modEnumerable"></param>
    /// <returns></returns>
    
    public static FieldInfo[] Card_DataFields = typeof (Card).GetFields();
    
    public static IEnumerable<ModConfig> SortMod(IEnumerable<ModConfig> modEnumerable)
    {
        ModConfig[] array = modEnumerable.ToArray<ModConfig>();
        //     NextModSetting nextModSetting = Main.I.NextModSetting;
        //    Func<ModConfig, ModConfigSetting> selector = (Func<ModConfig, ModConfigSetting>) (modConfig => nextModSetting.GetOrCreateModSetting(modConfig));
        //    return ((IEnumerable<ModConfig>) array).Select<ModConfig, ModConfigSetting>(selector).OrderBy<ModConfigSetting, int>((Func<ModConfigSetting, int>) (modSetting => modSetting.priority)).ThenBy<ModConfigSetting, string>((Func<ModConfigSetting, string>) (modSetting => modSetting.BindMod.SettingKey)).Select<ModConfigSetting, ModConfig>((Func<ModConfigSetting, ModConfig>) (data => data.BindMod));
        return array;
    }
    
    
    /// <summary>加载Mod配置</summary>
    /// <param name="dir"></param>
    /// <param name="showLog"></param>
    /// <returns></returns>
    public static ModConfig LoadModConfig(string dir, bool showLog)
    {
        ModConfig modConfig = ModManager.GetModConfig(dir);
        if (!showLog)
            return modConfig;
        Main.LogInfo((object) string.Format("ModManager.LoadMod", (object) dir));
        return modConfig;
    }
    
    
    private static ModConfig GetModConfig(string dir)
    {
        Main.LogInfo("hh1");
        ModConfig modConfig1 = (ModConfig) null;
        Main.LogInfo("hh2");
        try
        {
            Main.LogInfo("hh3");
            Debug.Log("?: "+dir);
            modConfig1 = ModConfig.Load(dir);
            Main.LogInfo("hh");
        }
        catch (Exception ex)
        {
            Main.LogWarning((object) "ModManager.ModConfigLoadFail");
            Main.LogError((object) ex);
        }
        ModConfig modConfig2 = modConfig1 ?? new ModConfig();
        modConfig2.Path = dir;
        modConfig2.State = ModState.Unload;
        return modConfig2;
    }
    
    
    public static void FirstLoadAllMod()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
     //   ModManager.CloneMainData();
        stopwatch.Stop();
        Main.LogInfo((object) string.Format("储存数据耗时：{0} s", (object) (float) ((double) stopwatch.ElapsedMilliseconds / 1000.0)));
   //     ModManager.OnModLoadStart();
        ModManager.LoadAllMod();
    //    ModManager.OnModLoadComplete();
    }
    
    
    public static void LoadAllMod() {
        Main.LogInfo((object) ("===================" + "ModManager.LoadingModData" + "====================="));
        ModManager.ReloadModMeta(true, true);
        foreach (ModGroup modGroup in ModManager.modGroups)
        {
            foreach (ModConfig modConfig in modGroup.ModConfigs)
            {
                /*if (!Main.I.NextModSetting.GetOrCreateModSetting(modConfig).enable)
                {
                    modConfig.State = ModState.Disable;
                }
                else
                {*/
                    try
                    {
                        ModManager.LoadModData(modConfig);
                    }
                    catch (Exception ex)
                    {
                        Main.LogError((object) string.Format("ModManager.LoadFail", (object) modGroup.GroupKey, (object) modConfig.Path));
                        Main.LogError((object) ex);
                        modConfig.Exception = ex;
                    }
              //  }
            }
        }/*
        ModManager._buildCacheSuccess = false;
        try
        {
            instance.Buff.Clear();
            instance.InitBuff();
            ModManager._buildCacheSuccess = true;
        }
        catch (Exception ex)
        {
            Main.LogError((object) ex);
        }
        if (CheckData.Check())
            return;
        Main.LogError((object) CheckData.log);*/
    }
    
    
    
     /// <summary>重载mod元数据</summary>
    /// <param name="resetModState">是否重置Mod状态</param>
    public static void ReloadModMeta(bool resetModState, bool showLog = false)
    {
      if (resetModState)
        ModManager.modGroups.Clear();
      Main.LogInfo(111);
      DirectoryInfo directoryInfo1 = new DirectoryInfo(Main.PathLocalModsDir.Value);
      Main.LogInfo("222: "+Main.PathLocalModsDir.Value);
      foreach (DirectoryInfo directoryInfo2 in WorkshopTool.GetAllModDirectory())
      {
          Main.LogInfo(directoryInfo2.FullName);
          DirectoryInfo dir = directoryInfo2;
        if (!WorkshopTool.CheckModIsDisable(dir.Name))
        {
          if (resetModState)
          {
            if (Directory.Exists(dir?.ToString() + "\\plugins\\Next"))
            {
              ModGroup modGroup = new ModGroup(dir, ModType.Workshop);
              Main.LogInfo("Add "+dir.FullName);
              ModManager.modGroups.Add(modGroup);
            }
          }
          else if (Directory.Exists(dir?.ToString() + "\\plugins\\Next"))
          {
            if (ModManager.modGroups.Find((Predicate<ModGroup>) (x => x.ModDir.FullName == dir.FullName)) == null)
            {
              ModGroup modGroup = new ModGroup(dir, ModType.Workshop);
              ModManager.modGroups.Add(modGroup);
            }
          }
          else
          {
            ModGroup modGroup = ModManager.modGroups.Find((Predicate<ModGroup>) (x => x.ModDir.FullName == dir.FullName));
            if (modGroup != null)
              ModManager.modGroups.Remove(modGroup);
          }
        }
      }
      foreach (DirectoryInfo directory in directoryInfo1.GetDirectories())
      {
        DirectoryInfo dir = directory;
        if (resetModState)
        {
          if (Directory.Exists(dir?.ToString() + "\\plugins\\Next"))
          {
            ModGroup modGroup = new ModGroup(dir, ModType.Local);
            ModManager.modGroups.Add(modGroup);
          }
        }
        else if (Directory.Exists(dir?.ToString() + "\\plugins\\Next"))
        {
          if (ModManager.modGroups.Find((Predicate<ModGroup>) (x => x.ModDir.FullName == dir.FullName)) == null)
          {
            ModGroup modGroup = new ModGroup(dir, ModType.Local);
            ModManager.modGroups.Add(modGroup);
          }
        }
        else
        {
          ModGroup modGroup = ModManager.modGroups.Find((Predicate<ModGroup>) (x => x.ModDir.FullName == dir.FullName));
          if (modGroup != null)
            ModManager.modGroups.Remove(modGroup);
        }
      }
      foreach (ModGroup modGroup in ModManager.modGroups)
      {
        modGroup.Init(resetModState, showLog);
      }
      ModManager.modGroups = ((IEnumerable<ModGroup>) ModManager.modGroups).ToList<ModGroup>();
      if (!resetModState)
        return;
    }


     public static Dictionary<string, List<Card_>> ModCardsCache = new Dictionary<string, List<Card_>>();
     public static Dictionary<string, Type> ModTypeCache = new Dictionary<string, Type>();


     public static Dictionary<string, List<SkeletonData>> ModSKeletonDataCache =
         new Dictionary<string, List<SkeletonData>>();


     private static void LoadModData(ModConfig modConfig)
    {
      Main.LogInfo((object) ("===================" + "ModManager.StartLoadMod" + "====================="));
      string dataDir = modConfig.GetDataDir();
      string scriptDir = modConfig.GetScriptDir();
      Main.LogInfo((object) ("Mod.Directory" + " : " + Path.GetFileNameWithoutExtension(modConfig.Path)));
      Main.LogIndent = 1;
      Main.LogInfo((object) ("Mod.Name" + " : " + modConfig.Name));
      Main.LogInfo((object) ("Mod.Author" + " : " + modConfig.Author));
      Main.LogInfo((object) ("Mod.Version" + " : " + modConfig.Version));
      Main.LogInfo((object) ("Mod.Description" + " : " + modConfig.Description));
      try
      { 
          
        modConfig.State = ModState.Loading;


        DirectoryInfo di = new DirectoryInfo(scriptDir);
        if (di.Exists)
        {
            Main.LogInfo("开始载入脚本: "+scriptDir);
            var Namespace = "firstPlugin";
            foreach  (FileInfo file in  di.GetFiles("*.dll"))
            {

                var dll = Assembly.LoadFile(@file.FullName);
                
                foreach(Type type in dll.GetExportedTypes())
                {
                    if (typeof(DeckBuildingGame.Card).IsAssignableFrom(type))
                    {
                        Debug.Log("preload: "+type.FullName);
                        ModTypeCache[type.FullName] = type;
                    }
                }
               
                /*Type t = Type.GetType($"{Namespace}."+$"{Path.GetFileNameWithoutExtension(file.FullName)}");
                Debug.Log(t== null);*/
            }
        }
        else
        {
            Main.LogInfo("没有需要载入的脚本文件");
        }
        
        
        #region 导入Card
        var CardFilePath = BepInEx.Utility.CombinePaths(dataDir, "Card.json");
        Card_[] c = null;
        Debug.Log(CardFilePath);
        if (File.Exists(CardFilePath))
        {
            c = (Card_[])JArray.Parse(File.ReadAllText(@CardFilePath)).ToObject(typeof(Card_[]));
        }
        /*
        c = JsonConvert.DeserializeObject<Card_>();*/
        else
        {
            Main.LogInfo("No Card Json");
        }

        if (c != null)
        {
            /*if (ModCardsCache.TryGetValue(modConfig.Name + ".Cards", out var cards))
            {
                cards.Add(c);
            }
            else
            {
                ModCardsCache[modConfig.Name + ".Cards"] = new List<Card_>(c){};
            }*/
            ModCardsCache[modConfig.Name + ".Cards"] = new List<Card_>(c){};
        }
        

        #endregion
        
        Main.Res.CacheAssetDir(modConfig.Path + "\\Assets");
        if (Directory.Exists(modConfig.Path + @"\Assets\spine"))
        {
           var skeletonData = Main.LoadSkeletonData(modConfig.Path+@"\Assets\spine\MarisaModelv3\MarisaModelv3.atlas",modConfig.Path+@"\Assets\spine\MarisaModelv3\MarisaModelv3.json");
           if (skeletonData != null)
           {
               if (ModSKeletonDataCache.TryGetValue(modConfig.Name + ".spine."+skeletonData.Name, out var cards))
               {
                   cards.Add(skeletonData);
               }
               else
               {
                   ModSKeletonDataCache[modConfig.Name + ".spine."+skeletonData.Name] = new List<SkeletonData>(){skeletonData};
               }
           }
        }
      }
      catch (Exception ex)
      {
        modConfig.State = ModState.LoadFail;
        throw;
      }
      modConfig.State = ModState.LoadSuccess;
      Main.LogIndent = 0;
      Main.LogInfo((object) ("===================" + "ModManager.LoadModComplete" + "====================="));
    }



     
     
     
     
    /*public static WorkShopItem ReadConfig(string path)
    {
        WorkShopItem workShopItem = new WorkShopItem();
        string path1 = path + "/Mod.bin";
        if (File.Exists(path1))
        {
            try
            {
                FileStream serializationStream = new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.Read);
                workShopItem = (WorkShopItem) new BinaryFormatter().Deserialize((Stream) serializationStream);
                serializationStream.Close();
            }
            catch (Exception ex)
            {
                Main.LogError((object) ex);
                Main.LogError((object) "读取配置文件失败".I18NTodo());
            }
        }
        return workShopItem;
    }*/
}