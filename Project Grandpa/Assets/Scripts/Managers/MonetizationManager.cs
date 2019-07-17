using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class MonetizationManager : Manager<MonetizationManager>
{
	public static string placementId = "video";
#if UNITY_ANDROID
	public static string unityGameId = "3185030";
#elif UNITY_IOS
	public static string unityGameId = "3185031";
#endif

	private void Start()
	{
		Monetization.Initialize(unityGameId, true);
	}

	private IEnumerator ShowAdWhenReady()
	{
		while (!Monetization.IsReady(placementId))
		{
			yield return new WaitForSeconds(0.25f);
		}

		ShowAdPlacementContent ad = null;
		ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

		if (ad != null)
		{
			ad.Show();
		}
	}

	public static void ShowAd()
	{
		Instance.StartCoroutine(Instance.ShowAdWhenReady());
	}
}
