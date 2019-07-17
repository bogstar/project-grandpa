using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grandpa/Level", fileName = "New Level")]
public class LevelSO : ScriptableObject
{
	[Header("Values")]
	[Tooltip("Level Name.")]
	public new string name;
	public List<Stage> stages;

	// Prefabs
	[Header("Prefabs")]
	public ObstacleInfo[] obstaclePrefabs;
	public GameObject[] sidePrefabs;
	public GameObject[] levelSegmentPrefab;
	public BuildingInfo[] buildingPrefabs;
	// Prefabs END

	public Stage currentStage { get; private set; }

	public void SetStage(int index)
	{
		if (index > stages.Count - 1 || index < 0)
		{
			return;
		}

		currentStage = stages[index];
		currentStage.Init();
		currentStage.Next();
	}

	[System.Serializable]
	public struct BuildingInfo
	{
		[Tooltip("Building length in units.")]
		public float size;
		public GameObject prefab;
	}

	[System.Serializable]
	public struct ObstacleInfo
	{
		public GameObject prefab;
		public AnimationClip deathClip;
		public int width;
		public int length;
		public List<ObstacleSegment> segments;
	}

	[System.Serializable]
	public struct ObstacleSegment
	{
		public bool unpassable;
		public bool slidable;

		public override bool Equals(object obj)
		{
			if (!(obj is ObstacleSegment))
			{
				return false;
			}

			var segment = (ObstacleSegment)obj;
			return unpassable == segment.unpassable &&
				   slidable == segment.slidable;
		}

		public override int GetHashCode()
		{
			var hashCode = 1679215154;
			hashCode = hashCode * -1521134295 + unpassable.GetHashCode();
			hashCode = hashCode * -1521134295 + slidable.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(ObstacleSegment obj1, ObstacleSegment obj2)
		{
			return obj1.slidable == obj2.slidable && obj1.unpassable == obj2.unpassable;
		}

		public static bool operator !=(ObstacleSegment obj1, ObstacleSegment obj2)
		{
			return !(obj1.slidable == obj2.slidable && obj1.unpassable == obj2.unpassable);
		}
	}
}