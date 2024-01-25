// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	using System;
	using System.Collections.Generic;

	public class EdifactSegment
	{
		private readonly List<EdifactDatenelement> datenelemente = new();
		private readonly string name;

		public EdifactSegment(string name, IEnumerable<EdifactDatenelement> datenelemente)
		{
			ArgumentNullException.ThrowIfNull(datenelemente);
			
			this.datenelemente.AddRange(datenelemente);
			this.name = name;
		}

		public string Name => name;

		public string? TryGetWertAusDatenelement(int position, int gruppendatenelementPosition)
		{
			EdifactDatenelement? element = TryGetDatenelement(position, gruppendatenelementPosition);
			return element?.Wert;
		}

		private EdifactDatenelement? TryGetDatenelement(int position, int gruppendatenelementPosition)
		{
			foreach (EdifactDatenelement element in datenelemente)
			{
				if (element.MatchesPosition(position, gruppendatenelementPosition))
				{
					return element;
				}
			}

			return null;
		}

		public string? TryGetWertAusDatenelement(int position)
		{
			EdifactDatenelement? element = TryGetDatenelement(position);
			return element?.Wert;
		}

		private EdifactDatenelement? TryGetDatenelement(int position)
		{
			foreach (EdifactDatenelement element in datenelemente)
			{
				if (element.MatchesPosition(position))
				{
					return element;
				}
			}

			return null;
		}
	}
}