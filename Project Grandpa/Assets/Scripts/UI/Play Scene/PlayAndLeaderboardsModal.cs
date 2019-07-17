using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grandpa.UI
{
	public class PlayAndLeaderboardsModal : PlaySceneModal
	{
		[Header("References")]
		[SerializeField] private Button startButton;
		[SerializeField] private Button leaderboardsButton;
		[SerializeField] private Button quitButton;

		protected override string HideAnimationName { get { return "Popdown"; } }

		public override void ShowModal(bool show, bool immediate = false)
		{
			base.ShowModal(show, immediate);
			startButton.enabled = show;
			leaderboardsButton.enabled = show;
			quitButton.enabled = show;
		}

		public void OnButton_Play()
		{
			ShowModal(false);
			PlayGUI.RunManager.ButtonPlay();
		}

		public void OnButton_Leaderboards()
		{
			AudioManager.PlayClipStackable(AudioManager.Audio.Click);
			ShowModal(false);
			PlayGUI.DisplayLeaderboardsModal(true);
		}

		public void OnButton_Quit()
		{
			((PlayLevelManager)LevelManager.Instance).BackToMainMenu();
		}
	}
}