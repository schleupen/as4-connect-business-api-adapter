namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

// TODO Test
public class FpFileNameExtractor : IFpFileNameExtractor
{
	public FpFileName ExtractFileName(InboxFpMessage fpMessage)
	{
		return new FpFileName()
		{
			MessageType = ToMessageType(fpMessage.BDEWProperties.BDEWDocumentType),
			EicNameBilanzkreis = "", // TODO Mapping,
			EicNameTso = "", // TODO Mapping
			Timestamp = fpMessage.BDEWProperties.BDEWFulfillmentDate, // TODO get this from file
			Date = fpMessage.BDEWProperties.BDEWFulfillmentDate, // TODO get this from file
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			TypeHaendlerfahrplan = "" // TODO parse?
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