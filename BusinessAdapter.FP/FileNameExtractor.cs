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
			MessageType = fpMessage.BDEWProperties.ToMessageType(),
			EicNameBilanzkreis = senderParty.Bilanzkreis,
			EicNameTso = parsedFile.Sender.Code,
			Timestamp = parsedFile.ValidityDate,
			Date = parsedFile.CreationDate,
			Version = fpMessage.BDEWProperties.BDEWDocumentNo,
			FahrplanHaendlerTyp = senderParty.FpType
		};
	}
}