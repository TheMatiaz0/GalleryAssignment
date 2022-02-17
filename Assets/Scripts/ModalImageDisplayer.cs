using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalImageDisplayer : MonoBehaviour
{
    [SerializeField]
    private Text fileName = null;

    [SerializeField]
    private Text fileDate = null;

    [SerializeField]
    private Image fullImage = null;

    [SerializeField]
    private GameObject visibleParent = null;

    protected void Awake()
    {
        FileImageObject.OnOpenModal += FileImageObject_OnOpenModal;
    }

    private void FileImageObject_OnOpenModal(Texture2D tex, string fileName, string fileDate)
    {
        Display(tex, fileName, fileDate);
    }

    protected void OnDestroy()
    {
        FileImageObject.OnOpenModal -= FileImageObject_OnOpenModal;
    }

    public void Display(Texture2D tex, string fileName, string fileDate)
    {
        fullImage.sprite = ImageLoader.LoadSpriteFromTexture(tex);
        this.fileName.text = fileName;
        this.fileDate.text = fileDate;
        visibleParent.SetActive(true);
    }

    public void UnDisplay()
    {
        visibleParent.SetActive(false);
    }
}
