namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Microsoft.Extensions.Options;

public class FpFileNameExtractor(IFpFileParser fpFileParser, IOptions<EICMapping> eicMapping) : IFpFileNameExtractor
{
	public FpFileName ExtractFileName(InboxFpMessage fpMessage)
	{
		var parsedFile = fpFileParser.ParseCompressedPayload(fpMessage.Payload);

		var senderParty = eicMapping.Value.GetPartyOrDefault(parsedFile.Sender);
		if (senderParty == null)
		{
			throw new InvalidDataException($"Unable to find mapping for MP: {parsedFile.Sender}");
		}

		return new FpFileName()
		{
			MessageType = ToMessageType(fpMessage.BDEWProperties.BDEWDocumentType),
			EicNameBilanzkreis = senderParty.Bilanzkreis,
			EicNameTso = parsedFile.Sender.Code,
			Timestamp = parsedFile.ValidityDate,
			Date = parsedFile.CreationDate,
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			FahrplanHaendlerTyp = senderParty.FpType
		};
	}

	private FpMessageType ToMessageType(string bdewDocumentType)
	{
		switch (bdewDocumentType)
		{
			case "A07":
			case "A08":
			case "A09":
				return FpMessageType.Confirmation;
			case "A01":
				return FpMessageType.Schedule;
			case "A17":
				return FpMessageType.Acknowledge;
			case "A16":
				return FpMessageType.Anomaly;
			case "A59": // prozessbeschreibung_fahrplananmeldung_v4.5 in A.4.1.1
				return FpMessageType.Status;
			default:
				throw new NotSupportedException($"Document type {bdewDocumentType} is not supported.");
		}
	}
}