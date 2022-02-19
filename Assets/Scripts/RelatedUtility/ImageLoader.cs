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
        const int TEXTURE_WIDTH = 256;
        const int TEXTURE_HEIGHT = TEXTURE_WIDTH;
        const TextureFormat TEXTURE_FORMAT = TextureFormat.RGBA32;
        const bool ENABLE_MIP_CHAIN = false;

        Texture2D tex = new Texture2D(TEXTURE_WIDTH, TEXTURE_HEIGHT, TEXTURE_FORMAT, ENABLE_MIP_CHAIN);

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
        const float PIVOT_X = 0.5f;
        const float PIVOT_Y = 0.5f;
        const float PIXELS_PER_UNIT = 100f;

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(PIVOT_X, PIVOT_Y), PIXELS_PER_UNIT);
        return sprite;
    }

    public static Sprite LoadSpriteFromFile(string filePath)
    {
        Texture2D tex = LoadTextureFromFile(filePath);
        return LoadSpriteFromTexture(tex);
    }
}
