using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grandpa.Editor.LevelGeneration
{
	public class LevelBuilderEditor : EditorWindow
	{
		private static Level level;
		private static LevelSO levelSO;
		private static int stageIndex;
		private static int levelLength = 20;
		private static int offset = 0;
		private static bool randomSeed = true;
		private static int seed = 0;
		private static bool autoGenerate = false;

		[MenuItem("Grandpa/Level Builder")]
		public static void ShowWindow()
		{
			var win = GetWindow(typeof(LevelBuilderEditor), false, "Level Builder");

			win.Show();
		}

		private void OnDestroy()
		{
			if (level != null)
				DestroyImmediate(level.gameObject);
		}

		private void OnGUI()
		{
			levelSO = (LevelSO)EditorGUILayout.ObjectField("Level Scriptable Object", levelSO, typeof(LevelSO), true);
			stageIndex = EditorGUILayout.IntField("Stage Index", stageIndex);
			levelLength = EditorGUILayout.IntField("Level Length", levelLength);
			offset = EditorGUILayout.IntField("Offset", offset);

			if (autoGenerate) GUI.enabled = false;
			randomSeed = EditorGUILayout.Toggle("Random Seed", randomSeed);
			if (autoGenerate) GUI.enabled = true;

			if (!randomSeed)
				seed = EditorGUILayout.IntField("Seed", seed);

			GUILayout.BeginHorizontal();
			if (autoGenerate) GUI.enabled = false;
			if (GUILayout.Button("Generate Level"))
				GenerateLevel();
			if (autoGenerate) GUI.enabled = true;

			if (randomSeed) GUI.enabled = false;
			autoGenerate = EditorGUILayout.Toggle("Auto Generate:", autoGenerate);
			if (randomSeed) GUI.enabled = true;

			GUILayout.EndHorizontal();

			if (autoGenerate)
				GenerateLevel();
		}

		private void GenerateLevel()
		{
			if (level == null)
			{
				var levelEditorBuilder = new GameObject("Editor Level Builder");
				level = levelEditorBuilder.AddComponent<Level>();
				level.Init(levelSO, stageIndex);
				level.SetSeed(randomSeed ? null : new int?(seed.GetHashCode()));
			}

			if (seed != level.seed)
				level.SetSeed(randomSeed ? null : new int?(seed.GetHashCode()));

			level.levelLength = levelLength;
			level.GenerateLevel(offset);
			level.LevelSegmentsHolder.transform.SetParent(level.transform);
			level.LevelSegmentsHolder.transform.position = new Vector3(level.LevelSegmentsHolder.transform.position.x, level.LevelSegmentsHolder.transform.position.y, - offset * level.segmentLength);
		}
	}
}