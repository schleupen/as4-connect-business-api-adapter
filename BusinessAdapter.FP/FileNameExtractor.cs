namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFileNameExtractor(IFpFileParser fpFileParser)
	: IFpFileNameExtractor
{
	public FpFileName ExtractFileName(InboxFpMessage fpMessage)
	{
		var payloadInfo = fpFileParser.ParseCompressedPayload(fpMessage.Payload);

		return new FpFileName()
		{
			Date = fpMessage.BDEWProperties.BDEWFulfillmentDate.Replace("-", string.Empty).Trim(),
			FahrplanHaendlerTyp = payloadInfo.FahrplanHaendlerTyp,
			EicNameBilanzkreis = payloadInfo.Receiver.Code,
			EicNameTso = payloadInfo.Sender.Code,
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			MessageType = fpMessage.BDEWProperties.ToMessageType(),
			Timestamp = payloadInfo.MessageDateTime,
		};
	}
}