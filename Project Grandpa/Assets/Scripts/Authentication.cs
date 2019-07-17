using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab.ClientModels;
using PlayFab;

namespace Grandpa.UserManagment
{
	public class Authentication : Singleton<Authentication>
	{
		private static string googleServerAuthCode = "";
		private static AuthenticationStatus authenticationStatus = AuthenticationStatus.NotStarted;
		public static string playerName = "";

		private static bool m_newlyCreated = false;
		/// <summary>
		/// Is logged-in account created on this login. Returns false if not logged in.
		/// </summary>
		public static bool NewlyCreated
		{
			get
			{
				if (authenticationStatus == AuthenticationStatus.LoggedIn)
				{
					return m_newlyCreated;
				}
				else
				{
					return false;
				}
			}
		}

		public static void GetHighScores(System.Action<Dictionary<string, string>> OnCompleted = null)
		{
			try
			{
				PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest()
				{
					StatisticName = "Best Time"
				},
				success =>
				{
					var leaderboard = success.Leaderboard;
					if (leaderboard == null || leaderboard.Count == 0)
					{
						OnCompleted?.Invoke(new Dictionary<string, string>()
						{
							{ "names", "Leaderboards are empty." },
							{ "scores", "" }
						});
					}
					else
					{
						string names = "";
						string scores = "";
						foreach (var entry in leaderboard)
						{
							names += (entry.Position + 1) + ". " + entry.DisplayName + "\n";
							scores += entry.StatValue + "\n";
						}

						OnCompleted?.Invoke(new Dictionary<string, string>()
						{
							{ "names", names },
							{ "scores", scores }
						});
					}
				},
				error =>
				{
					OnCompleted?.Invoke(new Dictionary<string, string>()
					{
						{ "names", "Error loading leaderboards." },
						{ "scores", "" }
					});
				});
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				OnCompleted?.Invoke(new Dictionary<string, string>()
				{
					{ "names", "Error loading leaderboards." },
					{ "scores", "" }
				});
			}
		}

		public static void LoginToGPG(System.Action OnSuccess = null, System.Action<string> OnFailure = null)
		{
			// Config
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			.RequestServerAuthCode(false)
			.AddOauthScope("profile")
			.Build();

			// Initialize Config
			PlayGamesPlatform.InitializeInstance(config);
			// recommended for debugging:
			PlayGamesPlatform.DebugLogEnabled = true;
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();

			// Authenticate
			Social.localUser.Authenticate((success, status) =>
			{
				// Successful GPS login
				if (success)
				{
					googleServerAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
					OnSuccess?.Invoke();
				}
				else
				{
					OnFailure?.Invoke(status);
				}
			});
		}

		public static void GetCurrentVersion(System.Action<string> OnSuccess = null, System.Action<PlayFabErrorCode> OnError = null)
		{
			PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
			{
				FunctionName = "getCurrentVersion",
				FunctionParameter = GameManager.GameVersion.GetTypeStringFormatted()
			},
			success =>
			{
				var result = (string)success.FunctionResult;
				OnSuccess?.Invoke(result);
			},
			error =>
			{
				OnError(error.Error);
			});
		}

		public static void SaveHighscore(int score, System.Action OnSuccess = null, System.Action OnError = null)
		{
			try
			{
				PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
				{
					FunctionName = "saveScore",
					FunctionParameter = new Dictionary<string, string>()
				{
					{ "version", GameManager.GameVersion.GetTypeStringFormatted() },
					{ "versionCode", GameManager.GameVersion.GetFormattedString() },
					{ "score", score.ToString() }
				}
				},
				success =>
				{
					OnSuccess?.Invoke();
				},
				error =>
				{
					OnError?.Invoke();
				});
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				OnError?.Invoke();
			}
		}

		public static void SetDisplayName(string newName, System.Action OnSuccess = null, System.Action OnError = null)
		{
			playerName = newName;
			PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
			{
				DisplayName = newName
			},
			success =>
			{
				OnSuccess?.Invoke();
			},
			error =>
			{
				OnError?.Invoke();
			});
		}

		public static void LoginToPlayFab(System.Action OnSuccess = null, System.Action OnFailure = null)
		{
			PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
			{
				ServerAuthCode = googleServerAuthCode,
				CreateAccount = true
			},
			success =>
			{
				if (success.NewlyCreated)
				{
					SetDisplayName(Social.localUser.userName);
				}
				else
				{
					PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
					{
						ProfileConstraints = new PlayerProfileViewConstraints()
						{
							ShowDisplayName = true
						}
					},
					success2 =>
					{
						playerName = success2.PlayerProfile.DisplayName;
					},
					error2 =>
					{

					});
				}
				authenticationStatus = AuthenticationStatus.LoggedIn;
				m_newlyCreated = success.NewlyCreated;
				OnSuccess?.Invoke();
			},
			error =>
			{
				LoginToPlayfabWithDeviceID();
			});
		}

		private static void LoginToPlayfabWithDeviceID(System.Action OnSuccess = null, System.Action OnFailure = null)
		{
			PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
			{
				AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true
			},
				success =>
				{
					authenticationStatus = AuthenticationStatus.LoggedIn;
					PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
					{
						ProfileConstraints = new PlayerProfileViewConstraints()
						{
							ShowDisplayName = true
						}
					},
					success2 =>
					{
						playerName = success2.PlayerProfile.DisplayName;
					},
					error2 =>
					{

					});
					m_newlyCreated = success.NewlyCreated;
					OnSuccess?.Invoke();
				},
				error =>
				{
					authenticationStatus = AuthenticationStatus.ErrorLoggingIn;
					OnFailure?.Invoke();
				});
		}

		public static void BeginLogin()
		{
			authenticationStatus = AuthenticationStatus.LoggingIn;
			LoginToGPG(
				OnSuccess: () => 
				{
					LoginToPlayFab();
				},
				OnFailure: (reason) =>
				{
					LoginToPlayfabWithDeviceID();
				}
			);
		}

		public static AuthenticationStatus GetAuthenticationStatus()
		{
			return authenticationStatus;
		}

		public enum AuthenticationStatus
		{
			NotStarted,
			LoggingIn,
			LoggedIn,
			ErrorLoggingIn
		}
	}
}