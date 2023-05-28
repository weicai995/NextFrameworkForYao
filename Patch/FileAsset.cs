using NextFrameworkForYao.Res;

namespace NextFrameworkForYao.Patch;

using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileAsset
  {
    public UnityEngine.Object asset;
    private string filePath;
    private bool isDone;
    private bool isLoading;

    public event Action<UnityEngine.Object> OnLoadedAsset;

    public FileAsset(string path) => this.filePath = path;

    public bool IsDone => this.isDone;

    public bool IsLoading => this.isLoading;

    public string FileRawPath => this.filePath;

    public void LoadAssetAsync(Action<UnityEngine.Object> callback)
    {
      if (!this.isDone)
      {
        this.OnLoadedAsset += callback;
        this.LoadAsync().Forget();
      }
      else
      {
        if (callback == null)
          return;
        callback(this.asset);
      }
    }

    public UnityEngine.Object LoadAsset()
    {
      if (this.isDone)
        return this.asset;
      try
      {
        this.Load();
      }
      catch (Exception ex)
      {
        Main.LogError((object) ex);
        return (UnityEngine.Object) null;
      }
      return this.asset;
    }

    private async UniTaskVoid LoadAsync()
    {
      if (this.isLoading || this.isDone)
        return;
      this.isLoading = true;
      UnityEngine.Object @object = (UnityEngine.Object) null;
      string extension = Path.GetExtension(this.filePath);
      try
      {
        UnityWebRequest webRequest;
        switch (extension)
        {
          case ".jpg":
          case ".png":
            webRequest = UnityWebRequestTexture.GetTexture(this.filePath);
            try
            {
              UnityWebRequest unityWebRequest = webRequest.SendWebRequest().webRequest;
              Texture2D content = DownloadHandlerTexture.GetContent(webRequest);
              content.hideFlags = HideFlags.HideAndDontSave;
              @object = (UnityEngine.Object) content;
            }
            finally
            {
              ((IDisposable) webRequest)?.Dispose();
            }
            webRequest = (UnityWebRequest) null;
            break;
          case ".mp3":
            webRequest = UnityWebRequestMultimedia.GetAudioClip(this.filePath, AudioType.UNKNOWN);
            try
            {
              UnityWebRequest unityWebRequest =  webRequest.SendWebRequest().webRequest;
              @object = (UnityEngine.Object) DownloadHandlerAudioClip.GetContent(webRequest);
            }
            finally
            {
              ((IDisposable) webRequest)?.Dispose();
            }
            webRequest = (UnityWebRequest) null;
            break;
          case ".ab":
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(this.filePath);
            try
            {
              UnityWebRequest unityWebRequest = webRequest.SendWebRequest().webRequest;
              @object = (UnityEngine.Object) DownloadHandlerAssetBundle.GetContent(webRequest);
            }
            finally
            {
              ((IDisposable) webRequest)?.Dispose();
            }
            webRequest = (UnityWebRequest) null;
            break;
          case ".bytes":
            @object = (UnityEngine.Object) new BytesAsset(File.ReadAllBytes(this.filePath));
            break;
        }
      }
      catch (Exception ex)
      {
        Main.LogError((object) ex);
        Debug.LogWarning((object) (this.filePath + "加载失败"));
        this.isLoading = false;
        this.isDone = false;
        return;
      }
      if (!this.isDone && @object != (UnityEngine.Object) null)
        this.asset = @object;
      if (this.asset != (UnityEngine.Object) null)
      {
        this.asset.name = this.filePath;
        Action<UnityEngine.Object> onLoadedAsset = this.OnLoadedAsset;
        if (onLoadedAsset != null)
          onLoadedAsset(this.asset);
        this.isLoading = false;
        this.isDone = true;
      }
      else
      {
        Main.LogWarning((object) (this.filePath + "加载失败"));
        this.isLoading = false;
        this.isDone = false;
      }
    }

    private void Load()
    {
      UnityEngine.Object @object = (UnityEngine.Object) null;
      switch (Path.GetExtension(this.filePath))
      {
        case ".jpg":
        case ".png":
          byte[] data = File.ReadAllBytes(this.filePath);
          Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
          tex.hideFlags = HideFlags.HideAndDontSave;
          tex.LoadImage(data);
          @object = (UnityEngine.Object) tex;
          break;
        case ".mp3":
          using (UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip(this.filePath, AudioType.UNKNOWN))
          {
            audioClip.SendWebRequest();
            do
              ;
            while (!audioClip.isDone);
            @object = (UnityEngine.Object) DownloadHandlerAudioClip.GetContent(audioClip);
            break;
          }
        case ".ab":
          this.asset = (UnityEngine.Object) AssetBundle.LoadFromFile(this.filePath);
          break;
        case ".bytes":
          @object = (UnityEngine.Object) new BytesAsset(File.ReadAllBytes(this.filePath));
          break;
      }
      if (!(@object != (UnityEngine.Object) null))
        return;
      this.asset = @object;
      this.isDone = true;
    }
  }