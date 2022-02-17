using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public struct PrimitiveFileImage
{
    public byte[] imgBytes;
    public string fileName;
    public DateTime creationDate;
}

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

    public Queue<PrimitiveFileImage> queue = new Queue<PrimitiveFileImage>();

    public string BaseDataPath { get; private set; }

    public event Action<string> OnRefresh = delegate { };

    private object locker = new object();

    private FileImageObject[] fileImages = null;

    private int pointer = 0;

    protected void Awake()
    {
        BaseDataPath = Path.Combine(Application.dataPath, chosenFolderName);
    }

    protected void Start()
    {
        Refresh();
    }

    protected void Update()
    {
        lock (locker)
        {
            if (queue.Count > 0)
            {
                PrimitiveFileImage file = queue.Dequeue();
                fileImages[pointer++].Initialize(
                   file.fileName,
                   FileImageLoader.LoadSpriteFromBytes(file.imgBytes),
                   file.creationDate.ToString());
            }
        }
    }

    public void Refresh()
    {
        string errorMessage = null;

        if (!Directory.Exists(BaseDataPath))
        {
            Directory.CreateDirectory(BaseDataPath);
        }

        fileImageParent.KillAllChildren();

        int len = Directory.GetFiles(BaseDataPath, $"*.{chosenFileType}").Length;

        fileImages = new FileImageObject[len];

        for (int i = 0; i < len; i++)
        {
            fileImages[i] = Instantiate(fileImagePrefab, fileImageParent);
        }

        Thread thread = new Thread(() =>
        {
            IEnumerable<string> allFiles = Directory.EnumerateFiles(BaseDataPath, $"*.{chosenFileType}");
            foreach (string specificFilePath in allFiles)
            {
                PrimitiveFileImage primitiveFileImage = new PrimitiveFileImage();
                primitiveFileImage.imgBytes = File.ReadAllBytes(specificFilePath);
                primitiveFileImage.fileName = Path.GetFileNameWithoutExtension(specificFilePath);
                primitiveFileImage.creationDate = File.GetCreationTime(specificFilePath);
                lock (locker)
                {
                    queue.Enqueue(primitiveFileImage);
                }
            }
        });

        thread.Start();
        


        OnRefresh(errorMessage);
    }
}
