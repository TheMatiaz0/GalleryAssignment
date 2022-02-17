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

    private object EnumerateLocker { get; } = new object();

    private FileImageObject[] fileImages = null;

    private int pointer = 0;
    private bool isRefreshing;

    private Thread enumerateThread = null;

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
        if (enumerateThread != null)
        {
            enumerateThread.Abort();
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

        StatusContainer.Instance.SetupStatus(BaseDataPath);

        isRefreshing = true;
        pointer = 0;

        if (!Directory.Exists(BaseDataPath))
        {
            DefaultStartSetup();
        }

        fileImageParent.KillAllChildren();

        int fileCount = Directory.GetFiles(BaseDataPath, BaseSearchPattern).Length;
        fileImages = new FileImageObject[fileCount];


        enumerateThread = EnumerateDirectoryThread();
        BaseRefresh(fileCount);
        enumerateThread.Start();
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
            foreach (string specificFilePath in Directory.EnumerateFiles(BaseDataPath,
                BaseSearchPattern, BaseEnumerationOptions))
            {
                if (!File.Exists(specificFilePath))
                {
                    StatusContainer.Instance.ThrowError($"Could not find file \"{specificFilePath}\". File was probably removed during refresh.");
                    continue;
                }
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
