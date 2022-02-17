using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusContainer : MonoBehaviour
{
    [SerializeField]
    private Text basePathStatus = null;

    [SerializeField]
    private Text errorStatus = null;

    [SerializeField]
    private FileImageManager fileImageManager = null;

    protected void Start()
    {
        basePathStatus.text = string.Format(basePathStatus.text, fileImageManager.BaseDataPath);
        errorStatus.gameObject.SetActive(false);
        fileImageManager.OnRefresh += FileImageManager_OnRefresh;
    }

    protected void OnDestroy()
    {
        fileImageManager.OnRefresh -= FileImageManager_OnRefresh;  
    }

    private void FileImageManager_OnRefresh(string errorMessage)
    {
        if (errorMessage != null)
        {
            errorStatus.text = errorMessage;
        }

        else
        {
            errorStatus.gameObject.SetActive(false);
        }

    }
}
