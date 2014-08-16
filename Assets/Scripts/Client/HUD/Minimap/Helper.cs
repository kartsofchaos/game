using System.IO;
using UnityEngine;

public static class Helper
{
	public static void CreateAssetFolderIfNotExists(string path) {
		var fullPath = string.Format ("{0}/{1}", Application.dataPath, path);
		if (!Directory.Exists (fullPath)) {
			Directory.CreateDirectory(fullPath);
		}
	}
}
