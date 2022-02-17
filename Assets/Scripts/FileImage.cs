using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileImage
{
    public byte[] imgBytes;
    public string fileName;
    public DateTime creationDate;

    public FileImage(byte[] imgBytes, string fileName, DateTime creationTime)
    {
        this.imgBytes = imgBytes;
        this.fileName = fileName;
        this.creationDate = creationTime;
    }
}