// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	public abstract class AbstractSegmentparser : ISegmentparser
	{
		public virtual string Segmentname => GetType().Name.Substring(0, 3).ToUpperInvariant();

		protected abstract SegmentStrukturInfo SegmentStrukturInfo { get; }

		public ParseSegmentResult Parse(int segmentNumber, EdifactParserConfiguration configuration, string[] composites)
		{
			int datenelementZaehler = 1;
			List<EdifactDatenelement> datenelemente = new List<EdifactDatenelement>();
			foreach (string composite in composites)
			{
				IEnumerable<EdifactDatenelement> datenelementeComposite = ProcessComposite(configuration, composite, datenelementZaehler);
				datenelemente.AddRange(datenelementeComposite);
				datenelementZaehler++;
			}

			EdifactSegment segment = new EdifactSegment(Segmentname, datenelemente);
			return new ParseSegmentResult(segment);
		}

		private IEnumerable<EdifactDatenelement> ProcessComposite(EdifactParserConfiguration parserConfiguration, string composite, int datenelementZaehler)
		{
			string[] data = SeparateElementsOfComposite(parserConfiguration, composite);

			if (data.Length > 1)
			{
				List<EdifactDatenelement> result = new List<EdifactDatenelement>();
				int position = 1;
				EdifactDatenelementgruppe gruppe = new EdifactDatenelementgruppe(datenelementZaehler);
				foreach (string item in data)
				{
					if (!string.IsNullOrEmpty(item))
					{
						result.Add(CreateEdifactDatenelement(parserConfiguration, item, position, gruppe));
					}

					position++;
				}

				return result;
			}

			string? wert = data.Length > 0 ? data[0] : null;
			if (string.IsNullOrEmpty(wert))
			{
				return Array.Empty<EdifactDatenelement>();
			}

			if (SegmentStrukturInfo.IsDataElementGroup(datenelementZaehler))
			{
				return new[] { CreateEdifactDatenelement(parserConfiguration, wert, 1, new EdifactDatenelementgruppe(datenelementZaehler)) };
			}

			return new[] { CreateEdifactDatenelement(parserConfiguration, wert, datenelementZaehler, null) };
		}

		private EdifactDatenelement CreateEdifactDatenelement(EdifactParserConfiguration parserConfiguration, string? wert, int position, EdifactDatenelementgruppe? gruppe)
		{
			SegmentStrukturInfoElement strukturInfo = SegmentStrukturInfo.GetDataElementInfo(gruppe, position);
			if (strukturInfo.IsNumeric())
			{
				wert = wert?.Replace(parserConfiguration.DecimalSeparator, ".", StringComparison.OrdinalIgnoreCase);
			}

			return new EdifactDatenelement(wert, position, gruppe);
		}

		private string[] SeparateElementsOfComposite(EdifactParserConfiguration parserConfiguration, string composite)
		{
			Regex dataElementRegex = parserConfiguration.GetDataElementRegex();
			string[] data = ReplaceEscapedDataElementSeparatorWithRaw(dataElementRegex.Split(composite), parserConfiguration);
			return data;
		}

		private static string[] ReplaceEscapedDataElementSeparatorWithRaw(IEnumerable<string> input, EdifactParserConfiguration configuration)
		{
			return input.Select(x => configuration
				.RegexReplace(x, "")
										 .Replace(
											 configuration.GetReleaseSeparatorFor(EdifactParserConfiguration.SeparatorType.Escaped),
											 configuration.GetReleaseSeparatorFor(EdifactParserConfiguration.SeparatorType.Raw), StringComparison.OrdinalIgnoreCase)).ToArray();
		}
	}
}