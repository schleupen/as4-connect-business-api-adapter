namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFile(
	EIC sender,
	EIC receiver,
	byte[] content,
	string fileName,
	string filePath,
	FpBDEWProperties fpBDEWProperties) : IFpFile
{
	public string FilePath { get; } = filePath;

	public EIC Sender { get; } = sender;

	public EIC Receiver { get; } = receiver;

	public FpBDEWProperties BDEWProperties { get; } = fpBDEWProperties;

	public byte[] Content { get; } = content;

	public string FileName { get; } = fileName;
}