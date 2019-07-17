using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grandpa.UI
{
	public class PlayGUI : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private TextMeshProUGUI scoreLabel;
		[SerializeField] private Button pauseButton;

		[SerializeField] private PlayAndLeaderboardsModal m_playAndLeaderboardsModal;
		[SerializeField] private LeaderboardsModal m_leaderboardsModal;
		[SerializeField] private GameOverModal m_gameOverModal;
		[SerializeField] private PauseModal m_pauseModal;

		public PlayAndLeaderboardsModal PlayAndLeaderboardsModal { get; private set; }
		public LeaderboardsModal LeaderboardsModal { get; private set; }
		public GameOverModal GameOverModal { get; private set; }
		public PauseModal PauseModal { get; private set; }

		public RunManager RunManager { get; private set; }

		private void Start()
		{
			LeaderboardsModal = m_leaderboardsModal;
			PlayAndLeaderboardsModal = m_playAndLeaderboardsModal;
			GameOverModal = m_gameOverModal;
			PauseModal = m_pauseModal;

			RunManager = FindObjectOfType<RunManager>();

			DisplayPauseButton(false);
			DisplayScore(false);
		}

		public void OnButton_Left()
		{
			RunManager.Left();
		}

		public void OnButton_Right()
		{
			RunManager.Right();
		}

		public void OnButton_Down()
		{
			RunManager.Down();
		}

		public void OnButton_Pause()
		{
			RunManager.ButtonPause();
			PauseModal.ShowModal(true, true);
			if (RunManager.Runner.CurrentState == Runner.State.Posing)
			{
				DisplayPlayLeaderboardsModal(false);
				DisplayLeaderboardsModal(false);
				DisplayGameOverModal(false);
			}
		}

		public void DisplayPauseButton(bool display)
		{
			pauseButton.gameObject.SetActive(display);
		}

		public void DisplayPlayLeaderboardsModal(bool show)
		{
			PlayAndLeaderboardsModal.ShowModal(show);
		}

		public void DisplayLeaderboardsModal(bool show)
		{
			LeaderboardsModal.ShowModal(show);
		}

		public void DisplayGameOverModal(bool show)
		{
			GameOverModal.ShowModal(show);
		}

		public void UpdateScore(int score)
		{
			scoreLabel.text = "Score:\n" + score;
		}

		public void DisplayScore(bool display)
		{
			scoreLabel.gameObject.SetActive(display);
		}
	}
}