using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLevelManager : LevelManager
{
	[Header("Values")]
	[SerializeField] private float fadeTime = 2f;
	[SerializeField] private string mainMenuLeveName = "Main Menu";

	
	void Start()
	{
		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.In, fadeTime);
	}

	public void BackToMainMenu()
	{
		SceneManager.Instance.StartLoadLevelAsync(mainMenuLeveName, true);
		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.Out, fadeTime, () =>
		{
			Time.timeScale = 1f;
			SceneManager.Instance.AllowLoadLevel();
		});
	}
}
