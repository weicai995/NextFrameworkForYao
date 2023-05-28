using System;
using System.IO;
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
        /*var Player= (DeckBuildingGame.Unit) Traverse.Create(__instance).Field("Player").GetValue();
        Player.UnitView.transform.Find("Avatar").Find("Spine").GetComponent<Spine.Unity.SkeletonAnimation>();#1#
        foreach (var pair  in ModManager.ModSKeletonDataCache)
        {
            foreach (var skeletonData in pair.Value)
            {
                /*var NameSpace = "firstPlugin";
                Type t = Type.GetType($"{NameSpace}.{card_.Type}");#1#
                DataManager.Instance.导入Mod卡(skeletonData);
            }
        }
    }*/
    
   
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DataManager), nameof(DataManager.Init))]
    private static void AddCard()
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
        /*Logger.LogInfo(dataList.Length);*/
    }
    
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
