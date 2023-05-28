using Spine;
using UnityEngine;

namespace NextFrameworkForYao.Tool;

public class SpineTextureLoader : TextureLoader
{
    public void Load (AtlasPage page, string path)
    {
        if (Main.Res.TryGetAsset(path, out Texture asset))
        {
            page.rendererObject = asset;
        }
        
    }

    public void Unload(object texture)
    {
        GameObject.Destroy((Texture)texture);
    }
}