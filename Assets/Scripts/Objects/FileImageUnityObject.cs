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

    [SerializeField]
    private GameObject enlargeLabel = null;

    [SerializeField]
    private Animator fileImgAnimator = null;

    private const string SCALE_STATE = "ScaleFileImagePrefab";
    private readonly int scaleStateId = Animator.StringToHash(SCALE_STATE);
    private const string DIRECTION_PARAMETER = "Direction";
    private readonly int directionId = Animator.StringToHash(DIRECTION_PARAMETER);

    private string filePath = null;

    public static event Action<Texture2D, string, string, string> OnOpenModal = delegate { };

    public void Initialize(string fileDate)
    {
        this.fileName.text = PlaceholderContainer.Instance.PlaceholderText;
        this.fileImage.texture = PlaceholderContainer.Instance.PlaceholderTexture;
        this.fileDate.text = fileDate;
        enlargeLabel.SetActive(false);
    }

    public void Initialize(string fileName, Texture2D fileImage, string fileDate, string filePath)
    {
        this.fileName.text = fileName;
        this.fileImage.texture = fileImage;
        this.fileDate.text = fileDate;
        this.filePath = filePath;
        enlargeLabel.SetActive(true);
    }

    public void OpenImageModal()
    {
        if (this.fileImage.texture != PlaceholderContainer.Instance.PlaceholderTexture)
        {
            OnOpenModal((Texture2D)fileImage.texture, fileName.text, fileDate.text, filePath);
        }
    }

    public void OnPointerEnterThumbnail()
    {
        TweenSpeedMultiply(1);
    }

    public void OnPointerExitThumbnail()
    {
        TweenSpeedMultiply(-1);
    }

    private void TweenSpeedMultiply(float multiply)
    {
        fileImgAnimator.SetFloat(directionId, multiply);
        fileImgAnimator.CrossFade(scaleStateId, 1, -1, 0);
    }
}
