// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class SegmentStrukturInfo
	{
		private readonly List<SegmentStrukturInfoElement> elements = new();
		private bool alreadyCreated;
		private SegmentStrukturInfoElementgruppe? currentGroup;
		private int? currentDataElementGroupPosition;
		private int? currentDataElementPosition;
		

		public SegmentStrukturInfo StartDataelementgroup(string name)
		{
			EnsureNotAlreadyCreated();

			currentDataElementGroupPosition = currentDataElementPosition + 1 ?? 1;
			currentDataElementPosition = null;

			currentGroup = new SegmentStrukturInfoElementgruppe(currentDataElementGroupPosition.Value, name);

			return this;
		}

		public SegmentStrukturInfo AddDataElement(string name)
		{
			return AddDataElement(name, null);
		}

		public SegmentStrukturInfo CloseDataElementGroup()
		{
			EnsureNotAlreadyCreated();

			if (currentGroup == null)
			{
				throw new InvalidOperationException("There is no open data element group", null);
			}

			currentDataElementPosition = currentDataElementGroupPosition;
			currentDataElementGroupPosition = null;
			currentGroup = null;

			return this;
		}

		public SegmentStrukturInfo Build()
		{
			currentGroup = null;
			currentDataElementPosition = null;
			currentDataElementGroupPosition = null;

			alreadyCreated = true;
			return this;
		}

		public bool IsDataElementGroup(int position)
		{
			SegmentStrukturInfoElement? datenelement = TryGetDataElement(position);
			return datenelement == null;
		}

		public SegmentStrukturInfoElement GetDataElementInfo(EdifactDatenelementgruppe? gruppe, int position)
		{
			return gruppe == null ? GetDataElementInfo(position) : gruppe.GetDataElementInfo(this, position);
		}

		public SegmentStrukturInfoElement GetDataElementInfo(int position, int gruppendatenelementPosition)
		{
			return GetDataElement(position, gruppendatenelementPosition);
		}

		private SegmentStrukturInfoElement GetDataElementInfo(int position)
		{
			return GetDataElement(position);
		}

		private SegmentStrukturInfoElement GetDataElement(int position)
		{
			SegmentStrukturInfoElement? result = TryGetDataElement(position);

			if (result != null)
			{
				return result;
			}

			throw new InvalidOperationException($"There is no data element at position {position}.");
		}

		private SegmentStrukturInfoElement GetDataElement(int position, int groupDataElementPosition)
		{
			foreach (SegmentStrukturInfoElement element in elements)
			{
				if (element.MatchesPosition(position, groupDataElementPosition))
				{
					return element;
				}
			}

			throw new InvalidOperationException($"There is no data element at position {groupDataElementPosition} in the group at position {position}.");
		}

		private SegmentStrukturInfoElement? TryGetDataElement(int position)
		{
			return elements.FirstOrDefault(element => element.MatchesPosition(position));
		}

		private void EnsureNotAlreadyCreated()
		{
			if (alreadyCreated)
			{
				throw new InvalidOperationException("The structure description was already created.", null);
			}
		}

		private SegmentStrukturInfo AddDataElement(string name, SegmentStrukturInfoElement.Elementtyp? typ)
		{
			EnsureNotAlreadyCreated();

			currentDataElementPosition = currentDataElementPosition + 1 ?? 1;
			elements.Add(new SegmentStrukturInfoElement(currentDataElementPosition.Value, name, currentGroup, typ));

			return this;
		}
	}
}