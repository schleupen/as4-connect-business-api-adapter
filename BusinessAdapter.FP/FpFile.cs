namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFile(
	EIC sender,
	EIC receiver,
	byte[] content,
	string fileName,
	string path,
	FpBDEWProperties fpBDEWProperties) : IFpFile
{
	public string Path { get; } = path;

	public EIC Sender { get; } = sender;

	public EIC Receiver { get; } = receiver;

	public FpBDEWProperties BDEWProperties { get; } = fpBDEWProperties;

	public byte[] Content { get; } = content;

	public string FileName { get; } = fileName;
}