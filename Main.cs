using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx;
using DeckBuildingGame;
using NextFrameworkForYao.Next;
using NextFrameworkForYao.Patch;
using HarmonyLib;
using NextFrameworkForYao.Mod;
using NextFrameworkForYao.Res;
using NextFrameworkForYao.Tool;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NextFrameworkForYao;


[HarmonyPatch]
[BepInPlugin("yaoguaihunter.plugin.Next", "NextFrameworkForYao", "0.0.1")]
public class Main : BaseUnityPlugin
{
    
    
    private ResourcesManager _resourcesManager;
    public static Lazy<string> PathLocalModsDir;
    public static Lazy<string> PathLibraryDir;
    public static Lazy<string> PathConfigDir;
    public static Lazy<string> PathExportOutputDir;
    public static Lazy<string> PathBaseDataDir;
    public static Lazy<string> PathLuaLibDir;
    public static Lazy<string> PathLanguageDir;
    public static Lazy<string> PathModSettingFile;
    public static Lazy<string> PathInnerAssetDir;
    public static Lazy<string> PathBaseFungusDataDir;
    public static Lazy<string> PathAbDir;
    public static int LogIndent;
    
    
    private void InitDir()
    {
        string dllPath = Directory.GetParent(typeof (Main).Assembly.Location).FullName;
        Debug.Log("path: "+dllPath);
        Main.PathLocalModsDir = new Lazy<string>((Func<string>) (() => BepInEx.Paths.GameRootPath + "\\本地Mod测试"));
        Main.PathLibraryDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(dllPath, "NextLib")));
        Main.PathConfigDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(dllPath, "NextConfig")));
        Main.PathExportOutputDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(dllPath, "../OutPut")));
        Main.PathBaseDataDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(Main.PathExportOutputDir.Value, "Data")));
        Main.PathBaseFungusDataDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(Main.PathExportOutputDir.Value, "Fungus")));
        Main.PathLuaLibDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(Main.PathLibraryDir.Value, "Lua")));
        Main.PathAbDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(Main.PathLibraryDir.Value, "AB")));
        Main.PathLanguageDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(Main.PathConfigDir.Value, "language")));
        Main.PathModSettingFile = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(BepInEx.Paths.GameRootPath, "nextModSetting.json")));
        Main.PathInnerAssetDir = new Lazy<string>((Func<string>) (() => BepInEx.Utility.CombinePaths(dllPath, "NextAssets")));
    }
    
    
    public static Main Instance => Main.I;

    public static Main I { get; private set; }

    public static ResourcesManager Res => Main.I._resourcesManager;
    
   // public NextModSetting NextModSetting;

    private void Awake() => this.Init();

    private void Start() => this.AfterInit();
    
    
