using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grandpa/Config", fileName = "New Config")]
public class Config : ScriptableObject
{
	public List<Lane> lanes = new List<Lane>();

	[System.Serializable]
	public struct Lane
	{
		public LevelGenerator.Obstacle obstacle;
	}
}