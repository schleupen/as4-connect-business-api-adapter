namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFileNameExtractor(IFpFileParser fpFileParser)
	: IFpFileNameExtractor
{
	public FpFileName ExtractFileName(InboxFpMessage fpMessage)
	{
		var parsedFile = fpFileParser.ParseCompressedPayload(fpMessage.Payload);

		return new FpFileName()
		{
			Date = fpMessage.BDEWProperties.BDEWFulfillmentDate.Replace("-", string.Empty).Trim(),
			FahrplanHaendlerTyp = parsedFile.FahrplanHaendlerTyp,
			EicNameBilanzkreis = parsedFile.Receiver.Code,
			EicNameTso = parsedFile.Sender.Code,
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			MessageType = fpMessage.BDEWProperties.ToMessageType(),
			Timestamp = parsedFile.MessageDateTime,
		};
	}
}