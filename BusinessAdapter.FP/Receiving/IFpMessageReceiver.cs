using System.Threading.Tasks;

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving
{

	public interface IFpMessageReceiver
	{
		/// <summary>
		/// Performs the receiving process.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		Task<ReceiveStatus> ReceiveAvailableMessagesAsync(CancellationToken cancellationToken);
	}
}
