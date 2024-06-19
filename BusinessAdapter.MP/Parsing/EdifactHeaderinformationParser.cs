// Copyright...: (c) Schleupen AG

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing;

using System;
using System.IO;

public class EdifactHeaderinformationParser : IEdifactHeaderinformationParser
{
	private EdifactSegment? unbSegment;
	private EdifactSegment? unhSegment;

	public void Parse(Stream stream)
	{
		using (EdifactParser parser = new EdifactParser(stream))
		{
			parser.RegisterSegmentparser(new ISegmentparser[] { new UnbSegmentparser(), new UnhSegmentparser() });
			try
			{
				unbSegment = parser.ReadNextSegment(0)?.Segment;
				unhSegment = parser.ReadNextSegment(0)?.Segment;

				if (unbSegment == null || unbSegment.Name != EdifactSegmentConstants.UnbName)
				{
					throw new InvalidOperationException("No UNB segment was found. Therefore, the message addressing cannot be parsed.");
				}

				if (unhSegment == null || unhSegment.Name != EdifactSegmentConstants.UnhName)
				{
					throw new InvalidOperationException("No UNH segment was found. Therefore, the message formatting information cannot be parsed.");
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new EdifactParsingException(ex.Message);
			}
		}
	}

	public DateTimeOffset GetErstellungszeitpunkt()
	{
		string? datum = unbSegment?.TryGetWertAusDatenelement(4, 1);
		string? uhrzeit = unbSegment?.TryGetWertAusDatenelement(4, 2);

		if (!int.TryParse(datum.AsSpan(0, 2), out int jahr))
		{
			throw new EdifactParsingException($"Error in EDIFACT creation date: The year '{jahr}' cannot be converted.");
		}

		if (!int.TryParse(datum.AsSpan(2, 2), out int monat))
		{
			throw new EdifactParsingException($"Error in EDIFACT creation date: The month '{monat}' cannot be converted.");
		}

		if (!int.TryParse(datum.AsSpan(4, 2), out int tag))
		{
			throw new EdifactParsingException($"Error in EDIFACT creation date: The day '{tag}' cannot be converted.");
		}

		if (!int.TryParse(uhrzeit.AsSpan(0, 2), out int stunde))
		{
			throw new EdifactParsingException($"Error in EDIFACT creation date: The hour '{stunde}' cannot be converted.");
		}

		if (!int.TryParse(uhrzeit.AsSpan(2, 2), out int minute))
		{
			throw new EdifactParsingException($"Error in EDIFACT creation date: The minute '{minute}' cannot be converted.");
		}

		DateTimeOffset erstellungszeitpunkt = new DateTimeOffset(2000 + jahr, monat, tag, stunde, minute, 0, TimeSpan.Zero).ToLocalTime();
		return erstellungszeitpunkt;
	}

	public string? GetDataformatname()
	{
		return unhSegment?.TryGetWertAusDatenelement(2, 1);
	}

	public string? GetAbsenderCodenummer()
	{
		return unbSegment?.TryGetWertAusDatenelement(2, 1);
	}

	public string? GetEmpfaengerCodenummer()
	{
		return unbSegment?.TryGetWertAusDatenelement(3, 1);
	}

	public CodeVergebendeStelle GetReceiverIdentificationNumberType()
	{
		return CreateCodeVergebendeStelleFuerEdifactUnbCode(unbSegment?.TryGetWertAusDatenelement(3, 2));
	}

	public string? GetApplicationReference()
	{
		return unbSegment?.TryGetWertAusDatenelement(7);
	}

	public string? GetDocumentnumber()
	{
		return unbSegment?.TryGetWertAusDatenelement(5);
	}

	private CodeVergebendeStelle CreateCodeVergebendeStelleFuerEdifactUnbCode(string? ediCode)
	{
		CodeVergebendeStelle? codeVergebendeStelle = Codenummer.CreateCodeVergebendeStelleAusEdifactUnbCode(ediCode);

		if (codeVergebendeStelle.HasValue)
		{
			return codeVergebendeStelle.Value;
		}

		throw new EdifactParsingException($"The Edifact code '{ediCode}' for the code issuing authority is not recognized.");
	}
}
