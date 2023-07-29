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
using NextFrameworkForYao.DataJson;
using NextFrameworkForYao.WorkShop;
using Spine;
using Spine.Unity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace NextFrameworkForYao.Mod;

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
        ModConfig modConfig1 = (ModConfig) null;
        try
        {
            Debug.Log("?: "+dir);
            modConfig1 = ModConfig.Load(dir);
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
    //  Main.LogInfo(111);
      DirectoryInfo directoryInfo1 = new DirectoryInfo(Main.PathLocalModsDir.Value);
   //   Main.LogInfo("222: "+Main.PathLocalModsDir.Value);
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

   //  public static Dictionary<string, Dictionary<string, List<object>>> ModCache;
     
   public static Dictionary<string, List<Buff_>> ModBuffCache = new Dictionary<string, List<Buff_>>(); 
   public static Dictionary<string, List<TalentTree_>> ModTalentTreeCache = new Dictionary<string, List<TalentTree_>>(); 
   public static Dictionary<string, List<Talent_>> ModTalentCache = new Dictionary<string, List<Talent_>>(); 
   public static Dictionary<string, List<Troop_>> ModTroopCache = new Dictionary<string, List<Troop_>>(); 
   public static Dictionary<string, List<Hex_>> ModHexCache = new Dictionary<string, List<Hex_>>(); 
   public static Dictionary<string, List<Hero_>> ModHeroCache = new Dictionary<string, List<Hero_>>(); 
     public static Dictionary<string, List<Unit_>> ModUnitCache = new Dictionary<string, List<Unit_>>();
     public static Dictionary<string, List<Card_>> ModCardsCache = new Dictionary<string, List<Card_>>();
     public static Dictionary<string, Type> ModTypeCache = new Dictionary<string, Type>();
     public static Dictionary<string, GameObject> ModPrefabCache = new Dictionary<string, GameObject>();

     public static Dictionary<string, List<SkeletonDataAsset>> ModSKeletonDataCache =
         new Dictionary<string, List<SkeletonDataAsset>>();

     public static Dictionary<string, List<Bonus_>> ModBonusCache = new Dictionary<string, List<Bonus_>>();

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
           // c = (Card_[])JArray.Parse(File.ReadAllText(@CardFilePath)).ToObject(typeof(Card_[]));
           c = DeckBuildingGame.JsonHelper.FromJson<Card_>(File.ReadAllText(@CardFilePath));
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

        //  var jsonDatatypes = typeof(JsonData).GetFields();
        foreach (var type in new Type[]{typeof(Unit_[]),typeof(Buff_[]),typeof(Hero_[]),typeof(Troop_[]),typeof(TalentTree_[]),typeof(Hex_[]),typeof(Talent_[]),typeof(Bonus_[])})
        {
            if(type == typeof(JsonData))
                continue;
            var FilePath = BepInEx.Utility.CombinePaths(dataDir, type.Name.TrimEnd("_[]".ToCharArray()) + ".json");
            object array = null;
            if (File.Exists(FilePath))
            {
                array = JArray.Parse(File.ReadAllText(@FilePath)).ToObject(type);
            }
            /*
            c = JsonConvert.DeserializeObject<Card_>();*/
            else
            {
                Main.LogInfo("No "+type.Name+" Json");
            }

            if (array != null)
            {
                if (type == typeof(Unit_[]))
                {
                    ModUnitCache[modConfig.Name + ".Units"] = new List<Unit_>(array as Unit_[]){};
                }
                else if (type == typeof(Hero_[]))
                {
                    ModHeroCache[modConfig.Name + ".Heros"] = new List<Hero_>(array as Hero_[]){};
                }

                if (type == typeof(Buff_[]))
                {
                    ModBuffCache[modConfig.Name + ".Buffs"] = new List<Buff_>(array as Buff_[]){};
                } 
                if (type == typeof(Troop_[]))
                {
                    ModTroopCache[modConfig.Name + ".Troops"] = new List<Troop_>(array as Troop_[]){};
                } 
                if (type == typeof(TalentTree_[]))
                {
                    ModTalentTreeCache[modConfig.Name + ".TalentTrees"] = new List<TalentTree_>(array as TalentTree_[]){};
                } 
                if (type == typeof(Hex_[]))
                {
                    ModHexCache[modConfig.Name + ".Hexs"] = new List<Hex_>(array as Hex_[]){};
                } 
                if (type == typeof(Talent_[]))
                {
                    ModTalentCache[modConfig.Name + ".Talents"] = new List<Talent_>(array as Talent_[]){};
                }

                if (type == typeof(Bonus_[]))
                {
                    ModBonusCache[modConfig.Name + ".Bonus"] = new List<Bonus_>(array as Bonus_[]){};
                }
            }
        }

      //  var sda = Utils.LoadModSkeletonDataAssetRuntime(modConfig.Path + @"\Assets\spine\MarisaModelv3", "MarisaModelv3");
        #endregion

        /// 把单位模型中的spine数据全部预加载
        DirectoryInfo directoryInfo = new DirectoryInfo(modConfig.Path + @"\Assets\UnitModel\Spine\");
        if (directoryInfo.Exists)
        {
            foreach (var subDir in directoryInfo.GetDirectories())
            {
                foreach (var fileInfo in subDir.GetFiles())
                {
                    if (fileInfo.Extension.EndsWith(".json"))
                    {
                       var fileName = fileInfo.Name.TrimEnd(".json".ToCharArray());
                       var skeletonDataAsset =   Utils.LoadModSkeletonDataAssetRuntime(modConfig.Path + @"\Assets\UnitModel\Spine\"+subDir.Name, fileName);
                       if (skeletonDataAsset != null)
                       {
                           if (ModSKeletonDataCache.TryGetValue(modConfig.Name + ".spine."+fileName, out var cards))
                           {
                               cards.Add(skeletonDataAsset);
                           }
                           else
                           {
                               ModSKeletonDataCache[modConfig.Name + ".spine."+fileName] = new List<SkeletonDataAsset>(){skeletonDataAsset};
                           }
                       }
                    }
                }
            }
            // var skeletonData =  Utils.LoadModSkeletonDataAssetRuntime(modConfig.Path + @"\Assets\spine\MarisaModelv3", "MarisaModelv3");;// Main.LoadSkeletonData(modConfig.Path+@"\Assets\spine\MarisaModelv3\MarisaModelv3.atlas",modConfig.Path+@"\Assets\spine\MarisaModelv3\MarisaModelv3.json");
        }
        /// 把UI大立绘中的spine数据全部预加载
        directoryInfo = new DirectoryInfo(modConfig.Path + @"\Assets\UI\StandPic\");
        if (directoryInfo.Exists)
        {
            foreach (var subDir in directoryInfo.GetDirectories())
            {
                foreach (var fileInfo in subDir.GetFiles())
                {
                    if (fileInfo.Extension.EndsWith(".json"))
                    {
                        var fileName = fileInfo.Name.TrimEnd(".json".ToCharArray());
                        var skeletonDataAsset =   Utils.LoadModSkeletonDataAssetRuntime(modConfig.Path + @"\Assets\UI\StandPic\"+subDir.Name, fileName);
                        if (skeletonDataAsset != null)
                        {
                            if (ModSKeletonDataCache.TryGetValue(modConfig.Name + ".StandPicSpine."+fileName, out var cards))
                            {
                                cards.Add(skeletonDataAsset);
                            }
                            else
                            {
                                ModSKeletonDataCache[modConfig.Name + ".StandPicSpine."+fileName] = new List<SkeletonDataAsset>(){skeletonDataAsset};
                            }
                        }
                    }
                }
            }
            // var skeletonData =  Utils.LoadModSkeletonDataAssetRuntime(modConfig.Path + @"\Assets\spine\MarisaModelv3", "MarisaModelv3");;// Main.LoadSkeletonData(modConfig.Path+@"\Assets\spine\MarisaModelv3\MarisaModelv3.atlas",modConfig.Path+@"\Assets\spine\MarisaModelv3\MarisaModelv3.json");
        }
        
        Main.Res.CacheAssetDir(modConfig.Path + "\\Assets");
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