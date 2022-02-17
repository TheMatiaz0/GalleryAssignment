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

    [SerializeField]
    private Sprite placeholderSprite = null;

    public void Initialize(string fileName)
    {
        this.fileName.text = fileName;
        this.fileImage.sprite = placeholderSprite;
    }

    public void Initialize(string fileName, Sprite fileImage)
    {
        Initialize(fileName);
        this.fileImage.sprite = fileImage;
        this.fileDate.text = null;
    }

    public void Initialize(string fileName, Sprite fileImage, string fileDate)
    {
        Initialize(fileName, fileImage);
        this.fileDate.text = fileDate;
    }
}
