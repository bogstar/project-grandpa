using Grandpa.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
	public static Version GameVersion { get; private set; }

	public static void SetVersion(string version)
	{
		var split = version.Split('-');
		var newGameVersion = new Version();
		if (split.Length < 2)
		{
			newGameVersion = new Version()
			{
				type = Version.Type.Release,
				code = version
			};
		}
		else
		{
			newGameVersion.code = split[0];
			switch (split[1])
			{
				case "internal":
					newGameVersion.type = Version.Type.Internal;
					break;
				case "alpha":
					newGameVersion.type = Version.Type.Alpha;
					break;
				case "beta":
					newGameVersion.type = Version.Type.Beta;
					break;
			}
		}

		GameVersion = newGameVersion;
		ScreenOverlay.Instance.SetVersionLabel(GameVersion.GetFormattedString());
	}
}