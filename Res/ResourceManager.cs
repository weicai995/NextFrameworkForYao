using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextFrameworkForYao.Patch;
using UnityEngine;

namespace NextFrameworkForYao.Res
{
  public class ResourcesManager : MonoBehaviour
  {
    
    public Dictionary<string, FileAsset> fileAssets = new Dictionary<string, FileAsset>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public Dictionary<int, Sprite> spriteCache = new Dictionary<int, Sprite>();

    public void Init() => this.LoadInnerAsset();

    public void Reset()
    {
      this.fileAssets.Clear();
      this.spriteCache.Clear();
      this.LoadInnerAsset();
    }

    public void LoadInnerAsset() => this.CacheAssetDir(Main.PathInnerAssetDir.Value);

    public void CacheAssetDir(string rootPath) => this.DirectoryHandle("Assets", rootPath, new ResourcesManager.FileHandle(this.AddAsset));

    public void DirectoryHandle(
      string rootPath,
      string dirPath,
      ResourcesManager.FileHandle fileHandle)
    {
      
      Debug.Log("Here: "+dirPath);
      if (!Directory.Exists(dirPath))
        return;
      foreach (string directory in Directory.GetDirectories(dirPath))
      {
        string withoutExtension = Path.GetFileNameWithoutExtension(directory);
        this.DirectoryHandle(rootPath + "/" + withoutExtension, directory, fileHandle);
      }
      foreach (string file in Directory.GetFiles(dirPath))
      {
        string fileName = Path.GetFileName(file);
        string virtualPath = rootPath + "/" + fileName;
        fileHandle(virtualPath, file);
      }
    }

    /// <summary>异步加载资源，返回值表示资源是否存在</summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public bool TryGetAsset(string path, Action<UnityEngine.Object> callback)
    {
      FileAsset fileAsset;
      if (!this.fileAssets.TryGetValue(path, out fileAsset))
        return false;
      fileAsset.LoadAssetAsync(callback);
      return true;
    }

    public bool TryGetAsset<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
      FileAsset fileAsset;
      if (!this.fileAssets.TryGetValue(path, out fileAsset))
        return false;
      fileAsset.LoadAssetAsync((Action<UnityEngine.Object>) (asset => callback(asset as T)));
      return true;
    }

    /// <summary>同步加载资源，返回值表示资源是否存在且加载完毕</summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool TryGetAsset(string path, out UnityEngine.Object asset)
    {
      FileAsset fileAsset;
      if (this.fileAssets.TryGetValue(path, out fileAsset))
      {
        asset = fileAsset.LoadAsset();
        return asset != (UnityEngine.Object) null;
      }
      asset = (UnityEngine.Object) null;
      return false;
    }

    /// <summary>同步加载泛型接口</summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool TryGetAsset<T>(string path, out T asset) where T : UnityEngine.Object
    {
      UnityEngine.Object asset1;
      if (this.TryGetAsset(path, out asset1) && asset1 is T obj)
      {
        asset = obj;
        return true;
      }
      asset = default (T);
      return false;
    }

    /// <summary>获取FileAsset，可以通过FileAsset获取资源原始位置及加载信息等</summary>
    /// <param name="path"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool TryGetFileAsset(string path, out FileAsset fileAsset) => this.fileAssets.TryGetValue(path, out fileAsset);

    /// <summary>异步按文件夹加载资源</summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <param name="includeSubfolder">是否包含子文件夹</param>
    /// <param name="extension">文件拓展名</param>
    /// <returns></returns>
    public void TryGetAssets(
      string path,
      Action<UnityEngine.Object[]> callback,
      bool includeSubfolder = false,
      string extension = ".*")
    {
      List<FileAsset> assets = new List<FileAsset>();
      string lower = path.ToLower();
      foreach (KeyValuePair<string, FileAsset> fileAsset1 in this.fileAssets)
      {
        string key = fileAsset1.Key;
        FileAsset fileAsset2 = fileAsset1.Value;
        if (key.StartsWith(lower) && (includeSubfolder || !key.Substring(lower.Length).Contains("/")) && (extension == ".*" || key.EndsWith(extension)))
          assets.Add(fileAsset2);
      }
      this.StartCoroutine(this.LoadAssets((IEnumerable<FileAsset>) assets, callback));
    }

