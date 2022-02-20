using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StatusSingletonObject : MonoSingleton<StatusSingletonObject>
{
    [SerializeField]
    private Text basePathStatus = null;

    [SerializeField]
    private Text errorStatus = null;

    [SerializeField]
    private float secondsUntilErrorDisappear = 5;

    protected void Start()
    {
        DesetupError();
    }

    private void DesetupError()
    {
        errorStatus.gameObject.SetActive(false);
    }

    public void SetupStatus(string dataPath)
    {
        basePathStatus.text = string.Format(basePathStatus.text, Path.GetFullPath(dataPath));
    }

    public void ThrowError(string errorMessage)
    {
        errorStatus.gameObject.SetActive(true);
        errorStatus.text = errorMessage;
        Invoke(nameof(DesetupError), secondsUntilErrorDisappear);
    }
}
