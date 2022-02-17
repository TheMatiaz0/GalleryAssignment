using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileImageObject : MonoBehaviour
{
    [SerializeField]
    private Image fileImage = null;

    [SerializeField]
    private Text fileName = null;

    [SerializeField]
    private Text fileDate = null;

    public void Initialize(string fileName, Sprite fileImage)
    {
        this.fileName.text = fileName;
        this.fileImage.sprite = fileImage;
    }

    public void Initialize(string fileName, Sprite fileImage, string fileDate)
    {
        Initialize(fileName, fileImage);
        this.fileDate.text = fileDate;
    }
}
