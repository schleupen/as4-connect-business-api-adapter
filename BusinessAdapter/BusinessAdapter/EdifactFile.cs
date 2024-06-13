// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using System;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Sending;

public class EdifactFile : IEdifactFile
{
	private readonly string filename;
	private readonly string receiverIdentificationNumber;
	private readonly string receiverIdentificationNumberType;
	private readonly string dataformatname;
	private readonly string documentNumber;
	private readonly DateTimeOffset documentDate;
	private readonly byte[] content;

	public EdifactFile(string path, string filename, string receiverIdentificationNumber, string senderIdentificationNumber, string receiverIdentificationNumberType, string dataformatname, string documentNumber, DateTimeOffset documentDate, byte[] content)
	{
		Path = path;
		this.filename = filename;
		this.receiverIdentificationNumber = receiverIdentificationNumber;
		SenderIdentificationNumber = senderIdentificationNumber;
		this.receiverIdentificationNumberType = receiverIdentificationNumberType;
		this.dataformatname = dataformatname;
		this.documentNumber = documentNumber;
		this.documentDate = documentDate;
		this.content = content;
	}

	public string SenderIdentificationNumber { get; }

	public string Path { get; }

	public OutboxMessage CreateOutboxMessage()
	{
		return new OutboxMessage(
			new ReceivingParty(receiverIdentificationNumber, receiverIdentificationNumberType),
			string.Empty,
			documentNumber,
			dataformatname,
			content,
			filename,
			documentDate);
	}
}