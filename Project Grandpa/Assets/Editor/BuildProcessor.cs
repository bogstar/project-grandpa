using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProcessor : IPostprocessBuildWithReport, IPreprocessBuildWithReport
{
	public int callbackOrder { get { return 0; } }

	public void OnPostprocessBuild(BuildReport report)
	{
		Debug.Log("PostProcess started. Assuming build finished successfuly.");
		Debug.Log("Actual result: " + report.summary.result);
		BuildManager.BuildCompleted();
	}

	public void OnPreprocessBuild(BuildReport report)
	{
		Debug.Log("PreProcess started.");
		BuildManager.StartPreBuild();
	}
}
