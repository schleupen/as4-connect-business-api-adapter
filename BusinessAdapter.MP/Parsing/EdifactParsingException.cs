// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing
{
	/// <summary>
	/// An exception that occurs during the parsing of EDIFACT.
	/// </summary>
	[Serializable]
	public class EdifactParsingException : Exception
	{
		public EdifactParsingException(string message)
			: base(message)
		{
		}
	}
}