using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class ImageLoader
{
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        return LoadTextureFromBytes(bytes);
    }

    public static Texture2D LoadTextureFromBytes(byte[] bytes)
    {
        Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);

        tex.LoadImage(bytes);
        return tex;
    }

    public static Sprite LoadSpriteFromBytes(byte[] bytes)
    {
        Texture2D tex = LoadTextureFromBytes(bytes);
        return LoadSpriteFromTexture(tex);
    }

    public static Sprite LoadSpriteFromTexture(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
    }

    public static Sprite LoadSpriteFromFile(string filePath)
    {
        Texture2D tex = LoadTextureFromFile(filePath);
        return LoadSpriteFromTexture(tex);
    }
}
