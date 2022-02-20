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

    // Using these because of no longer having {0} for formatting after changes
    private string startPathText = null;
    private string startErrorText = null;

    private Coroutine desetupErrorCoroutine = null;

    protected void Start()
    {
        startPathText = basePathStatus.text;
        startErrorText = errorStatus.text;
        errorStatus.gameObject.SetActive(false);
    }

    private IEnumerator DesetupError()
    {
        yield return new WaitForSeconds(secondsUntilErrorDisappear);
        errorStatus.gameObject.SetActive(false);
        desetupErrorCoroutine = null;
    }

    public void SetupStatus(string dataPath)
    {
        basePathStatus.text = string.Format(startPathText, Path.GetFullPath(dataPath));
    }

    public void ThrowError(string errorMessage)
    {
        errorStatus.gameObject.SetActive(true);
        errorStatus.text = string.Format(startErrorText, errorMessage);
        if (desetupErrorCoroutine == null)
        {
            desetupErrorCoroutine = StartCoroutine(DesetupError());
        }
    }
}
