// Copyright...: (c) Schleupen AG

namespace Schleupen.AS4.BusinessAdapter.Parsing;

using System;
using System.IO;

public class EdifactHeaderinformationParser : IEdifactHeaderinformationParser
{
	private readonly EdifactSegment? unbSegment;
	private readonly EdifactSegment? unhSegment;

	public EdifactHeaderinformationParser(Stream stream)
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
					throw new InvalidOperationException("Es wurde kein UNB-Segment gefunden. Die Adressierung der Meldung kann daher nicht geparst werden.");
				}

				if (unhSegment == null || unhSegment.Name != EdifactSegmentConstants.UnhName)
				{
					throw new InvalidOperationException("Es wurde kein UNH-Segment gefunden. Die Formatinformationen der Meldung können daher nicht geparst werden.");
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
			throw new EdifactParsingException($"Fehler im EDIFACT-Erstellungsdatum: Das Jahr '{jahr}' kann nicht umgewandelt werden.");
		}

		if (!int.TryParse(datum.AsSpan(2, 2), out int monat))
		{
			throw new EdifactParsingException($"Fehler im EDIFACT-Erstellungsdatum: Der Monat '{monat}' kann nicht umgewandelt werden.");
		}

		if (!int.TryParse(datum.AsSpan(4, 2), out int tag))
		{
			throw new EdifactParsingException($"Fehler im EDIFACT-Erstellungsdatum: Der Tag '{tag}' kann nicht umgewandelt werden.");
		}

		if (!int.TryParse(uhrzeit.AsSpan(0, 2), out int stunde))
		{
			throw new EdifactParsingException($"Fehler im EDIFACT-Erstellungsdatum: Die Stunde '{stunde}' kann nicht umgewandelt werden.");
		}

		if (!int.TryParse(uhrzeit.AsSpan(2, 2), out int minute))
		{
			throw new EdifactParsingException($"Fehler im EDIFACT-Erstellungsdatum: Die Minute '{minute}' kann nicht umgewandelt werden.");
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

		throw new EdifactParsingException($"Der Edifact-Code '{ediCode}' für die Codevergebende Stelle ist nicht bekannt.");
	}
}