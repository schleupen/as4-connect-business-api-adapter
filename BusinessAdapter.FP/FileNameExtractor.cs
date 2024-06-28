namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FileNameExtractor : IFileNameExtractor
{
	public FpFileName ExtractFileName(InboxFpMessage mpMessage)
	{
		return new FpFileName()
		{
			MessageType = ToMessageType(mpMessage.BDEWDocumentType),
			EicNameBilanzkreis = "", // TODO Mapping,
			EicNameTso = "", // TODO Mapping
			Timestamp = mpMessage.BDEWFulfillmentDate, // TODO is this correct?
			Date = mpMessage.BDEWFulfillmentDate, // TODO is this correct?
			Version = "", // TODO parse?
			Type = "" // TODO parse?
		};
	}

	private FpMessageType ToMessageType(string bdewDocumentType)
	{
		// TODO
		return FpMessageType.Acknowledge;
	}
}