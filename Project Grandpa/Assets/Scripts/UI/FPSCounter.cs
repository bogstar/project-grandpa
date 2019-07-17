using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
	public TextMeshProUGUI text;

	private int lastFramesCount = 50;

	List<int> lastFrames = new List<int>();

	private float refreshRate = 1f;
	private float timeToUpdate;

	private void Update()
	{
		int currentFps = (int)Mathf.Ceil(1f / Time.unscaledDeltaTime);
		lastFrames.Add(currentFps);
		if (lastFrames.Count > lastFramesCount)
		{
			for (int i = lastFrames.Count - lastFramesCount; i > -1; i--)
			{
				lastFrames.RemoveAt(i);
			}
		}

		float average = 0;
		foreach (var fps in lastFrames)
		{
			average += fps;
		}
		if (lastFrames.Count > 0)
		{
			average /= lastFrames.Count;
		}

		if (Time.time > timeToUpdate)
		{
			timeToUpdate = Time.time + refreshRate;
			text.text = "FPS: " + ((int)average).ToString();
		}
	}
}
