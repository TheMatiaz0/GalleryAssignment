using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderContainer : MonoSingleton<PlaceholderContainer>
{
    [SerializeField]
    private Texture2D placeholderTexture = null;

    [SerializeField]
    private string placeholderText = null;

    [SerializeField]
    private PlaceholderFileImage[] placeholderFiles = null;

    public Texture2D PlaceholderTexture => placeholderTexture;
    public string PlaceholderText => placeholderText;
    public PlaceholderFileImage[] PlaceholderFiles => placeholderFiles;
}
