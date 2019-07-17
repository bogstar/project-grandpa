using Grandpa.UserManagment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grandpa.UI
{
	public class LeaderboardsModal : PlaySceneModal
	{
		[Header("References")]
		[SerializeField] private Button backButton;
		[SerializeField] private TextMeshProUGUI namesLabel;
		[SerializeField] private TextMeshProUGUI scoresLabel;

		protected override string HideAnimationName { get { return "play-leader_popdown"; } }

		public override void ShowModal(bool show, bool immediate = false)
		{
			base.ShowModal(show, immediate);
			backButton.enabled = show;
			if (show)
			{
				UpdateLeaderboards();
			}
		}

		private void UpdateLeaderboards()
		{
			namesLabel.text = "Loading...";
			scoresLabel.text = "";

			Authentication.GetHighScores(OnCompleted: leaderboards =>
			{
				var names = leaderboards["names"];
				var scores = leaderboards["scores"];
				namesLabel.text = names;
				scoresLabel.text = scores;
			});
		}

		public void OnButton_Back()
		{
			AudioManager.PlayClipStackable(AudioManager.Audio.Click);
			ShowModal(false);
			PlayGUI.DisplayPlayLeaderboardsModal(true);
		}
	}
}