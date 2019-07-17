using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grandpa.UI
{
	public class PauseModal : PlaySceneModal
	{
		[Header("Values")]
		[SerializeField] private float unpauseDuration = 3f;

		[Header("References")]
		[SerializeField] private Button unpauseButton;
		[SerializeField] private Button quitButton;
		[SerializeField] private TextMeshProUGUI unpauseLabel;

		private int unpausingCounter = -1;


		public override void ShowModal(bool show, bool immediate = false)
		{
			base.ShowModal(show, immediate);

			if (show)
			{
				unpauseLabel.gameObject.SetActive(false);
				quitButton.gameObject.SetActive(true);
				unpauseButton.gameObject.SetActive(true);
				PlayGUI.DisplayPauseButton(false);
			}
		}

		public void OnButton_Unpause()
		{
			AudioManager.PlayClipStackable(AudioManager.Audio.Click);
			StartCoroutine(UnpauseCoroutine(unpauseDuration));
		}

		public void OnButton_Quit()
		{
			AudioManager.PlayClipStackable(AudioManager.Audio.Click);
			((PlayLevelManager)LevelManager.Instance).BackToMainMenu();
		}

		IEnumerator UnpauseCoroutine(float timer)
		{
			unpauseLabel.gameObject.SetActive(true);
			quitButton.gameObject.SetActive(false);
			unpauseButton.gameObject.SetActive(false);

			float expired = Time.unscaledTime + timer;

			while (Time.unscaledTime < expired)
			{
				int timeRemaining = Mathf.CeilToInt(expired - Time.unscaledTime);
				if (unpausingCounter != timeRemaining)
				{
					AudioManager.PlayClipStackable(AudioManager.Audio.Click);
					unpausingCounter = timeRemaining;
				}
				unpauseLabel.text = "Resuming in:\n" + timeRemaining;
				yield return null;
			}

			ShowModal(false, true);
			PlayGUI.RunManager.ButtonUnpause();
			PlayGUI.DisplayPauseButton(true);
		}
	}
}