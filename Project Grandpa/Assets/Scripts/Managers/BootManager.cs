using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootManager : Manager<BootManager>
{
	[Header("Prefabs")]
	[SerializeField] private GameObject managers;

	[Header("Values")]
	[SerializeField] private string splashLevelName = "Splash";
	[SerializeField] private string bootLevelName = "Boot";

	protected override void Awake()
	{
		base.Awake();

		StartCoroutine(LoadText());
		Instantiate(managers);
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	private void Start()
	{
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == bootLevelName)
		{
			SceneManager.Instance.StartLoadLevelAsync(splashLevelName);
		}
	}

	private IEnumerator LoadText()
	{
		var resource = Resources.LoadAsync<TextAsset>("Version/version");

		while (!resource.isDone)
		{
			yield return null;
		}

		GameManager.SetVersion(resource.asset.ToString());
	}
}