using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
	public List<LevelGenerator.Segment> levelSegments = new List<LevelGenerator.Segment>();
	public float segmentLength = 8f;
	public int latestSegmentIndex = 0;
	public int levelLength = 20;
	public int laneCount = 3;
	public LevelSO levelSO;
	public Stage stage;
	public int seed;

	public GameObject LevelSegmentsHolder { get; private set; }

	public void Init(LevelSO levelSO, int stageIndex)
	{
		this.levelSO = levelSO;
		this.levelSO.SetStage(stageIndex);

		if (LevelSegmentsHolder != null)
		{
			if (Application.isPlaying)
			{
				Destroy(LevelSegmentsHolder);
			}
			else
			{
				DestroyImmediate(LevelSegmentsHolder);
			}
		}

		LevelSegmentsHolder = new GameObject("Level Holder");
	}

	public void SetSeed(int? seed = null)
	{
		if (!seed.HasValue)
			seed = System.DateTime.UtcNow.Millisecond;

		this.seed = seed.Value;
		ClearSegments();

		Random.InitState(this.seed);
	}

	public void GenerateNextSegment(bool visualize = true, bool generateObstacles = true)
	{
		var config = levelSO.currentStage.Next();

		var seggy = LevelGenerator.GenerateLevel(config);
		seggy.index = latestSegmentIndex;
		levelSegments.Add(seggy);

		LevelGenerator.Segment currentSegment = levelSegments[levelSegments.Count - 1];
		LevelGenerator.Segment prevSegment = (levelSegments.Count < 2) ? null : levelSegments[levelSegments.Count - 2];

		if (visualize)
		{
			Vector3 position = new Vector3(0, -0.5f, segmentLength * currentSegment.index);
			currentSegment.gameObject = Instantiate(levelSO.levelSegmentPrefab[0], LevelSegmentsHolder.transform);
			currentSegment.gameObject.name = "Segment " + currentSegment.index;
			currentSegment.gameObject.transform.localPosition = position;
		}

		if (visualize && prevSegment != null && prevSegment.gameObject == null)
		{
			visualize = false;
		}

		// Scenery
		if (prevSegment != null)
		{
			while (true)
			{
				var buildingInfo = levelSO.buildingPrefabs[Random.Range(0, levelSO.buildingPrefabs.Length)];
				if (prevSegment.leftLastBuilding + buildingInfo.size < segmentLength)
				{
					if (visualize)
					{
						var building = Instantiate(buildingInfo.prefab, prevSegment.gameObject.transform);
						building.transform.localPosition = new Vector3(-10f, 0f, prevSegment.leftLastBuilding);
					}
					prevSegment.leftLastBuilding += buildingInfo.size;
				}
				else
				{
					currentSegment.leftLastBuilding = prevSegment.leftLastBuilding - segmentLength;
					break;
				}
			}
			while (true)
			{
				var buildingInfo = levelSO.buildingPrefabs[Random.Range(0, levelSO.buildingPrefabs.Length)];
				if (prevSegment.rightLastBuilding + buildingInfo.size < segmentLength)
				{
					if (visualize)
					{
						var building = Instantiate(buildingInfo.prefab, prevSegment.gameObject.transform);
						building.transform.localPosition = new Vector3(10f, 0f, prevSegment.rightLastBuilding);
						building.transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					prevSegment.rightLastBuilding += buildingInfo.size;
				}
				else
				{
					currentSegment.rightLastBuilding = prevSegment.rightLastBuilding - segmentLength;
					break;
				}
			}
		}

		// More scenery
		if (Random.Range(0f, 1f) <= .5f)
		{
			if (visualize)
			{
				var newSide = Instantiate(levelSO.sidePrefabs[Random.Range(0, levelSO.buildingPrefabs.Length)], currentSegment.gameObject.transform);
				newSide.transform.localPosition = new Vector3(-8f, .5f, 4f);
				newSide.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			else
			{
				Random.Range(0, levelSO.buildingPrefabs.Length);
			}
		}

		if (Random.Range(0f, 1f) <= .5f)
		{
			if (visualize)
			{
				var newSide = Instantiate(levelSO.sidePrefabs[Random.Range(0, levelSO.buildingPrefabs.Length)], currentSegment.gameObject.transform);
				newSide.transform.localPosition = new Vector3(8f, .5f, 4f);
				newSide.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			}
			else
			{
				Random.Range(0, levelSO.buildingPrefabs.Length);
			}
		}

		if (generateObstacles)
		{
			// Obstacles generation
			List<List<LevelGenerator.Obstacle>> subsegments = new List<List<LevelGenerator.Obstacle>>();
			for (int j = 0; j < laneCount; j++)
			{
				var obstacle = GetObstacle(currentSegment, j);

				if (obstacle == null)
				{
					continue;
				}

				List<LevelGenerator.Obstacle> subsegment = new List<LevelGenerator.Obstacle>
			{
				obstacle
			};
				while (j < laneCount)
				{
					var nextObstacle = GetObstacle(currentSegment, j + 1);
					if (nextObstacle != null && nextObstacle.unpassable == obstacle.unpassable && nextObstacle.slidable == obstacle.slidable)
					{
						subsegment.Add(nextObstacle);
						j++;
					}
					else
					{
						break;
					}
				}

				subsegments.Add(subsegment);
			}

			// Obstacle visualization
			for (int j = 0; j < subsegments.Count; j++)
			{
				var subsegment = subsegments[j];
				List<LevelSO.ObstacleSegment> segments = new List<LevelSO.ObstacleSegment>();
				foreach (var obstacle in subsegment)
				{
					segments.Add(new LevelSO.ObstacleSegment() { slidable = obstacle.slidable, unpassable = obstacle.unpassable });
				}

				var obs = ObstacleManager.GetObstacle(new LevelSO.ObstacleInfo() { width = subsegment.Count, segments = segments }, new List<LevelSO.ObstacleInfo>(levelSO.obstaclePrefabs));

				if (obs.Count == 0)
				{
					if (subsegment.Count == 1)
					{
						throw new System.Exception("Something is not right.");
					}

					var lastElement = subsegment[subsegment.Count - 1];
					subsegments.Add(new List<LevelGenerator.Obstacle>() { lastElement });
					subsegment.RemoveAt(subsegment.Count - 1);
					var remainder = new List<LevelGenerator.Obstacle>(subsegment);
					subsegments.Add(subsegment);
				}
				else
				{
					if (visualize)
					{
						var prefabToInstantiate = obs[Random.Range(0, obs.Count)];
						var obstacleGameObject = Instantiate(prefabToInstantiate.prefab, currentSegment.gameObject.transform);
						Obstacle obstacle = obstacleGameObject.AddComponent<Obstacle>();
						obstacle.ObstacleInfo = prefabToInstantiate;
						obstacleGameObject.transform.localPosition = new Vector3((subsegment[0].index - 1) * 4f, 0.5f, segmentLength / 2);
					}
					else
					{
						Random.Range(0, obs.Count);
					}
				}
			}
		}

		latestSegmentIndex++;
	}

	public void ClearSegments()
	{
		latestSegmentIndex = 0;
		for (int i = levelSegments.Count - 1; i > -1; i--)
		{
			if (levelSegments[i].gameObject != null)
			{
				if (Application.isPlaying)
				{
					Destroy(levelSegments[i].gameObject);
				}
				else
				{
					DestroyImmediate(levelSegments[i].gameObject);
				}
			}
			levelSegments.RemoveAt(i);
		}
	}

	public void GenerateLevel(int segmentIndex)
	{
		if (Mathf.Abs(latestSegmentIndex - levelLength) == segmentIndex)
		{
			return;
		}

		if (segmentIndex + levelLength < latestSegmentIndex)
		{
			SetSeed(seed);
			latestSegmentIndex = 0;
			for (int i = levelSegments.Count - 1; i > -1; i--)
			{
				if (levelSegments[i].gameObject != null)
				{
					if (Application.isPlaying)
					{
						Destroy(levelSegments[i].gameObject);
					}
					else
					{
						DestroyImmediate(levelSegments[i].gameObject);
					}
				}
				levelSegments.RemoveAt(i);
			}
		}

		int emptyNeeded = segmentIndex - latestSegmentIndex;
		for (int i = 0; i < emptyNeeded; i++)
		{
			GenerateNextSegment(false);
		}

		// Deleting obsolete segments
		for (int i = levelSegments.Count - 1; i > -1; i--)
		{
			if (levelSegments[i].index < segmentIndex - 1)
			{
				if (levelSegments[i].gameObject != null)
				{
					if (Application.isPlaying)
					{
						Destroy(levelSegments[i].gameObject);
					}
					else
					{
						DestroyImmediate(levelSegments[i].gameObject);
					}
				}
				levelSegments.RemoveAt(i);
			}
		}

		int fullNeeded = levelLength + (segmentIndex - latestSegmentIndex);
		for (int i = 0; i < fullNeeded; i++)
		{
			GenerateNextSegment(true, latestSegmentIndex > 15);
		}
	}

	private LevelGenerator.Obstacle GetObstacle(LevelGenerator.Segment segment, int index)
	{
		foreach (var obs in segment.obstacles)
		{
			if (obs.index == index)
			{
				return obs;
			}
		}

		return null;
	}
}