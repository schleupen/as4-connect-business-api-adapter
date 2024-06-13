// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing;

using System;

public interface IEdifactHeaderinformationParser
{
	string? GetDataformatname();

	DateTimeOffset GetErstellungszeitpunkt();

	string? GetAbsenderCodenummer();

	string? GetEmpfaengerCodenummer();

	CodeVergebendeStelle GetReceiverIdentificationNumberType();

	string? GetDocumentnumber();

	string? GetApplicationReference();
}