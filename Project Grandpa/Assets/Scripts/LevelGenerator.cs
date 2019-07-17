using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
	public static Segment GenerateLevel(Config config)
	{
		Segment segment = new Segment();

		for (int i = 0; i < config.lanes.Count; i++)
		{
			var lane = config.lanes[i];

			if (lane.obstacle != null && (lane.obstacle.slidable || lane.obstacle.unpassable))
			{
				Obstacle obstacle = new Obstacle
				{
					index = i,
					slidable = lane.obstacle.slidable,
					unpassable = lane.obstacle.unpassable
				};

				segment.obstacles.Add(obstacle);
			}
		}

		return segment;
	}

    public static void GenerateLevel(List<Segment> levelSegments, int segmentIndex, bool isPosing, int levelLength)
	{
		for (int i = segmentIndex; i < segmentIndex + levelLength; i++)
		{
			if (Segment.GetExistingSegment(levelSegments, i) == null)
			{
				Segment newSegment = new Segment();

				newSegment.index = i;

				Obstacle obstacle = new Obstacle();

				if (newSegment.index > 15 && !isPosing)
				{
					if (Segment.GetExistingSegment(levelSegments, i - 1) != null && Segment.GetExistingSegment(levelSegments, i - 1).obstacles.Count == 0)
					{
						List<WeightedChance> chances = new List<WeightedChance>();
						chances.Add(new WeightedChance() { amount = 0, chance = 3 });
						chances.Add(new WeightedChance() { amount = 1, chance = 5 });
						chances.Add(new WeightedChance() { amount = 2, chance = 2 });

						int amount = GetAmountWithWeight(chances);

						switch (amount)
						{
							case 1:
								obstacle = new Obstacle();
								obstacle.index = Random.Range(0, 3);
								if (Random.Range(0f, 1f) < 0.2f)
								{
									obstacle.unpassable = false;
									obstacle.slidable = true;
								}
								newSegment.obstacles.Add(obstacle);
								break;
							case 2:
								int ob1In = -1;
								int ob2In = -1;
								switch (Random.Range(0, 3))
								{
									case 0:
										ob1In = 0;
										ob2In = 1;
										break;
									case 1:
										ob1In = 0;
										ob2In = 2;
										break;
									case 2:
										ob1In = 2;
										ob2In = 1;
										break;
								}
								obstacle = new Obstacle();
								obstacle.index = ob1In;
								if (Random.Range(0f, 1f) < 0.2f)
								{
									obstacle.unpassable = false;
									obstacle.slidable = true;
								}
								newSegment.obstacles.Add(obstacle);
								obstacle = new Obstacle();
								obstacle.index = ob2In;
								if (Random.Range(0f, 1f) < 0.2f)
								{
									obstacle.unpassable = false;
									obstacle.slidable = true;
								}
								newSegment.obstacles.Add(obstacle);
								break;
						}

					}
					else if (Segment.GetExistingSegment(levelSegments, i - 1) != null && Segment.GetExistingSegment(levelSegments, i - 1).obstacles.Count == 1)
					{
						List<WeightedChance> chances = new List<WeightedChance>();
						chances.Add(new WeightedChance() { amount = 0, chance = 3 });
						chances.Add(new WeightedChance() { amount = 1, chance = 5 });
						chances.Add(new WeightedChance() { amount = 2, chance = 2 });

						int amount = GetAmountWithWeight(chances);

						/*
						IF 1
							IF B
								YES - (A || C) || (A & C)
							IF A
								YES - 33%(50%B || 50%C) ||
								33%(B & C) ||
								33%0
							IF C
								YES - (A || B) || (A & B)*/

						switch (Segment.GetExistingSegment(levelSegments, i - 1).obstacles[0].index)
						{
							// AKO JE SEGMENT A
							case 0:
								switch (Random.Range(0, 3))
								{
									// AKO JE SAMO JEDAN
									case 1:
										switch (Random.Range(0, 2))
										{
											// AKO JE B
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
									// AKO SU 2
									case 2:
										obstacle = new Obstacle();
										obstacle.index = 1;
										if (Random.Range(0f, 1f) < 0.2f)
										{
											obstacle.unpassable = false;
											obstacle.slidable = true;
										}
										newSegment.obstacles.Add(obstacle);
										obstacle = new Obstacle();
										obstacle.index = 2;
										if (Random.Range(0f, 1f) < 0.2f)
										{
											obstacle.unpassable = false;
											obstacle.slidable = true;
										}
										newSegment.obstacles.Add(obstacle);
										break;
								}
								break;
							// AKO JE SEGMENT C
							case 2:
								switch (Random.Range(0, 3))
								{
									// AKO JE SAMO JEDAN
									case 1:
										switch (Random.Range(0, 2))
										{
											// AKO JE A
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE B
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
									// AKO SU 2
									case 2:
										obstacle = new Obstacle();
										obstacle.index = 0;
										if (Random.Range(0f, 1f) < 0.2f)
										{
											obstacle.unpassable = false;
											obstacle.slidable = true;
										}
										newSegment.obstacles.Add(obstacle);
										obstacle = new Obstacle();
										obstacle.index = 1;
										if (Random.Range(0f, 1f) < 0.2f)
										{
											obstacle.unpassable = false;
											obstacle.slidable = true;
										}
										newSegment.obstacles.Add(obstacle);
										break;
								}
								break;
							// AKO JE SEGMENT B
							case 1:
								switch (Random.Range(0, 3))
								{
									// AKO JE SAMO JEDAN
									case 1:
										switch (Random.Range(0, 2))
										{
											// AKO JE A
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
									// AKO SU 2
									case 2:
										obstacle = new Obstacle();
										obstacle.index = 0;
										if (Random.Range(0f, 1f) < 0.2f)
										{
											obstacle.unpassable = false;
											obstacle.slidable = true;
										}
										newSegment.obstacles.Add(obstacle);
										obstacle = new Obstacle();
										obstacle.index = 2;
										if (Random.Range(0f, 1f) < 0.2f)
										{
											obstacle.unpassable = false;
											obstacle.slidable = true;
										}
										newSegment.obstacles.Add(obstacle);
										break;
								}
								break;
						}
					}
					else if (Segment.GetExistingSegment(levelSegments, i - 1) != null && Segment.GetExistingSegment(levelSegments, i - 1).obstacles.Count == 2)
					{
						List<WeightedChance> chances = new List<WeightedChance>();
						chances.Add(new WeightedChance() { amount = 0, chance = 3 });
						chances.Add(new WeightedChance() { amount = 1, chance = 5 });
						chances.Add(new WeightedChance() { amount = 2, chance = 2 });

						int amount = GetAmountWithWeight(chances);

						/*
						IF 2
							IF A & B
								(A || C || B) || (A & B) || (A & C)
							IF A & C
								(A || C || B) || (A & B) || (A & C) || (B & C)
							IF B & C
								(A || C || B) || (A & C) || (B & C)*/

						switch (Segment.GetExistingSegment(levelSegments, i - 1).obstacles[0].index + Segment.GetExistingSegment(levelSegments, i - 1).obstacles[1].index)
						{
							// AKO SU SEGMENT A i B
							case 0:
								switch (Random.Range(0, 3))
								{
									// AKO JE SAMO JEDAN
									case 1:
										switch (Random.Range(0, 3))
										{
											// AKO JE B
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE A
											case 2:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
									// AKO SU 2
									case 2:
										switch (Random.Range(0, 2))
										{
											// AKO SU A i B
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE A i C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
								}
								break;
							// AKO SU SEGMENT B i C
							case 3:
								switch (Random.Range(0, 3))
								{
									// AKO JE SAMO JEDAN
									case 1:
										switch (Random.Range(0, 3))
										{
											// AKO JE B
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE A
											case 2:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
									// AKO SU 2
									case 2:
										switch (Random.Range(0, 2))
										{
											// AKO SU A i C
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE B i C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
								}
								break;
							// AKO SU SEGMENT A i C
							case 2:
								switch (Random.Range(0, 3))
								{
									// AKO JE SAMO JEDAN
									case 1:
										switch (Random.Range(0, 3))
										{
											// AKO JE B
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE A
											case 2:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
									// AKO SU 2
									case 2:
										switch (Random.Range(0, 3))
										{
											// AKO SU A i C
											case 0:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE B i C
											case 1:
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 2;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
											// AKO JE A i B
											case 2:
												obstacle = new Obstacle();
												obstacle.index = 0;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												obstacle = new Obstacle();
												obstacle.index = 1;
												if (Random.Range(0f, 1f) < 0.2f)
												{
													obstacle.unpassable = false;
													obstacle.slidable = true;
												}
												newSegment.obstacles.Add(obstacle);
												break;
										}
										break;
								}
								break;
						}
					}
				}

				levelSegments.Add(newSegment);
			}
		}
	}

	struct WeightedChance
	{
		public int chance;
		public int amount;
	}

	private static int GetAmountWithWeight(List<WeightedChance> chances)
	{
		int totalChance = 0;

		foreach (var chance in chances)
		{
			totalChance += chance.chance;
		}

		int randomNumber = Random.Range(1, totalChance + 1);

		foreach (var chance in chances)
		{
			if (randomNumber > chance.chance)
			{
				randomNumber -= chance.chance;
				continue;
			}

			return chance.amount;
		}

		return -1;
	}

	[System.Serializable]
	public class Segment
	{
		public int index;
		public List<Obstacle> obstacles;
		public GameObject gameObject;
		public float leftLastBuilding;
		public float rightLastBuilding;

		public Segment()
		{
			obstacles = new List<Obstacle>();
		}

		public static Segment GetExistingSegment(List<Segment> segments, int index)
		{
			for (int i = 0; i < segments.Count; i++)
			{
				if (segments[i].index == index)
				{
					return segments[i];
				}
			}

			return null;
		}
	}

	[System.Serializable]
	public class Obstacle
	{
		public int index;
		public bool unpassable;
		public bool slidable;

		public Obstacle()
		{
			unpassable = true;
		}
	}
}