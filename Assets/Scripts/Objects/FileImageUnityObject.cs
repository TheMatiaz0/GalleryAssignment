using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileImageUnityObject : MonoBehaviour
{
    [SerializeField]
    private RawImage fileImage = null;

    [SerializeField]
    private Text fileName = null;

    [SerializeField]
    private Text fileDate = null;

    public static event Action<Texture2D, string, string> OnOpenModal = delegate { };

    public void Initialize(string fileDate)
    {
        this.fileName.text = PlaceholderContainer.Instance.PlaceholderText;
        this.fileImage.texture = PlaceholderContainer.Instance.PlaceholderTexture;
        this.fileDate.text = fileDate;
    }

    public void Initialize(string fileName, Texture2D fileImage, string fileDate)
    {
        this.fileName.text = fileName;
        this.fileImage.texture = fileImage;
        this.fileDate.text = fileDate;
    }

    public void OpenImageModal()
    {
        if (this.fileImage.texture != PlaceholderContainer.Instance.PlaceholderTexture)
        {
            OnOpenModal((Texture2D)fileImage.texture, fileName.text, fileDate.text);
        }
    }
}
