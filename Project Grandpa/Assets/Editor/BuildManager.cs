using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Grandpa.Models;

public class BuildManager : EditorWindow
{
	public static bool bundle { get; private set; }
	public static bool success { get; private set; }

	private static Dictionary<Version.Type, Version> latestBuilds = new Dictionary<Version.Type, Version>();
	private static Version nextBuild;
	private static Version lastBuild;
	private static string patchNotes = "";
	private static int patchNotesCountMax = 500;
	private static Vector2 patchNotesScrollPosition;

	[MenuItem("Grandpa/Build/New Version")]
    public static void ShowWindow()
	{
		var win = GetWindow(typeof(BuildManager), false, "New Version");

		ReadVersions();

		win.Show();
	}

	private void OnGUI()
	{
		EditorGUILayout.LabelField("Previous Builds", EditorStyles.boldLabel);

		if (latestBuilds == null)
		{
			GUILayout.Label("Loading...");
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUI.enabled = false;
			GUILayout.Label("Internal:");
			if (!latestBuilds.ContainsKey(Version.Type.Internal))
				GUILayout.TextField("N/A");
			else
				GUILayout.TextField(latestBuilds[Version.Type.Internal].GetFormattedString());
			GUILayout.Label("Alpha:");
			if (!latestBuilds.ContainsKey(Version.Type.Alpha))
				GUILayout.TextField("N/A");
			else
				GUILayout.TextField(latestBuilds[Version.Type.Alpha].GetFormattedString());
			GUILayout.Label("Beta:");
			if (!latestBuilds.ContainsKey(Version.Type.Beta))
				GUILayout.TextField("N/A");
			else
				GUILayout.TextField(latestBuilds[Version.Type.Beta].GetFormattedString());
			GUILayout.Label("Production:");
			if (!latestBuilds.ContainsKey(Version.Type.Release))
				GUILayout.TextField("N/A");
			else
				GUILayout.TextField(latestBuilds[Version.Type.Release].GetFormattedString());
			GUI.enabled = true;
			GUILayout.EndHorizontal();
		}

		GUILayout.Label("Latest Build / Bundle Code");
		GUILayout.BeginHorizontal();
		GUI.enabled = false;
		GUILayout.TextField(lastBuild.GetFormattedString());
		GUILayout.TextField(PlayerSettings.Android.bundleVersionCode.ToString());
		GUI.enabled = true;
		GUILayout.EndHorizontal();

		GUILayout.Space(50);
		EditorGUILayout.LabelField("Next Build", EditorStyles.boldLabel);

		GUILayout.Label("Next Build / Bundle Code");
		GUILayout.BeginHorizontal();
		nextBuild.code = GUILayout.TextField(nextBuild.code);
		GUI.enabled = false;
		GUILayout.TextField((PlayerSettings.Android.bundleVersionCode + 1).ToString());
		GUI.enabled = true;
		GUILayout.EndHorizontal();

		GUILayout.Label("Patch Notes:");
		patchNotesScrollPosition = GUILayout.BeginScrollView(patchNotesScrollPosition, GUILayout.Height(15 * 7 + 4));
		patchNotes = GUILayout.TextArea(patchNotes, GUILayout.ExpandHeight(true));
		if (patchNotes.Length > patchNotesCountMax)
		{
			patchNotes = patchNotes.Substring(0, patchNotesCountMax);
		}
		GUILayout.EndScrollView();
		GUILayout.Label("Length: " + patchNotes.Length + "/" + patchNotesCountMax);

		if (nextBuild.code == lastBuild.code)
			GUILayout.Label("Change next version.");

		GUILayout.Label("BUILD:");
		GUILayout.BeginHorizontal();
		if (nextBuild.code == lastBuild.code)
			GUI.enabled = false;
		if (GUILayout.Button("Internal"))
		{
			BuildVersion(Version.Type.Internal);
		}
		if (GUILayout.Button("Alpha"))
		{
			BuildVersion(Version.Type.Alpha);
		}
		GUI.enabled = false;
		GUILayout.Button("Beta");
		GUILayout.Button("Release");
		GUI.enabled = true;
		if (nextBuild.code == lastBuild.code)
			GUI.enabled = true;
		GUILayout.EndHorizontal();

		if (nextBuild.code != "")
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(nextBuild.code + "-internal");
			GUILayout.Label(nextBuild.code + "-alpha");
			GUILayout.Label(nextBuild.code + "-beta");
			GUILayout.Label(nextBuild.code);
			GUILayout.EndHorizontal();
		}
	}

	private static void BuildVersions()
	{
		if (!Directory.Exists(FileManager.VERSION_PATH))
		{
			Directory.CreateDirectory(FileManager.VERSION_PATH);
		}

		VersionBundle jsonObj = GetVersions();

		jsonObj.lastBuild = nextBuild;

		if (jsonObj.latestBuilds == null)
			jsonObj.latestBuilds = new Dictionary<Version.Type, Version>();
		jsonObj.AddToLatestBuilds(jsonObj.lastBuild);
			
		if (jsonObj.previousBuilds == null)
			jsonObj.previousBuilds = new List<Version>();
		jsonObj.previousBuilds.Add(nextBuild);

		string json = JsonUtility.ToJson(jsonObj);

		string versionTxtPath = FileManager.VERSION_PATH + FileManager.DELIMITER + "version.txt";
		File.WriteAllText(versionTxtPath, json);
	}

	private static void ReadVersions()
	{
		string versionTxtPath = FileManager.VERSION_PATH + FileManager.DELIMITER + "version.txt";


		if (Directory.Exists(FileManager.VERSION_PATH) && File.Exists(versionTxtPath))
		{
			VersionBundle json = GetVersions();
			nextBuild = lastBuild = json.lastBuild;
			latestBuilds = json.latestBuilds;
		}
		else
		{
			lastBuild = nextBuild = new Version()
			{
				code = "0.0.0",
				type = Version.Type.Internal,
				patchNotes = ""
			};
			latestBuilds = new Dictionary<Version.Type, Version>() { { Version.Type.Internal, new Version() { code = "0.0.0", type = Version.Type.Internal } } };
			Debug.Log(latestBuilds);

			BuildVersions();
		}
	}

	private static VersionBundle GetVersions()
	{
		string versionTxtPath = FileManager.VERSION_PATH + FileManager.DELIMITER + "version.txt";

		if (Directory.Exists(FileManager.VERSION_PATH) && File.Exists(versionTxtPath))
		{
			string json = File.ReadAllText(versionTxtPath);

			VersionBundle jsonObj = JsonUtility.FromJson<VersionBundle>(json);

			return jsonObj;
		}

		return default;
	}

	private static void IncrementBundleCode()
	{
		Debug.Log("Incrementing Bundle Code.");
		PlayerSettings.Android.bundleVersionCode++;
		currentBuild = Resources.Load<TextAsset>("Version/version").text;
		File.WriteAllText(FileManager.RESOURCES_PATH + FileManager.DELIMITER + "Version" + FileManager.DELIMITER + "version.txt", nextBuild.GetFormattedString());
		AssetDatabase.Refresh();
	}

	private static void DecrementBundleCode()
	{
		Debug.Log("Decrementing Bundle Code.");
		PlayerSettings.Android.bundleVersionCode--;
		File.WriteAllText(FileManager.RESOURCES_PATH + FileManager.DELIMITER + "Version" + FileManager.DELIMITER + "version.txt", currentBuild);
		AssetDatabase.Refresh();
	}

	public static void StartPreBuild()
	{
		PlayerSettings.bundleVersion = nextBuild.GetFormattedString();
		IncrementBundleCode();
		bundle = true;
	}

	public static void BuildCompleted()
	{
		bundle = false;
		success = true;
		BuildVersions();
		patchNotes = "";
		GetWindow<BuildManager>().Close();
		GetWindow<CustomBuildPlayerWindow>().Close();
	}

	public static void BuildFailed()
	{
		bundle = false;
		success = false;
		DecrementBundleCode();
	}

	private static string currentBuild;

	public static void BuildVersion(Version.Type type)
	{
		if (nextBuild.code == lastBuild.code)
		{
			return;
		}

		nextBuild.patchNotes = patchNotes;
		nextBuild.type = type;

		GetWindow(typeof(CustomBuildPlayerWindow)).Show();
	}

	[System.Serializable]
	public struct VersionBundle
	{
		public Version lastBuild;
		public Dictionary<Version.Type, Version> latestBuilds
		{
			get
			{
				if (m_latestBuilds == null)
				{
					return new Dictionary<Version.Type, Version>();
				}
				Dictionary<Version.Type, Version> tempDic = new Dictionary<Version.Type, Version>();
				foreach (var build in m_latestBuilds)
				{
					tempDic.Add(build.type, build);
				}
				return tempDic;
			}
			set
			{
				m_latestBuilds = new List<Version>();
				foreach (var kvp in value)
				{
					m_latestBuilds.Add(kvp.Value);
				}
			}
		}
		[SerializeField]
		private List<Version> m_latestBuilds;
		public List<Version> previousBuilds;

		public void AddToLatestBuilds(Version nextBuild)
		{
			var tempDic = latestBuilds;

			if (tempDic.ContainsKey(nextBuild.type))
				tempDic[nextBuild.type] = nextBuild;
			else
				tempDic.Add(lastBuild.type, nextBuild);

			latestBuilds = tempDic;
		}
	}
}
