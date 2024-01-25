// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using Schleupen.AS4.BusinessAdapter.Sending;

	public interface IEdifactFile
	{
		string Path { get; }

		string? SenderIdentificationNumber { get; }

		OutboxMessage CreateOutboxMessage();
	}
}
