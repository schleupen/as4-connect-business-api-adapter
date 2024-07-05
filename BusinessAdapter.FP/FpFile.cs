namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Sending;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFile(
	EIC sender,
	EIC receiver,
	byte[] content,
	string filename,
	string path,
	FpBDEWProperties fpBDEWProperties) : IFpFile
{
	public string Path { get; } = path;

	public EIC Sender { get; } = sender;

	public EIC Receiver { get; } = receiver;

	public FpBDEWProperties BDEWProperties { get; } = fpBDEWProperties;

	public byte[] Content { get; } = content;

	public FpOutboxMessage CreateOutboxMessage(EICMapping mapping)
	{
		var sendingParty = mapping.GetSendingParty(sender);
		var receivingParty = mapping.GetReceivingParty(receiver);

		return new FpOutboxMessage(
			sendingParty,
			receivingParty,
			content,
			filename,
			fpBDEWProperties);
	}
}