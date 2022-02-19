using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/* https://gist.github.com/ChuckSavage/dc079e21563ba1402cf6c907d81ac1ca?permalink_comment_id=3270422#gistcomment-3270422 */

public enum FileType
{
	Unknown,
	Jpeg,
	Bmp,
	Gif,
	Png,
	Pdf
}

public static class ImageHeaderChecker
{
	private static readonly Dictionary<FileType, byte[]> KNOWN_FILE_HEADERS = new Dictionary<FileType, byte[]>()
	{
		{ FileType.Jpeg, new byte[]{ 0xFF, 0xD8 }},
		{ FileType.Bmp, new byte[]{ 0x42, 0x4D }},
		{ FileType.Gif, new byte[]{ 0x47, 0x49, 0x46 }},
		{ FileType.Png, new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }},
		{ FileType.Pdf, new byte[]{ 0x25, 0x50, 0x44, 0x46 }}
	};

	private static readonly string[] fileTypeLiteralExtensions = new string[]
	{
		"",
		".jpeg",
		".bmp",
		".gif",
		".png",
		".pdf"
	};


	/// <summary>
	/// Get literal (string) representation of file from byte array data
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	public static string GetLiteralExtensionFromType(ReadOnlySpan<byte> data)
    {
		return fileTypeLiteralExtensions[(int)GetKnownFileType(data)];
    }


	public static FileType GetKnownFileType(ReadOnlySpan<byte> data)
	{
		foreach (var check in KNOWN_FILE_HEADERS)
		{
			if (data.Length >= check.Value.Length)
			{
				var slice = data.Slice(0, check.Value.Length);
				if (slice.SequenceEqual(check.Value))
				{
					return check.Key;
				}
			}
		}

		return FileType.Unknown;
	}
}