    /// <summary>
    /// 同步按文件夹加载资源
    /// 如果一个资源没有准备就绪，不会被加载进来
    /// </summary>
    /// <param name="path"></param>
    /// <param name="asstes"></param>
    /// <param name="includeSubfolder"></param>
    /// <param name="extension"></param>
    public void TryGetAssets(
      string path,
      out UnityEngine.Object[] asstes,
      bool includeSubfolder = false,
      string extension = ".*")
    {
      List<FileAsset> source = new List<FileAsset>();
      string lower = path.ToLower();
      foreach (KeyValuePair<string, FileAsset> fileAsset1 in this.fileAssets)
      {
        string key = fileAsset1.Key;
        FileAsset fileAsset2 = fileAsset1.Value;
        if (key.StartsWith(lower) && (includeSubfolder || !key.Substring(lower.Length).Contains("/")) && (extension == ".*" || key.EndsWith(extension)))
          source.Add(fileAsset2);
      }
      asstes = source.Where<FileAsset>((Func<FileAsset, bool>) (asset => asset.IsDone)).Select<FileAsset, UnityEngine.Object>((Func<FileAsset, UnityEngine.Object>) (asset => asset.LoadAsset())).ToArray<UnityEngine.Object>();
    }

    private IEnumerator LoadAssets(IEnumerable<FileAsset> assets, Action<UnityEngine.Object[]> callback)
    {
      List<FileAsset> loadAssets = new List<FileAsset>();
      List<FileAsset> doneAssets = new List<FileAsset>();
      loadAssets.AddRange(assets);
      while (doneAssets.Count < loadAssets.Count)
      {
        for (int index = loadAssets.Count - 1; index >= 0; --index)
        {
          FileAsset fileAsset = loadAssets[index];
          if (fileAsset.IsDone)
          {
            loadAssets.RemoveAt(index);
            doneAssets.Add(fileAsset);
          }
        }
        yield return (object) null;
      }
      Action<UnityEngine.Object[]> action = callback;
      if (action != null)
        action(doneAssets.Select<FileAsset, UnityEngine.Object>((Func<FileAsset, UnityEngine.Object>) (asset => asset.asset)).ToArray<UnityEngine.Object>());
    }

    /// <summary>是否存在资源（无论是否加载完毕）</summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool HaveAsset(string path) => this.fileAssets.ContainsKey(path);

    /// <summary>是否存在资源且加载完毕</summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsAssetReady(string path)
    {
      FileAsset fileAsset;
      return this.fileAssets.TryGetValue(path, out fileAsset) && fileAsset.IsDone;
    }

    public void AddAsset(string cachePath, string path)
    {
      FileAsset fileAsset = new FileAsset(path);
      string lower = cachePath.Replace("\\\\", "/").ToLower();
      if (this.fileAssets.TryGetValue(lower, out FileAsset _))
        Main.LogWarning((object) ("重复添加Asset (" + lower + ")"));
      else
        Main.LogInfo((object) ("添加Asset (" + lower + ")"));
      this.fileAssets[lower] = fileAsset;
    }

    public Sprite GetSpriteCache(Texture2D texture, string spriteType = "CardView")
    {
      Sprite spriteCache;
      if (!this.spriteCache.TryGetValue(((object) texture).GetHashCode(), out spriteCache))
      {
        float width = 351f;
        float height = 253f;
        switch (spriteType)
        {
          case "Skills":
          {
            width = 140f;
            height = 140f;
            break;
          }
          case "TalentPics":
          {
            width = 120f;
            height = 120f;
            break;
          }
        }
        
        spriteCache = Sprite.Create(texture, new Rect(0.0f, 0.0f, width, (float)height), new Vector2(0.5f, 0.5f));
        this.spriteCache.Add(((object) texture).GetHashCode(), spriteCache);
      }
      return spriteCache;
    }

    private void DrawWindow(int id)
    {
      int count = this.fileAssets.Count;
      GUILayout.Label(string.Format("资源加载进度：{0} / {1}", (object) this.fileAssets.Count<KeyValuePair<string, FileAsset>>((Func<KeyValuePair<string, FileAsset>, bool>) (pair => pair.Value.IsDone)), (object) count));
    }

    public delegate void FileHandle(string virtualPath, string filePath);
  }
}