//    internal void LoadModSetting() => this.NextModSetting = NextModSetting.LoadSetting();

  //  internal void SaveModSetting() => NextModSetting.SaveSetting(this.NextModSetting);
    
    private void Init()
    {
        Main.I = this;
        this.InitDir();
        new Harmony("yaoguaihunter.plugin.Next").PatchAll();
        this._resourcesManager = this.gameObject.AddComponent<ResourcesManager>();
        this._resourcesManager.Init();
        // Plugin startup logic
        /*LoadTexturePatch.LoadTexture2D("challengeMode");*/
        ModManager.FirstLoadAllMod();
        Logger.LogInfo($"Plugin {"yaoguaihunter.plugin.Next"} is loaded!");
    }
    
    private void AfterInit(){}


    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.SwitchHeroPage))]
    public static bool PatchModSpine(MainMenuManager __instance, ref bool modHero)
    {
        modHero = true;
        var ska = __instance.HeroPage.transform.Find("HeroInfos").Find("立绘").Find("侍卫")
            .GetComponent<SkeletonAnimation>();
        foreach (var pair  in ModManager.ModSKeletonDataCache)
        {
            foreach (var sda in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");#1#
                Utils.ChangeSkeletonDataAssetRuntime(sda,ska);
                Main.LogInfo("替换了 选人界面立绘: "+ pair.Key);
            }
        }
        return true;
    }*/
    
    
    
    public static SkeletonData LoadSkeletonData(string atlasPath,string  jsonPath)
    {
        Debug.Log("atlasPath: "+atlasPath);
        Debug.Log("jsonPath: "+jsonPath);
        using (StreamReader reader = new StreamReader(atlasPath))
        {
            TextureLoader textureLoader = new SpineTextureLoader();
            Atlas atlas = new Atlas((TextReader)reader, Path.GetDirectoryName(atlasPath), textureLoader);
            AtlasAttachmentLoader attachmentLoader = new AtlasAttachmentLoader(atlas);
            SkeletonJson json = new SkeletonJson(attachmentLoader);
            SkeletonData skeletonData = json.ReadSkeletonData(jsonPath);
            Debug.Log("Load 成功: "+skeletonData.Name);
            return skeletonData;
        }

        return null;
    }

    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.LoadLevel))]
    static void PatchSpine(RunManager __instance)
    {
        var Player= (DeckBuildingGame.Unit) Traverse.Create(__instance).Field("Player").GetValue();
        Player.UnitView.transform.Find("Avatar").Find("Spine").GetComponent<Spine.Unity.SkeletonAnimation>();
        foreach (var pair  in ModManager.ModSKeletonDataCache)
        {
            foreach (var skeletonData in pair.Value)
            {
                var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");
                DataManager.Instance.导入Mod卡(skeletonData);
            }
        }
    }*/
    
   
    /*[HarmonyPostfix]
    [HarmonyPatch(typeof(DataManager), nameof(DataManager.Init))]
    private static void AddCard()
    {
        foreach (var pair  in ModManager.ModCardsCache)
        {
            foreach (var card_ in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");#1#
                DataManager.Instance.导入Mod卡(card_);
                Main.LogInfo("Add Card: "+ card_.CardName);
            }
        }
        /*Logger.LogInfo(dataList.Length);#1#
    }*/
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DataManager), nameof(DataManager.Init))]
    private static void ImplementMod()
    {
        foreach (var pair  in ModManager.ModCardsCache)
        {
            foreach (var card_ in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod卡(card_);
                Main.LogInfo("Add Card: "+ card_.CardName);
            }
        }
        foreach (var pair  in ModManager.ModUnitCache)
        {
            var modName = pair.Key.TrimEnd(".Units".ToCharArray());
            foreach (var Unit_ in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                
                /*if (Unit_.Pic.Contains("."))
                {
                    var modName = Unit_.Pic.Split('.')[0];
                    /*Main.LogError(ModManager.modGroups.Count);
                    foreach (var VARIABLE in ModManager.modGroups)
                    {
                        Main.LogError(VARIABLE.ModConfigs[0].Name);
                    }#1#
                    var modGroup = ModManager.modGroups.FirstOrDefault(i => i.ModConfigs[0].Name == modName);
                    if (modGroup == null)
                    {
                        Main.LogError("Load Unit failed: "+"No such ModGroup called "+modName);
                        continue;
                    }
                    var unitPrafabName = Unit_.Pic.Split('.').Last();
                    if (modGroup.ModConfigs.Count == 0)
                    {
                        Main.LogError("Load Unit failed: "+"No such mod config file for modGroup "+modName);
                        continue;
                    }
                    if (!Directory.Exists(modGroup.ModConfigs[0].Path + @"\Assets\spine\"+unitPrafabName))
                    {
                        Main.LogError("Load Unit failed: "+"No such spine file called "+unitPrafabName+" for Unit "+Unit_.id);
                        continue;
                    }
                    else
                    {
                        ///主角3 是模板
                        var newUnit = UnityEngine.Object.Instantiate(ResHelper.GetUnitPrefab("主角3"));
                        var sda = newUnit.transform.Find("Spine").GetComponent<SkeletonAnimation>();
                        Utils.ChangeSkeletonDataAssetRuntime(ModManager.ModSKeletonDataCache[modGroup.ModConfigs[0].Name + ".spine."+unitPrafabName][0],sda);
                        LoadUnitPrefabPatch.UnitPrefabCacche[Unit_.Pic] = newUnit;
                        newUnit.SetActive(false);
                        GameObject.DontDestroyOnLoad(newUnit);
                        Main.LogInfo("Load UnitPrefab: "+Unit_.Pic);
                    }
                }*/
               
                    /*Main.LogError(ModManager.modGroups.Count);
                    foreach (var VARIABLE in ModManager.modGroups)
                    {
                        Main.LogError(VARIABLE.ModConfigs[0].Name);
                    }*/
                    var modGroup = ModManager.modGroups.FirstOrDefault(i => i.ModConfigs[0].Name == modName);
                    if (modGroup == null)
                    {
                        Main.LogError("Load Unit failed: "+"No such ModGroup called "+modName);
                        continue;
                    }
                    var unitPrafabName = Unit_.Pic;
        
                    if (modGroup.ModConfigs.Count == 0)
                    {
                        Main.LogInfo("Load Unit failed: "+"No such mod config file for modGroup "+modName);
                        continue;
                    }
                    /*if (Unit_.Spine && !Directory.Exists(modGroup.ModConfigs[0].Path + @"\Assets\UnitModel\Spine\"+unitPrafabName))
                    {
                        Main.LogInfo("Load Unit failed: "+"No such spine file called "+unitPrafabName+" for Unit "+Unit_.id);
                        continue;
                    }
                    else if (!Unit_.Spine && !Directory.Exists(modGroup.ModConfigs[0].Path + @"\Assets\UnitModel\Pic\"))
                    {
                        Main.LogInfo("Load Unit failed: "+"No such Pic file called "+unitPrafabName+" for Unit "+Unit_.id);
                        continue;
                    }*/
                    else 
                    {
                        ///主角3 是模板
                        if(Unit_.Spine){
                            if (ModManager.ModSKeletonDataCache.TryGetValue(
                                    modGroup.ModConfigs[0].Name + ".spine." + unitPrafabName,
                                    out var skeletonDatacache))
                            {
                                var newUnit = UnityEngine.Object.Instantiate(ResHelper.GetUnitPrefab("主角3"));
                                var sda = newUnit.transform.Find("Spine").GetComponent<SkeletonAnimation>();
                                Utils.ChangeSkeletonDataAssetRuntime(skeletonDatacache[0],sda);
                                LoadUnitPrefabPatch.UnitPrefabCacche[Unit_.Pic] = newUnit;
                                newUnit.SetActive(false);
                                GameObject.DontDestroyOnLoad(newUnit);
                                Main.LogInfo("Load UnitPrefab with spine: "+Unit_.Pic);
                            }
                            else
                            {
                                var newUnit = UnityEngine.Object.Instantiate(ResHelper.GetUnitPrefab(unitPrafabName));
                                LoadUnitPrefabPatch.UnitPrefabCacche[Unit_.Pic] = newUnit;
                                newUnit.SetActive(false);
                                GameObject.DontDestroyOnLoad(newUnit);
                                Main.LogInfo("Load UnitPrefab with spine: "+Unit_.Pic);
                            }
                        }
                        else
                        {
                            
                            var newUnit = UnityEngine.Object.Instantiate(ResHelper.GetUnitPrefab("招的符尸"));
                         //   var sda = newUnit.transform.Find("Spine").GetComponent<SpriteRenderer>();
                        //    sda.enabled = false;
                           // Utils.ChangeSkeletonDataAssetRuntime(ModManager.ModSKeletonDataCache[modGroup.ModConfigs[0].Name + ".spine."+unitPrafabName][0],sda);
                           var sr =  newUnit.transform.Find("Spine").gameObject.GetComponent<SpriteRenderer>();
                           if (!Main.Res.TryGetAsset("assets/UnitModel/Pic/" + Unit_.Pic + ".png", out var asset) || !(asset is Texture2D texture))
                           {
                               LogError("找不到角色图片: "+"assets/UnitModel/Pic/" + Unit_.Pic + ".png");
                               sr.sprite = null;
                           }
                           else{
                               /*LogError("找到角色图片: "+"assets/" + Unit_.Pic + ".png");
                              var sa = newUnit.transform.Find("Spine").gameObject.GetComponent<SkeletonAnimation>();
                              GameObject.DestroyImmediate(sa);
                              var mf = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshFilter>();
                              GameObject.DestroyImmediate(mf);
                              var mr = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshRenderer>();
                              GameObject.DestroyImmediate(mr);*/
                               sr.sprite = Main.Res.GetSpriteCache(texture);
                           }
                           LoadUnitPrefabPatch.UnitPrefabCacche[Unit_.Pic] = newUnit;
                           newUnit.SetActive(false);
                           GameObject.DontDestroyOnLoad(newUnit);
                           Main.LogInfo("Load UnitPrefab with pic: "+Unit_.Pic);
                        }
                    }
                
                DataManager.Instance.导入Mod(Unit_);
                ///需要创建UnitPrefab的场合
                
                Main.LogInfo("Add Unit: "+ Unit_.id);
            }
        }

      
        foreach (var pair in ModManager.ModHeroCache)
        {
            foreach (var hero in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(hero);
                Main.LogInfo("Add Hero: " + (hero).id);
            }
        }
        
        foreach (var pair in ModManager.ModBuffCache)
        {
            foreach (var buff in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(buff);
                Main.LogInfo("Add Buff: " + (buff).id);
            }
        }
        
        foreach (var pair in ModManager.ModTalentCache)
        {
            foreach (var talent in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(talent);
                Main.LogInfo("Add Talent: " + (talent).id);
            }
        }
        
        foreach (var pair in ModManager.ModTalentTreeCache)
        {
            foreach (var talentTree in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(talentTree);
                Main.LogInfo("Add TalentTree: " + (talentTree).id);
            }
        }
        
        foreach (var pair in ModManager.ModHexCache)
        {
            foreach (var hex in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(hex);
                Main.LogInfo("Add Hex: " + (hex).id);
            }
        }
        
        foreach (var pair in ModManager.ModTroopCache)
        {
            foreach (var troop in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(troop);
                Main.LogInfo("Add Troop: " + (troop).id);
            }
        }
        foreach (var pair in ModManager.ModBonusCache)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add Bonus: " + (bonus).id);
            }
        }
        
        foreach (var pair in ModManager.ModSystemCache)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add System: " + (bonus).id);
            }
        }
        foreach (var pair in ModManager.ModSoundCache)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add Sound: " + (bonus).id);
            }
        }
        foreach (var pair in ModManager.ModAscension)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add Ascension: " + (bonus).id);
            }
        }
        foreach (var pair in ModManager.ModDialogue)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add dialogue: " + (bonus).id);
            }
        }
        
        foreach (var pair in ModManager.ModDialogueTiming)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add DialogueTiming: " + (bonus).id);
            }
        }
        foreach (var pair in ModManager.ModHeroLevel)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add HeroLevel: " + (bonus).id);
            }
        }
        foreach (var pair in ModManager.ModAchievement)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add Achievement: " + (bonus).id);
            }
        }
        foreach (var pair in ModManager.ModChallenge)
        {
            foreach (var bonus in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");*/
                DataManager.Instance.导入Mod(bonus);
                Main.LogInfo("Add Challenge: " + (bonus).id);
            }
        }
        /*Logger.LogInfo(dataList.Length);*/
        LogInfo("Mod加载完毕");
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(ProfileData), nameof(ProfileData.Import))]
    public static void ModifyModAscension()
    {
        foreach (var pair in ModManager.ModHeroCache)
        {
            foreach (var hero in pair.Value)
            {
                if(!DeckBuildingGame.GameManager.Instance.ModAscensionLevel.TryGetValue(hero.id,out var _))
                {
                    DeckBuildingGame.GameManager.Instance.ModAscensionLevel[hero.id] = 0;
                }
                if(!DeckBuildingGame.GameManager.Instance.ModAscensionSteps.TryGetValue(hero.id,out var _))
                {
                    DeckBuildingGame.GameManager.Instance.ModAscensionSteps[hero.id] = 0;
                }
                Main.LogInfo("Add Hero Ascension Save File: " + (hero).id);
            }
        }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DeckBuildingGame.GameManager), nameof(DeckBuildingGame.GameManager.InitProfile))]
    public static void ModifyModAscension2()
    {
        if(DeckBuildingGame.GameManager.Instance.ModAscensionLevel == null)
            DeckBuildingGame.GameManager.Instance.ModAscensionLevel = new Dictionary<string, int>();
        if(DeckBuildingGame.GameManager.Instance.ModAscensionSteps == null)
            DeckBuildingGame.GameManager.Instance.ModAscensionSteps = new Dictionary<string, int>();
        foreach (var pair in ModManager.ModHeroCache)
        {
            foreach (var hero in pair.Value)
            {
                if(!DeckBuildingGame.GameManager.Instance.ModAscensionLevel.TryGetValue(hero.id,out var _))
                {
                    DeckBuildingGame.GameManager.Instance.ModAscensionLevel[hero.id] = 0;
                }
                if(!DeckBuildingGame.GameManager.Instance.ModAscensionSteps.TryGetValue(hero.id,out var _))
                {
                    DeckBuildingGame.GameManager.Instance.ModAscensionSteps[hero.id] = 0;
                }
                Main.LogInfo("Add Hero Ascension Save File: " + (hero).id);
            }
        }
    }
   


    /// <summary>
