namespace Grandpa.Models
{
	[System.Serializable]
	public struct Version
	{
		public Type type;
		public string code;
		public string patchNotes;

		public string GetFormattedString()
		{
			return code + GetSuffix(type);
		}

		public enum Type
		{
			Internal, Alpha, Beta, Release
		}

		private string GetSuffix(Type type)
		{
			switch (type)
			{
				case Type.Alpha:
					return "-alpha";
				case Type.Internal:
					return "-internal";
				case Type.Beta:
					return "-beta";
			}

			return "";
		}

		public string GetTypeStringFormatted()
		{
			switch (type)
			{
				case Type.Alpha:
					return "alpha";
				case Type.Internal:
					return "internal";
				case Type.Beta:
					return "beta";
				case Type.Release:
					return "release";
			}

			return "";
		}
	}
}