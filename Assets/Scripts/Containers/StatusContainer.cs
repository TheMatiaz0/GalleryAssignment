using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusContainer : MonoSingleton<StatusContainer>
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
        basePathStatus.text = string.Format(basePathStatus.text, dataPath);
    }

    public void ThrowError(string errorMessage)
    {
        errorStatus.gameObject.SetActive(true);
        errorStatus.text = errorMessage;
        Invoke(nameof(DesetupError), secondsUntilErrorDisappear);
    }
}