/// 根据mod新加入的 Hero, 创建选人界面中对应的按钮
/// </summary>
/// <returns></returns>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.InitHeroPage))]
    public static void AddNewHeroBtn(MainMenuManager __instance)
    {
        
        List<GameObject> HeroBtns= Traverse.Create(__instance).Field("HeroBtns").GetValue() as List<GameObject>;
        Debug.LogError("length : "+HeroBtns.Count);
        var btn = HeroBtns[0];
        var 名字GameObject = btn.transform.parent.parent.Find("HeroInfos").Find("人名").Find("HeroName").Find("侍卫").gameObject;

        var oldValue = DeckBuildingGame.GameManager.Instance.HeroNum;
        
            foreach (var pair in ModManager.ModHeroCache)
            {
                var modName = pair.Key.TrimEnd(".Heros".ToCharArray());
             
                foreach (var Hero_ in pair.Value)
                {
                  
                    if (Hero_.id == "侍卫" || Hero_.id == "道士" || Hero_.id == "巫女" || Hero_.id == "龙妹") continue;


                    Sprite namePicSprite = null;
                    float namePicLocX = 0;
                    float namePicLocY = 0;
                    float namePicSizeX = 0;
                    float namePicSizeY = 0;
                    Sprite btnPickPicSprite = null;
                    Sprite btnUnpickPicSprite = null;
                    
                    // 如果没有修改立绘的需求
                    if (Hero_.ModData==null)
                    {
                      
                        if (Hero_.Unit == "主角3")
                        {
                            var 立绘 = __instance.HeroArt.transform.Find("侍卫").gameObject;

                            var clone立绘 = GameObject.Instantiate(立绘, 立绘.transform.parent);
                            clone立绘.transform.localPosition = new Vector3(clone立绘.transform.localPosition.x,
                                clone立绘.transform.localPosition.y,
                                clone立绘.transform.localPosition.z);
                            clone立绘.name = Hero_.id;
                            clone立绘.SetActive(false);
                        }
                        else if (Hero_.Unit == "主角2")
                        {
                            var 立绘 = __instance.HeroArt.transform.Find("道士").gameObject;

                            var clone立绘 = GameObject.Instantiate(立绘, 立绘.transform.parent);
                            clone立绘.transform.localPosition = new Vector3(clone立绘.transform.localPosition.x,
                                clone立绘.transform.localPosition.y,
                                clone立绘.transform.localPosition.z);
                            clone立绘.name = Hero_.id;
                            clone立绘.SetActive(false);
                        }
                        else if (Hero_.Unit == "主角4")
                        {
                            var 立绘 = __instance.HeroArt.transform.Find("巫女").gameObject;

                            var clone立绘 = GameObject.Instantiate(立绘, 立绘.transform.parent);
                            clone立绘.transform.localPosition = new Vector3(clone立绘.transform.localPosition.x,
                                clone立绘.transform.localPosition.y,
                                clone立绘.transform.localPosition.z);
                            clone立绘.name = Hero_.id;
                            clone立绘.SetActive(false);
                        }
                        else if (Hero_.Unit == "主角5")
                        {
                            var 立绘 = __instance.HeroArt.transform.Find("龙妹").gameObject;

                            var clone立绘 = GameObject.Instantiate(立绘, 立绘.transform.parent);
                            clone立绘.transform.localPosition = new Vector3(clone立绘.transform.localPosition.x,
                                clone立绘.transform.localPosition.y,
                                clone立绘.transform.localPosition.z);
                            clone立绘.name = Hero_.id;
                            clone立绘.SetActive(false);
                        }
                        
                        Debug.LogError("A3");
                    }
                    else
                    {
                        var modData = Hero_.ModData.Split('，');
                
                            var 立绘 = __instance.HeroArt.transform.Find("侍卫").gameObject;
                            var clone立绘 = GameObject.Instantiate(立绘, 立绘.transform.parent);
                            clone立绘.transform.localPosition = new Vector3(clone立绘.transform.localPosition.x, 0f,
                                clone立绘.transform.localPosition.z);
                            clone立绘.name = Hero_.id;
                            clone立绘.SetActive(false);
                            var ska = clone立绘.GetComponent<SkeletonAnimation>();
                            
                            
                            var standPicPath= "";
                            var standPicLocX =  0f;
                            var standPicLocY =  0f;
                            var standPicSizeX = 0f;
                            var standPicSizeY = 0f;
                            var namePicPath = "";
                            var btnPickPicPath = "";
                            var btnUnpickPath = "";
                            var useSpine = false;
                            try
                            { 
                              //  Debug.LogError("Moddatalength: "+ modData.Length);
                              standPicPath = modData[0];
                              standPicLocX =  float.Parse(modData[1]);
                              standPicLocY =  float.Parse(modData[2]);
                              standPicSizeX = float.Parse(modData[3]);
                              standPicSizeY = float.Parse(modData[4]);
                              namePicPath = modData[5];
                             namePicLocX = float.Parse(modData[6]);
                             namePicLocY = float.Parse(modData[7]);
                             namePicSizeX =float.Parse(modData[8]);
                             namePicSizeY = float.Parse(modData[9]);
                              btnPickPicPath = modData[10];
                              btnUnpickPath = modData[11];
                              useSpine = bool.Parse(modData[12]);
                             
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                             
                            clone立绘.transform.localPosition = new Vector3(standPicLocX, standPicLocY, clone立绘.transform.localPosition.z);
                            clone立绘.transform.localScale = new Vector3(standPicSizeX, standPicSizeY, clone立绘.transform.localScale.z);
                            
                            ///大立绘使用 spine的场合
                          //  if (standPicPath.StartsWith("Spine_"))
                           
                          if(useSpine)
                          {
                              Debug.Log("使用spine");
                                if (ModManager.ModUnitCache.TryGetValue(modName + ".Units", out var units))
                                {
                                
                                    var unit = units.FirstOrDefault(i => i.id == Hero_.Unit);
                                    if (unit!= null)
                                    {
                                        if (ModManager.ModSKeletonDataCache.TryGetValue(
                                                modName + ".StandPicSpine." + standPicPath, out var sda))
                                        {
                                            Utils.ChangeSkeletonDataAssetRuntime(sda[0], ska);
                                            Debug.Log("加载立绘spine");
                                        }
                                        else
                                        {
                                            Debug.Log("无法加载spine： "+ modName + ".StandPicSpine." + standPicPath);
                                        }
                                    }
                                }
                            }
                            ///大立绘使用 png图片的场合
                            else
                            {
                                ///先删除spine部分
                                var spc = clone立绘.GetComponent<StandingPicController>();
                                GameObject.DestroyImmediate(spc);
                                var sa = clone立绘.GetComponent<SkeletonAnimation>();
                                GameObject.DestroyImmediate(sa);
                                var mr = clone立绘.GetComponent<MeshRenderer>();
                                GameObject.DestroyImmediate(mr);
                                var mf = clone立绘.GetComponent<MeshFilter>();
                                GameObject.DestroyImmediate(mf);
                                
                                
                                Sprite standPicSprite;
                                if (!Main.Res.TryGetAsset("assets/UI/StandPic/" + standPicPath + ".png", out var asset) || !(asset is Texture2D texture))
                                {
                                    LogError("找不到角色图片: "+"assets/UI/StandPic/" + standPicPath + ".png");
                                }
                                else{
                                    /*LogError("找到角色图片: "+"assets/" + Unit_.Pic + ".png");
                                   var sa = newUnit.transform.Find("Spine").gameObject.GetComponent<SkeletonAnimation>();
                                   GameObject.DestroyImmediate(sa);
                                   var mf = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshFilter>();
                                   GameObject.DestroyImmediate(mf);
                                   var mr = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshRenderer>();
                                   GameObject.DestroyImmediate(mr);*/
                                    standPicSprite = Main.Res.GetSpriteCache(texture);
                                   var sr = clone立绘.AddComponent<SpriteRenderer>();
                                    sr.sprite = standPicSprite;
                                    Debug.Log("添加sprite成功："+ clone立绘.transform.localPosition);
                                }
                            }
                            
                            
                          
                            if (!Main.Res.TryGetAsset("assets/UI/NamePic/" + namePicPath + ".png", out var asset2) || !(asset2 is Texture2D texture2))
                            {
                                LogError("找不到角色图片: "+"assets/UI/NamePic/" + namePicPath + ".png");
                            }
                            else{
                                /*LogError("找到角色图片: "+"assets/" + Unit_.Pic + ".png");
                               var sa = newUnit.transform.Find("Spine").gameObject.GetComponent<SkeletonAnimation>();
                               GameObject.DestroyImmediate(sa);
                               var mf = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshFilter>();
                               GameObject.DestroyImmediate(mf);
                               var mr = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshRenderer>();
                               GameObject.DestroyImmediate(mr);*/
                                namePicSprite = Main.Res.GetSpriteCache(texture2);
                            }
                            if (!Main.Res.TryGetAsset("assets/UI/BtnPic/" + btnPickPicPath + ".png", out var asset3) || !(asset3 is Texture2D texture3))
                            {
                                LogError("找不到角色图片: "+"assets/UI/BtnPic/" + btnPickPicPath + ".png");
                            }
                            else{
                                /*LogError("找到角色图片: "+"assets/" + Unit_.Pic + ".png");
                               var sa = newUnit.transform.Find("Spine").gameObject.GetComponent<SkeletonAnimation>();
                               GameObject.DestroyImmediate(sa);
                               var mf = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshFilter>();
                               GameObject.DestroyImmediate(mf);
                               var mr = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshRenderer>();
                               GameObject.DestroyImmediate(mr);*/
                                btnPickPicSprite = Main.Res.GetSpriteCache(texture3);
                            }
                            if (!Main.Res.TryGetAsset("assets/UI/BtnPic/" + btnUnpickPath + ".png", out var asset4) || !(asset4 is Texture2D texture4))
                            {
                                LogError("找不到角色图片: "+"assets/UI/BtnPic/" + btnUnpickPath + ".png");
                            }
                            else{
                                /*LogError("找到角色图片: "+"assets/" + Unit_.Pic + ".png");
                               var sa = newUnit.transform.Find("Spine").gameObject.GetComponent<SkeletonAnimation>();
                               GameObject.DestroyImmediate(sa);
                               var mf = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshFilter>();
                               GameObject.DestroyImmediate(mf);
                               var mr = newUnit.transform.Find("Spine").gameObject.GetComponent<MeshRenderer>();
                               GameObject.DestroyImmediate(mr);*/
                                btnUnpickPicSprite = Main.Res.GetSpriteCache(texture4);
                            }

                             
                        
                    }
                   
                  
               //     var ska = clone立绘.GetComponent<SkeletonAnimation>();



                    //SkeletonAnimation sda = clone立绘.GetComponent<SkeletonAnimation>();
                    
                    var clonebtn = GameObject.Instantiate(btn, btn.transform.parent);
                    clonebtn.transform.localPosition = btn.transform.localPosition + new Vector3(oldValue * 1.15f, 0f, 0f);
                    clonebtn.gameObject.name = Hero_.id;
                    clonebtn.transform.Find("Picked").GetComponent<SpriteRenderer>().sprite = btnPickPicSprite;
                    clonebtn.transform.Find("Unpicked").GetComponent<SpriteRenderer>().sprite = btnUnpickPicSprite;
                    HeroBtns.Add(clonebtn);
                    oldValue += 1;
                    
                    var cloneNameGameObject = GameObject.Instantiate(名字GameObject, 名字GameObject.transform.parent);
                    
                    cloneNameGameObject.gameObject.name = Hero_.id;
                  //  cloneNameGameObject.transform
                  DeckBuildingGame.Utils.SetText(cloneNameGameObject, Hero_.id);
                  LogInfo("增加了选人界面按钮:"+ Hero_.id);
                  for (int i = 0;i< cloneNameGameObject.transform.childCount;i++)
                  {
                      cloneNameGameObject.transform.GetChild(i).gameObject.SetActive(false);
                  }
                  cloneNameGameObject.SetActive(false);
                  cloneNameGameObject.transform.parent = 名字GameObject.transform.parent;
                  cloneNameGameObject.transform.localPosition = new Vector3(namePicLocX, namePicLocY,
                      cloneNameGameObject.transform.localPosition.z);
                  cloneNameGameObject.transform.localScale = new Vector3(namePicSizeX, namePicSizeY,
                      cloneNameGameObject.transform.localPosition.z);
                  cloneNameGameObject.transform.Find("莫三中文").GetComponent<SpriteRenderer>().sprite = namePicSprite;
                  cloneNameGameObject.transform.Find("莫三英文").GetComponent<SpriteRenderer>().sprite = namePicSprite;
                }
            }
        

        /*foreach (var pair  in ModManager.ModSKeletonDataCache)
        {
            foreach (var sda in pair.Value)
            {
                var ska =  __instance.HeroArt.transform.Find("侍卫mod").gameObject.GetComponent<SkeletonAnimation>(); 
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");#1#
                Utils.ChangeSkeletonDataAssetRuntime(sda,ska);
           //     Main.LogInfo("替换了 选人界面立绘: "+ pair.Key);
            }
        }*/
        
      //  return true;
    }

    /// <summary>
    /// 此方法原用于初始化时根据不同职业修改UI上的特效 因此补丁补充了Mod角色使用莫三的特效的规则
    /// </summary>
    /// <returns></returns>
    /*[HarmonyPrefix]
    [HarmonyPatch(typeof(ClassSpriteController), nameof(ClassSpriteController.SetClass))]
    public static bool UseDefaultCLassSprite(ref string className)
    {
        if (className != "侍卫" || className != "道士" || className != "巫女" || className != "龙妹")
            className = "侍卫";
        
        return true;
    }*/
    
    
    /// <summary>
///  修改switchHero
/// </summary>
/// <param name="type"></param>
/// <param name="__result"></param>
/// <returns></returns>
/*[HarmonyPrefix]
[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.InitHeroPage))]
public static bool switchHeroPagePatch(ref GameObject btnClicked, ref bool iSInital, ref string __result)
{
    return true;
}*/

    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Battle), nameof(Battle.InitCardType))]
    public static bool InitCardTypePatch(ref string type, ref Card __result)
    {
        if (type == null)
            return true;
        if (ModManager.ModTypeCache.TryGetValue(type, out var t))
        {
             Card card = Activator.CreateInstance(t) as Card;
             __result = card; 
             return false;
        }
        else
        {
            return true;
        }
    }

