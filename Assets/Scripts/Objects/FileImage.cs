using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileImage
{
    public byte[] ImgBytes { get; }
    public string FileName { get; }
    public DateTime FileCreationDate { get; }
    public FileImageUnityObject UnityObject { get; }

    public FileImage(byte[] imgBytes, string fileName, DateTime creationTime, FileImageUnityObject unityObject)
    {
        ImgBytes = imgBytes;
        FileName = fileName;
        FileCreationDate = creationTime;
        UnityObject = unityObject;
    }
}