// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System.Threading.Tasks;

	/// <summary>
	/// Controller for receiving AS4 message from AS4 Connect.
	/// </summary>
	public interface IReceiveMessageAdapterController
	{
		/// <summary>
		/// Performs the receiving process.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		Task ReceiveAvailableMessagesAsync(CancellationToken cancellationToken);
	}
}
