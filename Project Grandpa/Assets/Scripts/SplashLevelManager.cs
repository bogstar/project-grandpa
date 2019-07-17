using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grandpa.UserManagment;

public class SplashLevelManager : LevelManager
{
	[Header("Values")]
	[Tooltip("Defines how long should one splash screen persist on screen before fading. (in seconds)")]
	[SerializeField] private float splashPersistence = 5f;
	[SerializeField] private float fadeTime = .5f;
	[SerializeField] private string mainMenuLevelName = "Main Menu";

	[Header("References")]
	[SerializeField] private GameObject splashScreen1;
	[SerializeField] private GameObject splashScreen2;
	[SerializeField] private LoadingBar loadingBar;

	private Coroutine splashPersistingCoroutine;

	private void Start()
	{
		splashScreen1.SetActive(true);
		splashScreen2.SetActive(false);
		ScreenOverlay.Instance.FadeInstant(ScreenOverlay.FadeType.Out);
		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.In, fadeTime, () =>
		{
			splashPersistingCoroutine = StartCoroutine(PersistSplash(splashPersistence, () =>
			{
				OnButtonSkipSplash(0);
			}));
		});
		AudioManager.PlayClipStackable(AudioManager.Audio.Intro);
	}

	private IEnumerator PersistSplash(float duration, System.Action OnFinishedCallback = null)
	{
		float expiryTime = Time.time + duration;

		while (Time.time < expiryTime)
		{
			yield return null;
		}

		OnFinishedCallback?.Invoke();
	}
	
	IEnumerator LoadLevel()
	{
		SceneManager sceneManager = SceneManager.Instance;

		sceneManager.StartLoadLevelAsync(mainMenuLevelName, true);
		Authentication.BeginLogin();

		loadingBar.AssignLoadingBar(() =>
		{
			var authStatus = Authentication.GetAuthenticationStatus();
			if (authStatus == Authentication.AuthenticationStatus.LoggingIn)
			{
				return 0.2f;
			}
			else
			{
				return Mathf.Lerp(0.2f, 1f, sceneManager.CurrentLevelLoadPercentage() / .9f);
			}
		});

		float timeout = Time.unscaledTime + 5f;

		while (!(sceneManager.CurrentLevelLoadPercentage() >= .9f &&
			(Time.unscaledTime >= timeout || Authentication.GetAuthenticationStatus() == Authentication.AuthenticationStatus.LoggedIn ||
			Authentication.GetAuthenticationStatus() == Authentication.AuthenticationStatus.ErrorLoggingIn)))
		{
			yield return null;
		}

		yield return new WaitForSeconds(2f);

		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.Out, fadeTime, () =>
		{
			splashScreen2.SetActive(false);
			sceneManager.AllowLoadLevel();
		});
	}
	
	public void OnButtonSkipSplash(int index)
	{
		StopCoroutine(splashPersistingCoroutine);
		switch (index)
		{
			case 0:
				ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.Out, fadeTime, () =>
				{
					splashScreen1.SetActive(false);
					splashScreen2.SetActive(true);
					ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.In, fadeTime, () =>
					{
						StartCoroutine(LoadLevel());
					});
				});
				break;
		}
	}
}