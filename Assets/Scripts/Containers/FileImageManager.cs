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

    private Queue<FileImage> ConcurrentQueue { get; } = new Queue<FileImage>();

    public string BaseDirectoryPath { get; private set; }
    public string BaseSearchPattern { get; private set; }
    public EnumerationOptions BaseEnumerationOptions { get; private set; }

    private object EnumerateLocker { get; } = new object();

    private bool isRefreshing;

    private Thread enumerateThread = null;

    private FileImage[] fileImgs = null;

    public event Action<bool> OnRefresh = delegate { };

    protected void Awake()
    {
        BaseDirectoryPath = Path.Combine(Application.dataPath, chosenDirectoryName);
        BaseSearchPattern = $"*.{chosenFileExtension}";
        BaseEnumerationOptions = new EnumerationOptions()
        {
            RecurseSubdirectories = true
        };
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
                    Debug.LogWarning("FileImage has been removed from directory while refreshing");
                    StatusSingletonObject.Instance.ThrowError("FileImage has been removed from directory while refreshing");
                    return;
                }

                FileImageUnityObject instantedObject = fileImgFromQueue.UnityObject;

                instantedObject.Initialize(
                   fileImgFromQueue.FileName,
                   ImageLoader.LoadTextureFromBytes(fileImgFromQueue.ImgBytes),
                   fileImgFromQueue.FileCreationDate.ToString(), fileImgFromQueue.FilePath);

                // if this is the last queue object:
                if (ConcurrentQueue.Count == 0)
                {
                    OnRefresh(false);
                }
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

    /// <summary>
    /// Creating placeholder images within base directory path
    /// </summary>
    private void DefaultStartSetup()
    {
        Directory.CreateDirectory(BaseDirectoryPath);

        foreach (PlaceholderFileImageContainer fileImg in PlaceholderContainer.Instance.PlaceholderFiles)
        {
            string creationDirPath = Path.Combine(BaseDirectoryPath, $"{fileImg.FileName}.{chosenFileExtension}");
            byte[] imgBytes = fileImg.Texture.EncodeToPNG();
            using (FileStream fs = new FileStream(creationDirPath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(imgBytes, 0, imgBytes.Length);
            }
        }

    }

    public void Refresh()
    {
        if (isRefreshing == true || ConcurrentQueue.Count > 0)
        {
            Debug.LogWarning("Can't refresh images while refreshing. Please wait till refreshing ends");
            StatusSingletonObject.Instance.ThrowError("Can't refresh images while refreshing. Please wait till refreshing ends");
            return;
        }

        isRefreshing = true;
        OnRefresh(true);

        if (!Directory.Exists(BaseDirectoryPath))
        {
            Debug.Log("Directory does not exist, creating a default one with placeholder images");
            DefaultStartSetup();
        }

        fileImageParent.KillAllChildren();

        string[] paths = Directory.GetFiles(
            BaseDirectoryPath,
            BaseSearchPattern,
            BaseEnumerationOptions);

        fileImgs = new FileImage[paths.Length];
        ConvertPathsToData(paths);

        enumerateThread = EnumerateDirectoryThread(fileImgs);
        enumerateThread.Start();
    }

    private void ConvertPathsToData(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];

            byte[] imageData = File.ReadAllBytes(path);

            if (ImageHeaderChecker.GetLiteralExtensionFromType(imageData) != $".{chosenFileExtension}")
            {
                Debug.LogWarning($"Loaded image {path} is not a type of {chosenFileExtension} image");
                StatusSingletonObject.Instance.ThrowError($"Loaded image {path} is not a type of {chosenFileExtension} image");
                continue;
            }

            FileImageUnityObject fileImgUnityObject = Instantiate(fileImagePrefab, fileImageParent);

            FileImage fileImgObject = new FileImage(
                imageData,
                Path.GetFileNameWithoutExtension(path),
                File.GetCreationTime(path), 
                fileImgUnityObject,
                path
            );

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
