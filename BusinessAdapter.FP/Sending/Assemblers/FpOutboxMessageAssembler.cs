namespace Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public sealed class FpOutboxMessageAssembler(IOptions<EICMapping> eicMapping) : IFpOutboxMessageAssembler
{
	public List<FpOutboxMessage> ToFpOutboxMessages(IEnumerable<FpFile> filesToSend)
	{
		return filesToSend.Select(ToFpOutboxMessage).ToList();
	}

	public FpOutboxMessage ToFpOutboxMessage(FpFile file)
	{
		var sendingParty = eicMapping.Value.GetSendingParty(file.Sender);
		var receivingParty = eicMapping.Value.GetReceivingParty(file.Receiver);

		return new FpOutboxMessage(
			Guid.NewGuid(),
			sendingParty,
			receivingParty,
			file.Content,
			file.FileName,
			file.FilePath,
			file.BDEWProperties);
	}
}