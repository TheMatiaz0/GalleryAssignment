using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileImageObject : MonoBehaviour
{
    [SerializeField]
    private RawImage fileImage = null;

    [SerializeField]
    private Text fileName = null;

    [SerializeField]
    private Text fileDate = null;

    [SerializeField]
    private Texture2D placeholderSprite = null;

    [SerializeField]
    private string placeholderText = null;

    public void Initialize()
    {
        this.fileName.text = placeholderText;
        this.fileImage.texture = placeholderSprite;
        this.fileDate.text = null;
    }

    public void Initialize(string fileName, Texture2D fileImage, string fileDate)
    {
        this.fileName.text = fileName;
        this.fileImage.texture = fileImage;
        this.fileDate.text = fileDate;
    }
}
