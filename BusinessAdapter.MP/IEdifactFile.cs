// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Schleupen.AS4.BusinessAdapter.MP.Sending;

public interface IEdifactFile
{
	string? SenderIdentificationNumber { get; }

	string Path { get; }

	OutboxMessage CreateOutboxMessage();
}