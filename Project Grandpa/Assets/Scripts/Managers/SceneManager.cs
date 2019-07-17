using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Manager<SceneManager>
{
	private AsyncOperation levelLoadingOperation;

	public float CurrentLevelLoadPercentage()
	{
		if (levelLoadingOperation != null)
		{
			return levelLoadingOperation.progress;
		}
		return 0f;
	}

	public void StartLoadLevelAsync(string levelName, bool disallowImmediateLoad = false)
	{
		StartCoroutine(LoadLevel(levelName, disallowImmediateLoad));
	}

	public void AllowLoadLevel()
	{
		if (levelLoadingOperation != null)
		{
			levelLoadingOperation.allowSceneActivation = true;
		}
	}

	private IEnumerator LoadLevel(string levelName, bool disallowImmediateLoad)
	{
		levelLoadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelName);
		levelLoadingOperation.allowSceneActivation = false;

		while (levelLoadingOperation.progress < .9f)
		{
			yield return null;
		}

		if (!disallowImmediateLoad)
		{
			levelLoadingOperation.allowSceneActivation = true;
		}

		while (!levelLoadingOperation.isDone)
		{
			yield return null;
		}

		levelLoadingOperation = null;
		LevelManager levelManager = FindObjectOfType<LevelManager>();
		if (levelManager == null)
		{
			Debug.LogError("Level Manager for this level is not set up.");
		}
		else
		{
			levelManager.Init();
		}
	}
}