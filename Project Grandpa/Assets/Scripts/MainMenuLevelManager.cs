using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Grandpa.UserManagment;

public class MainMenuLevelManager : LevelManager
{
	[Header("Values")]
	[SerializeField] private float fadeTime = 2f;
	[SerializeField] private string playLevelName = "Play Scene";

	[Header("References")]
	[SerializeField] private Modal modal;
	[SerializeField] private TMP_InputField nameInput;
	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject settingsMenu;
	[SerializeField] private GameObject firstLoginMenu;
	[SerializeField] private TMP_InputField settingsNameInput;
	[SerializeField] private TextMeshProUGUI boardName;

	private string welcomeMessageString = "";

	private IEnumerator PlayLevelCoroutine()
	{
		float timeExpiry = Time.unscaledTime + fadeTime * 2;
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.Out, fadeTime, () =>
		{
			SceneManager.Instance.StartLoadLevelAsync(playLevelName, true);
			ScreenOverlay.Instance.StartLoading(() =>
			{
				return Mathf.Lerp(0.2f, 1f, SceneManager.Instance.CurrentLevelLoadPercentage() / .9f);
			});
			ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.In, fadeTime);
		});

		while (Time.unscaledTime < timeExpiry || SceneManager.Instance.CurrentLevelLoadPercentage() < .9f)
		{
			yield return null;
		}

		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.Out, fadeTime, () =>
		{
			SceneManager.Instance.AllowLoadLevel();
			ScreenOverlay.Instance.FinishLoading();
		});
	}

	public void PlayLevel()
	{
		StartCoroutine(PlayLevelCoroutine());
	}

	Dictionary<string, string> randomPerson = new Dictionary<string, string>();
	IEnumerator GetRandomUser(float timeout, System.Action OnFinishedCallback = null)
	{
		using (UnityWebRequest webRequest = UnityWebRequest.Get("https://uinames.com/api/?region=croatia"))
		{
			timeout = Time.unscaledTime + timeout;

			// Request and wait for the desired page.
			webRequest.SendWebRequest();

			while (!webRequest.isDone)
			{
				yield return null;

				if (Time.unscaledTime >= timeout)
				{
					randomPerson.Add("name", "Unknown");
					OnFinishedCallback?.Invoke();
					yield break;
				}
			}

			if (webRequest.isNetworkError)
			{
				randomPerson.Add("name", "Unknown");
				OnFinishedCallback?.Invoke();
			}
			else
			{
				/*if (webRequest.responseCode == 508)
				{*/
					randomPerson.Add("name", new string[] { "John", "Greg", "Elvira" }[Random.Range(0, 3)]);
				/*}
				else
				{
					print(webRequest.downloadHandler.text);
					randomPerson = JsonUtility.FromJson<Dictionary<string, string>>(webRequest.downloadHandler.text);
					print(randomPerson);
				}*/
				OnFinishedCallback?.Invoke();
			}
		}
	}

	public void OnButtonSettings()
	{
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		settingsMenu.SetActive(true);
		settingsNameInput.text = Authentication.playerName;
	}

	public void OnButtonSettingsSave()
	{
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		if (settingsNameInput.text != "")
		{
			Authentication.SetDisplayName(settingsNameInput.text,
				OnSuccess: () =>
				{
					boardName.text = Authentication.playerName;
				}
			);
		}
		settingsMenu.SetActive(false);
	}

	public void OnButtonSettingsBack()
	{
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		settingsMenu.SetActive(false);
	}

	private bool warned;
	public void OnButtonSaveName()
	{
		if (nameInput.text == "" && !warned)
		{
			modal.Display("Error", "Please input your name, or it will be assigned randomly.");
			warned = true;
		}
		else if (nameInput.text == "")
		{
			StartCoroutine(GetRandomUser(3f, () =>
			{
				Authentication.SetDisplayName(randomPerson["name"],
					OnSuccess: () =>
					{
						boardName.text = randomPerson["name"];
					}
				);
			}));

			mainMenu.SetActive(true);
			firstLoginMenu.SetActive(false);
			modal.Display("Welcome", welcomeMessageString);
		}
		else
		{
			Authentication.SetDisplayName(nameInput.text,
				OnSuccess: () =>
				{
					boardName.text = Authentication.playerName;
				}
			);
			mainMenu.SetActive(true);
			firstLoginMenu.SetActive(false);
			modal.Display("Welcome, " + nameInput.text, welcomeMessageString);
		}
	}

	void Start()
    {
		mainMenu.SetActive(false);
		settingsMenu.SetActive(false);
		firstLoginMenu.SetActive(false);
		boardName.text = "";

		try
		{
			Authentication.GetCurrentVersion(
			OnSuccess: version =>
			{
				if (version == GameManager.GameVersion.GetFormattedString())
				{
					welcomeMessageString = "Welcome to the test build " + GameManager.GameVersion.GetFormattedString() + "\n\n" +
					"Expect bugs and not many completed features. If you find bugs, please report them."; ;
				}
				else
				{
					welcomeMessageString = "<color=red>Your build version is outdated. Some features might be" +
					" unavailable.</color>\nGo to Play Store and update.\n\n" +
					"Expect bugs and not many completed features. If you find bugs, please report them.";
				}
			},
			OnError: error =>
			{
				welcomeMessageString = "Welcome to the test build " + GameManager.GameVersion.GetFormattedString() + "\n\nCannot connect to database. Some features might be" +
					" unavailable\n\nExpect bugs and not many completed features. If you find bugs, please report them.";
			}
			);
		}
		catch (System.Exception e)
		{
			welcomeMessageString = "Welcome to the test build " + GameManager.GameVersion.GetFormattedString() + "\n\nCannot connect to database. Some features might be" +
					" unavailable\n\nExpect bugs and not many completed features. If you find bugs, please report them.";
		}

		if (Authentication.NewlyCreated)
		{
			firstLoginMenu.SetActive(true);
		}
		else
		{
			mainMenu.SetActive(true);
			boardName.text = Authentication.playerName;
		}

		ScreenOverlay.Instance.Fade(ScreenOverlay.FadeType.In, fadeTime, () =>
		{
			if (Authentication.playerName != "")
			{
				modal.Display("Welcome", welcomeMessageString, onClicked: () =>
				{

				});
			}
		});
	}
}
