using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
	//public List<Segment> segments;
	//public List<Obstacle> obstacles;

	public static List<LevelSO.ObstacleInfo> GetObstacle(LevelSO.ObstacleInfo obstaclePrototype, List<LevelSO.ObstacleInfo> obstacles)
	{
		List<LevelSO.ObstacleInfo> allObstacles = new List<LevelSO.ObstacleInfo>();

		foreach (var obstacle in obstacles)
		{
			if (obstacle.width == obstaclePrototype.width)
			{
				bool okay = true;
				for (int i = 0; i < obstacle.segments.Count; i++)
				{
					if (obstacle.segments[i] != obstaclePrototype.segments[i])
					{
						okay = false;
						break;
					}
				}

				if (okay)
				{
					allObstacles.Add(obstacle);
				}
			}
		}

		return allObstacles;
	}
}