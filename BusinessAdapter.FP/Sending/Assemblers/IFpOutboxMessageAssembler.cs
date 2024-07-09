namespace Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

public interface IFpOutboxMessageAssembler
{
	List<FpOutboxMessage> ToFpOutboxMessages(List<FpFile> filesToSend);
	FpOutboxMessage ToFpOutboxMessage(FpFile file);
}