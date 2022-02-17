using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileImageLoader : MonoBehaviour
{
    [SerializeField]
    private string chosenFileType = "png";

    [SerializeField]
    private string chosenFolderName = "Gallery";

    [SerializeField]
    private FileImageContainer fileImagePrefab = null;

    [SerializeField]
    private Transform fileImageParent = null;

    protected void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        string basePath = Path.Combine(Application.dataPath, chosenFolderName);

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        fileImageParent.KillAllChildren();

        foreach (string specificFilePath in Directory.EnumerateFiles(basePath, $"*.{chosenFileType}"))
        {
            FileImageContainer fileImageContainer = Instantiate(fileImagePrefab, fileImageParent);
            fileImageContainer.Initialize(Path.GetFileNameWithoutExtension(specificFilePath), 
                LoadSpriteFromFile(specificFilePath), 
                File.GetCreationTime(specificFilePath).ToString());
        }

    }

    public Texture2D LoadTextureFromFile(string filePath)
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

    public Sprite LoadSpriteFromFile(string filePath)
    {
        Texture2D tex = LoadTextureFromFile(filePath);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
    }
}
