using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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

    public Queue<FileImage> queue = new Queue<FileImage>();

    public string BaseDataPath { get; private set; }

    public event Action<string> OnRefresh = delegate { };

    private object locker = new object();

    private FileImageObject[] fileImages = null;

    private int pointer = 0;
    private bool isRefreshing;

    private Thread directoryEnumerateThread;

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
                FileImage fileImg = queue.Dequeue();

                FileImageObject createdObject = fileImages[pointer++];
                createdObject.Initialize(
                   fileImg.fileName,
                   FileImageLoader.LoadTextureFromBytes(fileImg.imgBytes),
                   fileImg.creationDate.ToString());
            }
        }
    }

    protected void OnDestroy()
    {
        if (directoryEnumerateThread != null)
        {
            directoryEnumerateThread.Abort();
        }
    }

    public void Refresh()
    {
        if (isRefreshing == true || queue.Count > 0)
        {
            return;
        }

        isRefreshing = true;
        pointer = 0;
        string errorMessage = null;

        if (!Directory.Exists(BaseDataPath))
        {
            Directory.CreateDirectory(BaseDataPath);
        }

        fileImageParent.KillAllChildren();

        int fileCount = Directory.GetFiles(BaseDataPath, $"*.{chosenFileType}").Length;

        fileImages = new FileImageObject[fileCount];

        for (int i = 0; i < fileCount; i++)
        {
            FileImageObject fileImgObj = Instantiate(fileImagePrefab, fileImageParent);
            fileImgObj.Initialize();
            fileImages[i] = fileImgObj;
        }

        directoryEnumerateThread = new Thread(() =>
        {
            IEnumerable<string> allFiles = Directory.EnumerateFiles(BaseDataPath, 
                $"*.{chosenFileType}", 
                new EnumerationOptions() 
                { 
                    RecurseSubdirectories = true 
                });

            foreach (string specificFilePath in allFiles)
            {
                FileImage fileImage = new FileImage(
                    File.ReadAllBytes(specificFilePath),
                    Path.GetFileNameWithoutExtension(specificFilePath),
                    File.GetCreationTime(specificFilePath)
                );

                lock (locker)
                {
                    queue.Enqueue(fileImage);
                }
            }

            isRefreshing = false;
        });

        directoryEnumerateThread.Start();
        
        OnRefresh(errorMessage);
    }
}
