using System.IO;
using UnityEngine;
using UnityEngine.UI;

public static class FileImageLoader
{
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(0, 0); // parameters here don't matter, because of loading image in the next step
            tex.LoadImage(bytes);
            return tex;
        }

        return null;
    }

    public static Sprite LoadSpriteFromFile(string filePath)
    {
        Texture2D tex = LoadTextureFromFile(filePath);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
    }
}
