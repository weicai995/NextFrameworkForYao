using HarmonyLib;
using UnityEngine;

namespace NextFrameworkForYao.Patch;

/*public class LoadTexturePatch
{
    public static void LoadTexture2D(string path)
    {
        Debug.Log("1: "+path);
        Object asset;
        if (!Main.Res.TryGetAsset("Assets/" + path + ".png", out asset) || !(asset is Texture2D texture2D))
        {
            Debug.Log("找到图片");
        }
        else
        {
            Debug.Log("没有找到图片");
        }
      
    }*/
    
    


    /// <summary>游戏资源加载通用Patch</summary>
    [HarmonyPatch(typeof (ResHelper), "GetSpriteAsset")]
    public class ModResourcesLoadSpritePatch
    {
        [HarmonyPrefix]
        public static bool LoadSprite(string path, ref UnityEngine.Sprite __result)
        {
            Object asset;
            if (!Main.Res.TryGetAsset("assets/" + path.TrimStart("CardArts/".ToCharArray()) + ".png", out asset) || !(asset is Texture2D texture)){
                /*foreach (var VARIABLE in Main.Res.fileAssets.Keys)
                {
                    Debug.Log("assets/" + path.TrimStart("CardArts/".ToCharArray()) + ".png");
                    Debug.Log(VARIABLE);
                }
                Debug.Log("没找到");*/
                return true;
            }
            __result = Main.Res.GetSpriteCache(texture);
            return false;
        }
    }