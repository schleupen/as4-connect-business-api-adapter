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

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Eigenschaften dürfen keine Arrays zurückgeben", Justification = "Used as intended.")]
	public byte[] Content { get; } = content;

	public string FileName { get; } = fileName;
}