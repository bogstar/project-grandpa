using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grandpa/Chunk", fileName = "New Chunk")]
public class Chunk : ScriptableObject
{
	public int length;
	public List<Config> configs;

	private int currentIndex;

	public void Init()
	{
		currentIndex = -1;
	}

	public Config Next()
	{
		currentIndex++;

		if (currentIndex > length)
		{
			return null;
		}

		return configs[Random.Range(0, configs.Count)];
	}
}