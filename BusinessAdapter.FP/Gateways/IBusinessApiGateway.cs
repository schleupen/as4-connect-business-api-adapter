// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using System.Threading.Tasks;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.FP.Sending;
	using Schleupen.AS4.BusinessAdapter.FP.Receiving;

	public interface IBusinessApiGateway : IDisposable
	{
		Task<BusinessApiResponse<FpOutboxMessage>> SendMessageAsync(FpOutboxMessage message, CancellationToken cancellationToken);
		
		/// <summary>
		/// Queries the available messages ready to be received.
		/// </summary>
		/// <param name="limit">default: 50, min: 1, max: 1000</param>
		/// <returns>The Metadata of the available messages.</returns>
		Task<MessageReceiveInfo> QueryAvailableMessagesAsync(int limit = 50);
		
		/// <summary>
		/// Queries XML files for the given AS4 message..
		/// </summary>
		/// <param name="mpMessage">The Metadata of the message.</param>
		/// <returns>The XML file.</returns>
		Task<BusinessApiResponse<InboxFpMessage>> ReceiveMessageAsync(As4FpMessage mpMessage);

		/// <summary>
		/// Confirms that a message was successfully received.
		/// </summary>
		/// <param name="fpMessage">The received message that should be acknowledged.</param>
		/// <returns>Whether the acknowledgement was successful.</returns>
		Task<BusinessApiResponse<bool>> AcknowledgeReceivedMessageAsync(InboxFpMessage mpMessage);
	}
}