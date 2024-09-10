namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

using System.Security.Cryptography;

public class InboxFpMessage(
	string messageId,
	SendingParty sender,
	ReceivingParty receiver,
	string? content,
	byte[] payload,
	FpBDEWProperties bdewProperties)
	: IInboxMessage
{
    public SendingParty Sender { get; } = sender;

    public ReceivingParty Receiver { get; } = receiver;

    public string MessageId { get; } = messageId;

    public string? Content { get; } = content;

	/// <summary>
	/// XML payload.
	/// </summary>
#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
	public byte[] Payload { get; } = payload;
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben

	public string? ContentHashSha256
	{
		get
		{
			if (Payload == null || Payload.Length == 0)
			{
				return null;
			}

			// The hash has to be calculated on the zipped payload
			using var sha = SHA256.Create();
			byte[] ediHash = sha.ComputeHash(Payload);
			return Convert.ToBase64String(ediHash);
		}
	}

	public FpBDEWProperties BDEWProperties { get; } = bdewProperties;

    /// <summary>
    /// The file name of the XML file.
    /// </summary>
    public string FileName { get; }
}