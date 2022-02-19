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

    [SerializeField]
    private Button openDefaultBtn = null;

    protected void Awake()
    {
        FileImageUnityObject.OnOpenModal += FileImageObject_OnOpenModal;
    }

    private void FileImageObject_OnOpenModal(Texture2D tex, string fileName, string fileDate, string filePath)
    {
        Display(tex, fileName, fileDate, filePath);
    }

    protected void OnDestroy()
    {
        FileImageUnityObject.OnOpenModal -= FileImageObject_OnOpenModal;
    }

    public void Display(Texture2D tex, string fileName, string fileDate, string pathToFile)
    {
        fullImage.sprite = ImageLoader.LoadSpriteFromTexture(tex);
        this.fileName.text = fileName;
        this.fileDate.text = fileDate;
        openDefaultBtn.onClick.AddListener(() => Application.OpenURL($"file://{pathToFile}"));
        visibleParent.SetActive(true);
    }

    public void UnDisplay()
    {
        visibleParent.SetActive(false);
    }
}
