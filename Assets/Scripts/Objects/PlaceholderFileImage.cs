using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlaceholderFileImage
{
    [SerializeField]
    private Texture2D texture = null;

    [SerializeField]
    private string fileName = null;

    public Texture2D Texture => texture;
    public string FileName => fileName;
}
