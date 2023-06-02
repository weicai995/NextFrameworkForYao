using System.Collections.Generic;
using HarmonyLib;
using Spine.Unity;
using UnityEngine;

namespace NextFrameworkForYao.Patch;

public class LoadUnitPrefabPatch
{

    public static Dictionary<string, GameObject> UnitPrefabCacche = new Dictionary<string, GameObject>();
    
    /// <summary>游戏资源加载UnitPrefabPatch</summary>
    [HarmonyPatch(typeof (ResHelper), nameof(ResHelper.GetUnitPrefab))]
    public class ModResourcesLoadSpritePatch
    {
        [HarmonyPrefix]
        public static bool LoadSprite(string path, ref GameObject __result)
        {
            GameObject asset;
            
            if (!UnitPrefabCacche.TryGetValue(path, out asset)){
                /*foreach (var VARIABLE in Main.Res.fileAssets.Keys)
                {
                    Debug.Log("assets/" + path.TrimStart("CardArts/".ToCharArray()) + ".png");
                    Debug.Log(VARIABLE);
                }
                Debug.Log("没找到");*/
                Debug.LogError("没找到"+path);
                return true;
            }
            Debug.LogError("找到了"+path);
            __result = GameObject.Instantiate(UnitPrefabCacche[path]);
            return false;
        }
    }
}