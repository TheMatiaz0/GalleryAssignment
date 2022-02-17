using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class FileImageLoader
{
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        return LoadTextureFromBytes(bytes);
    }

    public static Texture2D LoadTextureFromBytes(byte[] bytes)
    {
        Texture2D tex = new Texture2D(0, 0); // parameters here don't matter, because of loading image in the next step
        tex.LoadImage(bytes);
        return tex;
    }

    public static Sprite LoadSpriteFromBytes(byte[] bytes)
    {
        Texture2D tex = LoadTextureFromBytes(bytes);
        return LoadSprite(tex);
    }

    private static Sprite LoadSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
    }

    public static Sprite LoadSpriteFromFile(string filePath)
    {
        Texture2D tex = LoadTextureFromFile(filePath);
        return LoadSprite(tex);
    }
}