[HarmonyPrefix]
[HarmonyPatch(typeof(DeckBuildingGame.GameManager), nameof(DeckBuildingGame.GameManager.HeroIdxById))]
public static bool changeHeroIdxById(ref string heroId, ref int __result)
{
    if (heroId == "侍卫")
        __result = 0;
    else if (heroId == "道士")
        __result = 1;
    else if (heroId == "巫女")
        __result = 2;
    else if (heroId == "龙妹")
        __result = 3;
    else 
        __result = -1;
    return false;
}
    
    public static void LogInfo(object obj) => Main.I.Logger.LogInfo((object) string.Format("{0}{1}", (object) Main.GetIndent(), obj));

    public static void LogWarning(object obj) => Main.I.Logger.LogWarning((object) string.Format("{0}{1}", (object) Main.GetIndent(), obj));

    public static void LogError(object obj) => Main.I.Logger.LogError((object) string.Format("{0}{1}", (object) Main.GetIndent(), obj));
    

    private static string GetIndent()
    {
        if (Main.LogIndent <= 0)
            return string.Empty;
        StringBuilder stringBuilder = new StringBuilder(Main.LogIndent * 4);
        for (int index = 0; index < Main.LogIndent * 4; ++index)
            stringBuilder.Append(' ');
        return stringBuilder.ToString();
    }
    
    
}

