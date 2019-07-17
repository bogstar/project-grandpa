using Grandpa.UI;
using Grandpa.UserManagment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunManager : MonoBehaviour
{
	public float segmentLength = 8f;
	public Runner Runner;
	public Level Level;
	public LevelSO levelSO;

	public AudioClip music;
	public AudioClip loseMusic;
	private AudioSource musicSource;
	private AudioSource loseSource;

	public float score;

	public PlayGUI PlayGUI { get; private set; }
	public string GameOverText { get; private set; }


	private void Start()
	{
		PlayGUI = FindObjectOfType<PlayGUI>();

		PlayGUI.DisplayPlayLeaderboardsModal(true);
		ShowGameOverScreen(false);

		Level = gameObject.AddComponent<Level>();

		Runner.Init();

		musicSource = gameObject.AddComponent<AudioSource>();
		musicSource.volume = 0;
		musicSource.loop = true;
		loseSource = gameObject.AddComponent<AudioSource>();

		StartPose();
	}

	private void Update()
	{
		if (Runner.CurrentState == Runner.State.Running && !Paused)
		{
			PlayGUI.UpdateScore((int)score);
			score += (Time.deltaTime * 17.537f);
		}

		if (Runner.CurrentState != Runner.State.Dead && !Paused)
		{
			int segmentIndex = (int)Mathf.Floor(Runner.DistanceTravelled / segmentLength);
			Level.GenerateLevel(segmentIndex);

			var holder = Level.LevelSegmentsHolder.transform;

			holder.position = new Vector3(holder.position.x, holder.position.y, holder.position.z - Runner.MovementSpeed * Time.deltaTime);
		}
	}

	private IEnumerator audioFadeOut(AudioSource source)
	{
		while (source.volume > 0.05f)
		{
			yield return null;
			source.volume -= Time.deltaTime * 6f;
		}

		source.volume = 0f;
	}

	private IEnumerator audioFadeIn(AudioSource source)
	{
		while (source.volume < 0.95f)
		{
			yield return null;
			source.volume += Time.deltaTime * 6f;
		}

		source.volume = 1f;
	}

	public void Crashed()
	{
		//audioSource.clip = FindObjectOfType<RunManager>().impact;
		//audioSource.Play();

		Runner.UnregisterCrashedCallback(Crashed);
		PlayGUI.DisplayScore(false);
		loseSource.volume = 1f;
		loseSource.clip = loseMusic;
		loseSource.Play();
		StartCoroutine(audioFadeOut(musicSource));
		ShowGameOverScreen(true);
		PlayGUI.DisplayPauseButton(false);
		//isDead = true;

		Authentication.SaveHighscore((int)score);
		//MonetizationManager.ShowAd();
	}

	private void Restart()
	{
		Level.Init(levelSO, 0);
		Level.SetSeed();
		Level.ClearSegments();
		score = 0;

		Runner.StartRunning();
		PlayGUI.DisplayPauseButton(true);

		//ShowPlayAndLeaderboards(false);
		PlayGUI.DisplayScore(true);
		musicSource.clip = music;
		musicSource.Play();
		StartCoroutine(audioFadeOut(loseSource));
		StartCoroutine(audioFadeIn(musicSource));
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
	}

	public void ShowGameOverScreen(bool show)
	{
		if (show)
		{
			GameOverText = "It doesn't matter though. You landed on the leaderboards anyway. Check your standings.\nScore:\n"
				+ (int)score;
			PlayGUI.DisplayScore(false);
		}
		else
		{
			if (Runner.CurrentState == Runner.State.Running)
			{
				PlayGUI.DisplayScore(true);
			}
		}
		PlayGUI.DisplayGameOverModal(show);
	}

	public void ButtonPlay()
	{
		Restart();
	}

	public bool Paused { get; private set; }

	public void ButtonPause()
	{
		musicSource.Pause();
		Paused = true;
	}

	public void ButtonUnpause()
	{
		musicSource.Play();
		Paused = false;
	}

	public void StartPose()
	{
		Level.Init(levelSO, 0);
		Level.ClearSegments();
		PlayGUI.DisplayPauseButton(false);
		Runner.RegisterCrashedCallback(Crashed);
		Runner.StartPose();
	}

	public void OnButtonGameOverBack()
	{
		AudioManager.PlayClipStackable(AudioManager.Audio.Click);
		ShowGameOverScreen(false);
	}

	/*
	private void GenerateLevel(float pos)
	{
		int segmentIndex = (int)Mathf.Floor(pos / segmentLength);

		if (currentSegment == segmentIndex && levelSegments.Count > 0)
		{
			return;
		}

		currentSegment = segmentIndex;

		for (int i = segmentIndex; i < segmentIndex + levelLength; i++)
		{
			if (LevelGenerator.Segment.GetExistingSegment(levelSegments, i) == null)
			{
				var seggy = LevelGenerator.GenerateLevel(i);
				levelSegments.Add(seggy);
			}
		}

		//LevelGenerator.GenerateLevel(levelSegments, segmentIndex, Runner.CurrentState == Runner.State.Posing, levelLength);

		// Deleting obsolete segments
		for (int i = levelSegments.Count - 1; i > -1; i--)
		{
			if (levelSegments[i].index < segmentIndex - 1)
			{
				if (levelSegments[i].gameObject != null)
				{
					Destroy(levelSegments[i].gameObject);
				}
				levelSegments.RemoveAt(i);
			}
		}

		// Creating visuals
		for (int i = 0; i < levelSegments.Count; i++)
		{
			// If this segment has already been visualized, skip!
			if (levelSegments[i].gameObject != null)
			{
				continue;
			}

			LevelGenerator.Segment currentSegment = levelSegments[i];
			LevelGenerator.Segment prevSegment = (i == 0) ? null : levelSegments[i - 1];

			Vector3 position = new Vector3(0, -0.5f, segmentLength * currentSegment.index);
			currentSegment.gameObject = Instantiate(levelSegment);
			currentSegment.gameObject.name = "Segment " + currentSegment.index;
			currentSegment.gameObject.transform.position = position;

			// Scenery
			if (prevSegment != null)
			{
				while (true)
				{
					var buildingInfo = buildingsPrefabs[Random.Range(0, buildingsPrefabs.Length)];
					if (prevSegment.leftLastBuilding + buildingInfo.size < segmentLength)
					{
						var building = Instantiate(buildingInfo.prefab, prevSegment.gameObject.transform);
						building.transform.localPosition = new Vector3(-10f, 0f, prevSegment.leftLastBuilding);
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
					var buildingInfo = buildingsPrefabs[Random.Range(0, buildingsPrefabs.Length)];
					if (prevSegment.rightLastBuilding + buildingInfo.size < segmentLength)
					{
						var building = Instantiate(buildingInfo.prefab, prevSegment.gameObject.transform);
						building.transform.localPosition = new Vector3(10f, 0f, prevSegment.rightLastBuilding);
						building.transform.localScale = new Vector3(-1f, 1f, 1f);
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
				var newSide = Instantiate(sidePrefabs[Random.Range(0, sidePrefabs.Length)], currentSegment.gameObject.transform);
				newSide.transform.localPosition = new Vector3(-8f, .5f, 4f);
				newSide.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}

			if (Random.Range(0f, 1f) <= .5f)
			{
				var newSide = Instantiate(sidePrefabs[Random.Range(0, sidePrefabs.Length)], currentSegment.gameObject.transform);
				newSide.transform.localPosition = new Vector3(8f, .5f, 4f);
				newSide.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			}

			if (Runner.CurrentState == Runner.State.Posing)
			{
				continue;
			}

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
				List<ObstacleManager.ObstacleSegment> segments = new List<ObstacleManager.ObstacleSegment>();
				foreach (var obstacle in subsegment)
				{
					segments.Add(new ObstacleManager.ObstacleSegment() { slidable = obstacle.slidable, unpassable = obstacle.unpassable });
				}

				var obstacleManager = FindObjectOfType<ObstacleManager>();
				var obs = obstacleManager.GetObstacle(new ObstacleManager.Obstacle() { width = subsegment.Count, segments = segments });
				
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
					var prefabToInstantiate = obs[Random.Range(0, obs.Count)];
					var obstacleGameObject = Instantiate(prefabToInstantiate.prefab, currentSegment.gameObject.transform);
					Obstacle obstacle = obstacleGameObject.AddComponent<Obstacle>();
					obstacle.ObstacleInfo = prefabToInstantiate;
					obstacleGameObject.transform.localPosition = new Vector3((subsegment[0].index - 1) * 4f, 0.5f, segmentLength / 2);
				}
			}
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
	}*/

	public void Left()
	{
		if (!Paused)
		{
			Runner.Left();
		}
	}

	public void Right()
	{
		if (!Paused)
		{
			Runner.Right();
		}
	}

	public void Down()
	{
		if (!Paused)
		{
			Runner.Down();
		}
	}
}