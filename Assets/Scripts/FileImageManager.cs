using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileImageManager : MonoBehaviour
{
    [SerializeField]
    private string chosenFileType = "png";

    [SerializeField]
    private string chosenFolderName = "Gallery";

    [SerializeField]
    private FileImageObject fileImagePrefab = null;

    [SerializeField]
    private Transform fileImageParent = null;

    public string BaseDataPath { get; private set; }

    public event Action<string> OnRefresh = delegate { };

    protected void Awake()
    {
        BaseDataPath = Path.Combine(Application.dataPath, chosenFolderName);
    }

    protected void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        string errorMessage = null;

        if (!Directory.Exists(BaseDataPath))
        {
            Directory.CreateDirectory(BaseDataPath);
        }

        fileImageParent.KillAllChildren();

        foreach (string specificFilePath in Directory.EnumerateFiles(BaseDataPath, $"*.{chosenFileType}"))
        {
            FileImageObject fileImageContainer = Instantiate(fileImagePrefab, fileImageParent);
            fileImageContainer.Initialize(Path.GetFileNameWithoutExtension(specificFilePath),
                FileImageLoader.LoadSpriteFromFile(specificFilePath),
                File.GetCreationTime(specificFilePath).ToString());
        }

        OnRefresh(errorMessage);

    }
}
