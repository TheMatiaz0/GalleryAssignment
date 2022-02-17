using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FileImageManager : MonoBehaviour
{
    [Header("Required")]
    [SerializeField]
    private FileImageObject fileImagePrefab = null;

    [SerializeField]
    private Transform fileImageParent = null;


    [Header("Further Task Customization")]
    [SerializeField]
    private string chosenFileExtension = "png";

    [SerializeField]
    private string chosenDirectoryName = "Gallery";

    private Queue<FileImage> ConcurrentQueue { get; } = new Queue<FileImage>(); // tried using ConcurrentQueue class, but couldn't figure it out so I used locks

    public string BaseDataPath { get; private set; }
    public string BaseSearchPattern { get; private set; }
    public EnumerationOptions BaseEnumerationOptions { get; private set; }

    public event Action<string> OnRefresh = delegate { };

    private object EnumerateLocker { get; } = new object();

    private FileImageObject[] fileImages = null;

    private int pointer = 0;
    private bool isRefreshing;

    private Thread directoryEnumerateThread;

    protected void Awake()
    {
        BaseDataPath = Path.Combine(Application.dataPath, chosenDirectoryName);
        BaseSearchPattern = $"*.{chosenFileExtension}";
        BaseEnumerationOptions = new EnumerationOptions()
        {
            RecurseSubdirectories = true
        };
    }

    protected void Start()
    {
        Refresh();
    }

    protected void Update()
    {
        lock (EnumerateLocker)
        {
            if (ConcurrentQueue.Count > 0)
            {
                FileImage fileImg = ConcurrentQueue.Dequeue();

                FileImageObject createdObject = fileImages[pointer++];
                createdObject.Initialize(
                   fileImg.FileName,
                   ImageLoader.LoadTextureFromBytes(fileImg.ImgBytes),
                   fileImg.FileCreationDate.ToString());
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

    private void DefaultStartSetup()
    {
        Directory.CreateDirectory(BaseDataPath);

        foreach (PlaceholderFileImage fileImg in PlaceholderContainer.Instance.PlaceholderFiles)
        {
            string creationPath = Path.Combine(BaseDataPath, $"{fileImg.FileName}.{chosenFileExtension}");
            byte[] imgBytes = fileImg.Texture.EncodeToPNG();
            using (var fs = new FileStream(creationPath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(imgBytes, 0, imgBytes.Length);
            }
        }

    }

    public void Refresh()
    {
        if (isRefreshing == true || ConcurrentQueue.Count > 0)
        {
            return;
        }

        isRefreshing = true;
        pointer = 0;
        string errorMessage = null;

        if (!Directory.Exists(BaseDataPath))
        {
            DefaultStartSetup();
        }

        fileImageParent.KillAllChildren();

        int fileCount = Directory.GetFiles(BaseDataPath, BaseSearchPattern).Length;
        fileImages = new FileImageObject[fileCount];
        BaseRefresh(fileCount);

        directoryEnumerateThread = EnumerateDirectoryThread();
        directoryEnumerateThread.Start();
        
        OnRefresh(errorMessage);
    }

    private void BaseRefresh(int fileCount)
    {
        for (int i = 0; i < fileCount; i++)
        {
            FileImageObject fileImgObj = Instantiate(fileImagePrefab, fileImageParent);
            fileImgObj.Initialize();
            fileImages[i] = fileImgObj;
        }
    }

    private Thread EnumerateDirectoryThread()
    {
        return new Thread(() =>
        {
            IEnumerable<string> allFilePaths = Directory.EnumerateFiles(BaseDataPath,
                BaseSearchPattern, BaseEnumerationOptions);

            foreach (string specificFilePath in allFilePaths)
            {
                FileImage fileImage = new FileImage(
                    File.ReadAllBytes(specificFilePath),
                    Path.GetFileNameWithoutExtension(specificFilePath),
                    File.GetCreationTime(specificFilePath)
                );

                lock (EnumerateLocker)
                {
                    ConcurrentQueue.Enqueue(fileImage);
                }
            }

            isRefreshing = false;
        });
    }
}
