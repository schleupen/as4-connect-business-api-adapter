// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	using System;
	using System.IO;

	public interface IEdifactHeaderinformationParser
	{
		void Parse(Stream stream);

		string? GetDataformatname();

		DateTimeOffset GetErstellungszeitpunkt();

		string? GetAbsenderCodenummer();

		string? GetEmpfaengerCodenummer();

		CodeVergebendeStelle GetReceiverIdentificationNumberType();

		string? GetDocumentnumber();

		string? GetApplicationReference();
	}
}