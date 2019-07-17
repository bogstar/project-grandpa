using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
	public static readonly char DELIMITER = Path.DirectorySeparatorChar;
	public static readonly string PROJECT_PATH = Application.dataPath.TrimEnd(Application.dataPath.Substring(Application.dataPath.LastIndexOf('/')).ToCharArray());
	public static readonly string VERSION_PATH = PROJECT_PATH + DELIMITER + "Version";
	public static readonly string ASSET_PATH = PROJECT_PATH + DELIMITER + "Assets";
	public static readonly string RESOURCES_PATH = ASSET_PATH + DELIMITER + "Resources";
}
