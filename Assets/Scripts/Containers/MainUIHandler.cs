using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIHandler : MonoBehaviour
{
    [SerializeField]
    private FileImageManager fileImgManager = null;

    [SerializeField]
    private Animator refreshAnimator = null;

    private const string IS_REFRESHING_BOOLEAN = "IsRefreshing";

    private readonly int isRefreshingID = Animator.StringToHash(IS_REFRESHING_BOOLEAN);

    protected void Awake()
    {
        fileImgManager.OnRefresh += FileImgManager_OnRefresh;
    }

    protected void Start()
    {
        OnButtonRefreshClick();
    }

    private void FileImgManager_OnRefresh(bool isRefreshing)
    {
        refreshAnimator.SetBool(isRefreshingID, isRefreshing);
    }

    protected void OnDestroy()
    {
        fileImgManager.OnRefresh -= FileImgManager_OnRefresh;
    }

    public void OnButtonRefreshClick()
    {
        StatusSingletonObject.Instance.SetupStatus(fileImgManager.BaseDirectoryPath);
        fileImgManager.Refresh();
    }

    public void OnButtonOpenExplorerClick()
    {
        Application.OpenURL($"file://{fileImgManager.BaseDirectoryPath}");
    }
}
