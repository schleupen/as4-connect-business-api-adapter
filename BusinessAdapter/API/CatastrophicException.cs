// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// 
	/// </summary>
	public class CatastrophicException : Exception
	{
		public CatastrophicException(string message)
			: base(message)
		{
		}
	}
}