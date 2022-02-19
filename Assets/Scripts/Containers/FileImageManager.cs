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
    private FileImageUnityObject fileImagePrefab = null;

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

    private bool isRefreshing;

    private Thread enumerateThread = null;

    private FileImage[] fileImgs = null;

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
                FileImage fileImgFromQueue = ConcurrentQueue.Dequeue();

                if (fileImgFromQueue == null)
                {
                    return;
                }

                FileImageUnityObject createdObject = fileImgFromQueue.UnityObject;

                createdObject.Initialize(
                   fileImgFromQueue.FileName,
                   ImageLoader.LoadTextureFromBytes(fileImgFromQueue.ImgBytes),
                   fileImgFromQueue.FileCreationDate.ToString());
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
            using (FileStream fs = new FileStream(creationPath, FileMode.Create, FileAccess.Write))
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

        if (!Directory.Exists(BaseDataPath))
        {
            DefaultStartSetup();
        }

        fileImageParent.KillAllChildren();

        string[] filePaths = Directory.GetFiles(
            BaseDataPath,
            BaseSearchPattern,
            BaseEnumerationOptions);

        fileImgs = new FileImage[filePaths.Length];
        ConvertPathsToData(filePaths);

        enumerateThread = EnumerateDirectoryThread(fileImgs);
        enumerateThread.Start();
    }

    private void ConvertPathsToData(string[] filePaths)
    {
        for (int i = 0; i < filePaths.Length; i++)
        {
            string path = filePaths[i];

            byte[] byteData = File.ReadAllBytes(path);

            if (ImageHeaderChecker.GetLiteralExtensionFromType(byteData) != $".{chosenFileExtension}")
            {
                continue;
            }

            FileImageUnityObject fileImgUnityObject = Instantiate(fileImagePrefab, fileImageParent);

            FileImage fileImgObject = new FileImage(
            byteData,
            Path.GetFileNameWithoutExtension(path),
            File.GetCreationTime(path), 
            fileImgUnityObject);

            fileImgUnityObject.Initialize($"({fileImgObject.FileName}, {fileImgObject.FileCreationDate})");

            fileImgs[i] = fileImgObject;
        }
    }

    private Thread EnumerateDirectoryThread(FileImage[] enumerateArray)
    {
        return new Thread(() =>
        {
            for (int i = 0; i < enumerateArray.Length; i++)
            {
                lock (EnumerateLocker)
                {
                    ConcurrentQueue.Enqueue(enumerateArray[i]);
                }
            }

            isRefreshing = false;
        });
    }
}
