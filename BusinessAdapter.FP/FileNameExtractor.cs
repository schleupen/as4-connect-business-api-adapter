namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFileNameExtractor : IFpFileNameExtractor
{
	public FpFileName ExtractFileName(InboxFpMessage mpMessage)
	{
		return new FpFileName()
		{
			MessageType = ToMessageType(mpMessage.BDEWProperties.BDEWDocumentType),
			EicNameBilanzkreis = "", // TODO Mapping,
			EicNameTso = "", // TODO Mapping
			Timestamp = mpMessage.BDEWProperties.BDEWFulfillmentDate, // TODO is this correct?
			Date = mpMessage.BDEWProperties.BDEWFulfillmentDate, // TODO is this correct?
			Version = "", // TODO parse?
			Type = "" // TODO parse?
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
			case "A??": // TODO: --> welcher Wert?
				return FpMessageType.Status; 
			default:
				throw new NotSupportedException($"Document type {bdewDocumentType} is not supported.");
		}
	}
}