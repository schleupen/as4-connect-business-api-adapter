namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFileNameExtractor : IFpFileNameExtractor
{
	private IFpFileParser fpFileParser;

	public FpFileNameExtractor(IFpFileParser fpFileParser)
	{
		this.fpFileParser = fpFileParser;
	}
	
	public FpFileName ExtractFileName(InboxFpMessage fpMessage)
	{
		var parsedFile = fpFileParser.ParsePayload(fpMessage.Payload);
		
		return new FpFileName()
		{
			MessageType = ToMessageType(fpMessage.BDEWProperties.BDEWDocumentType),
			EicNameBilanzkreis = "", // TODO Mapping,
			EicNameTso = parsedFile.Receiver.Code,
			Timestamp = parsedFile.ValidityDate,
			Date = parsedFile.CreationDate,
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			TypeHaendlerfahrplan = "" // TODO mapping
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
			case "A59": // Gefunden in: prozessbeschreibung_fahrplananmeldung_v4.5 in A.4.1.1 
				return FpMessageType.Status;
			default:
				throw new NotSupportedException($"Document type {bdewDocumentType} is not supported.");
		}
	}
}