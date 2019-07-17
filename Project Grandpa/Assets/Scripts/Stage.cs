using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grandpa/Stage", fileName = "New Stage")]
public class Stage : ScriptableObject
{
	public List<Chunk> chunks;

	private int currentIndex = 0;

	public void Init()
	{
		currentIndex = 0;
		chunks[currentIndex].Init();
	}

	public Config Next()
	{
		var next = chunks[currentIndex].Next();

		if (next == null)
		{
			currentIndex++;

			if (currentIndex > chunks.Count - 1)
			{
				return null;
			}

			chunks[currentIndex].Init();
			return chunks[currentIndex].Next();
		}

		return next;
	}
}