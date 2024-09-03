namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public interface IFpFile
{
	string FilePath { get; }
	EIC Sender { get; }
	EIC Receiver { get; }
	FpBDEWProperties BDEWProperties { get; }
	byte[] Content { get; }
}