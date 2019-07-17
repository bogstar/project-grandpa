using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingBar : MonoBehaviour
{
	[Header("Values")]
	[Tooltip("Defines how wide is the loading bar in pixels.")]
	[SerializeField] private float loadingBarWidth = 300f;
	[Header("References")]
	[SerializeField] private Image fillBar;

	private System.Func<float> GetCurrentPerc = null;
	private float currentPerc = 0f;

	private void Start()
	{
		fillBar.rectTransform.sizeDelta = new Vector2(-loadingBarWidth, fillBar.rectTransform.sizeDelta.y);
	}

	private void Update()
	{
		if (GetCurrentPerc == null || currentPerc >= 1f)
		{
			enabled = false;
			return;
		}

		currentPerc = GetCurrentPerc();
		float newWidth = Mathf.Clamp(loadingBarWidth * (1f - currentPerc), -loadingBarWidth, 0f);

		if (currentPerc < 0.01f)
		{
			fillBar.rectTransform.sizeDelta = new Vector2(-loadingBarWidth, fillBar.rectTransform.sizeDelta.y);
		}
		else
		{
			fillBar.rectTransform.sizeDelta = new Vector2(newWidth, fillBar.rectTransform.sizeDelta.y);
		}
	}

	public void AssignLoadingBar(System.Func<float> getCurrentPercCB)
	{
		GetCurrentPerc = getCurrentPercCB;
		enabled = true;
	}

	public void KillLoadingBar()
	{
		GetCurrentPerc = null;
		enabled = false;
	}
}