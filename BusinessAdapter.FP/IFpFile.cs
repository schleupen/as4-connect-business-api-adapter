namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public interface IFpFile
{
	string FilePath { get; }

	EIC Sender { get; }

	EIC Receiver { get; }

	FpBDEWProperties BDEWProperties { get; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Eigenschaften dürfen keine Arrays zurückgeben", Justification = "Used as intended.")]
	byte[] Content { get; }
}