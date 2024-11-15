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
			MessageType = fpMessage.BDEWProperties.ToMessageType(),
			EicNameBilanzkreis = parsedFile.Receiver.Code,
			EicNameTso = parsedFile.Sender.Code,
			Timestamp = parsedFile.ValidityDate,
			Date = parsedFile.CreationDate,
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			FahrplanHaendlerTyp = parsedFile.FahrplanHaendlerTyp
		};
	}
}