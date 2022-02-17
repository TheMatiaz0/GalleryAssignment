using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static void KillAllChildren(this Transform parent)
    {
        foreach (Transform child in parent.transform)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
