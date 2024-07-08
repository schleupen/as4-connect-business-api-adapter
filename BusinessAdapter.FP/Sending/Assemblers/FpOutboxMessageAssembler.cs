namespace Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

using Schleupen.AS4.BusinessAdapter.FP.Configuration;

// TODO Test
public sealed class FpOutboxMessageAssembler(EICMapping eicMapping) : IFpOutboxMessageAssembler
{
	public List<FpOutboxMessage> ToFpOutboxMessages(List<FpFile> filesToSend)
	{
		return filesToSend.Select(ToFpOutboxMessage).ToList();
	}

	public FpOutboxMessage ToFpOutboxMessage(FpFile file)
	{
		var sendingParty = eicMapping.GetSendingParty(file.Sender);
		var receivingParty = eicMapping.GetReceivingParty(file.Receiver);

		return new FpOutboxMessage(
			Guid.NewGuid(),
			sendingParty,
			receivingParty,
			file.Content,
			file.FileName,
			file.BDEWProperties);
	}
}