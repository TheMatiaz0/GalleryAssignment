using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileImage
{
    public byte[] ImgBytes { get; }
    public string FileName { get; }
    public DateTime FileCreationDate { get; }

    public FileImage(byte[] imgBytes, string fileName, DateTime creationTime)
    {
        this.ImgBytes = imgBytes;
        this.FileName = fileName;
        this.FileCreationDate = creationTime;
    }
}