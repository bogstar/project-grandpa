using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomBuildPlayerWindow : BuildPlayerWindow
{
	private void OnFocus()
	{
		if (!BuildManager.success && BuildManager.bundle)
		{
			Debug.Log("Build Player detected unsuccessful build.");
			BuildManager.BuildFailed();
		}
	}
}
