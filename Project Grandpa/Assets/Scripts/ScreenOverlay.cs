using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOverlay : Singleton<ScreenOverlay>
{
	[Header("Values")]
	[Tooltip("Defines the curve for alpha fading per percentage.")]
	[SerializeField] private AnimationCurve fadeCurve;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI versionLabel;
	[SerializeField] private Image fadeEffect;
	[SerializeField] private GameObject loadingContent;
	[SerializeField] private LoadingBar loadingBar;

	
	public void SetVersionLabel(string version)
	{
		versionLabel.text = "Version: " + version;
	}

	public void StartLoading(System.Func<float> callback)
	{
		loadingContent.SetActive(true);
		loadingBar.AssignLoadingBar(callback);
	}

	public void FinishLoading()
	{
		loadingContent.SetActive(false);
		loadingBar.KillLoadingBar();
	}

	public void FadeInstant(FadeType type)
	{
		switch (type)
		{
			case FadeType.In:
				fadeEffect.gameObject.SetActive(false);
				fadeEffect.color = new Color(0f, 0f, 0f, 0f);
				break;
			case FadeType.Out:
				fadeEffect.gameObject.SetActive(true);
				fadeEffect.color = new Color(0f, 0f, 0f, 1f);
				break;
		}
	}

	public void Fade(FadeType type, float duration, System.Action OnFinishedCallback = null)
	{
		switch (type)
		{
			case FadeType.In:
				StartCoroutine(Fade(0f, duration, () =>
				{
					fadeEffect.gameObject.SetActive(false);
					OnFinishedCallback?.Invoke();
				}));
				break;
			case FadeType.Out:
				fadeEffect.gameObject.SetActive(true);
				StartCoroutine(Fade(1f, duration, () =>
				{
					OnFinishedCallback?.Invoke();
				}));
				break;
		}
	}

	private IEnumerator Fade(float target, float duration, System.Action OnFinishedCallback = null)
	{
		float expiryTime = Time.unscaledTime + duration;
		float startTime = Time.unscaledTime;

		while (Time.unscaledTime <= expiryTime)
		{
			float percentageDone = Mathf.Clamp01(Mathf.InverseLerp(startTime, expiryTime, Time.unscaledTime));
			float alpha = fadeCurve.Evaluate(Mathf.Abs(target - 1f + percentageDone));
			fadeEffect.color = new Color(0f, 0f, 0f, alpha);

			yield return null;
		}

		fadeEffect.color = new Color(0f, 0f, 0f, target);

		OnFinishedCallback?.Invoke();
	}

	public enum FadeType
	{
		In, Out
	}
}