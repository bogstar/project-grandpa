using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grandpa.UI
{
	public class GameOverModal : PlaySceneModal
	{
		[Header("References")]
		[SerializeField] private Button backButton;
		[SerializeField] private TextMeshProUGUI scoreLabel;

		public override void ShowModal(bool show, bool immediate = false)
		{
			base.ShowModal(show, immediate);
			backButton.enabled = show;
			if (show)
			{
				scoreLabel.text = FindObjectOfType<RunManager>().GameOverText;
			}
		}

		public void OnButton_Back()
		{
			AudioManager.PlayClipStackable(AudioManager.Audio.Click);
			ShowModal(false, true);
			PlayGUI.RunManager.StartPose();
			PlayGUI.DisplayLeaderboardsModal(true);
		}
	}
}