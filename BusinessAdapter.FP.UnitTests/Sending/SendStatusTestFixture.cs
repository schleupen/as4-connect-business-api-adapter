namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending;

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Sending;

public partial class SendStatusTest
{
	private Fixture fixture = default!;

	[SetUp]
	public void Setup()
	{
		fixture = new Fixture();
	}

	[TearDown]
	public void Dispose()
	{
		fixture = null!;
	}

	private sealed class Fixture
	{
		public TestData Data { get; } = new();

		public Mocks Mocks { get; } = new();

		public SendStatus CreateSendStatusObject()
		{
			return new SendStatus(DirectoryResult.Empty("idk"));
		}
	}

	private sealed class Mocks
	{
		public Mock<ILogger> LoggerMock = new();
	}

	private sealed class TestData
	{
		public static SendingParty Sender = new (PartyId1, PartyType);
		public static ReceivingParty Receiver = new (PartyId2, PartyType);
		public const string PartyType = "type";
		public const string PartyId1 = "party_id_1";
		public const string PartyId2 = "party_id_2";
		public static readonly FpBDEWProperties BDEWProperties = new("1", "2", "3", "4", "5");

		public TestData()
		{
			this.SuccessfulOutboundMessage = CreateFpOutboundMessage();
			this.FailedOutboundMessage = CreateFpOutboundMessage();

			this.DirectoryResult = new DirectoryResult(
				"./sendDirectory",
				new List<FpFile>()
				{
					new FpFile(new EIC("sender"),
						new EIC("receiver"),
						Array.Empty<byte>(),
						SuccessfulOutboundMessage.FileName,
						SuccessfulOutboundMessage.FilePath,
						TestData.BDEWProperties),
					new FpFile(new EIC("sender"),
						new EIC("receiver"),
						Array.Empty<byte>(),
						FailedOutboundMessage.FileName,
						FailedOutboundMessage.FilePath,
						TestData.BDEWProperties)
				}.ToList(),
				[]);
		}

		public FpOutboxMessage SuccessfulOutboundMessage { get; }

		public FpOutboxMessage FailedOutboundMessage { get; }

		public DirectoryResult DirectoryResult { get;  }

		public FpOutboxMessage CreateFpOutboundMessage()
		{
			var id = Guid.NewGuid();
			return new FpOutboxMessage(Guid.NewGuid(),
				Sender,
				Receiver,
				Array.Empty<byte>(),
				$"{id}_name",
				$"{id}_path",
				BDEWProperties);
		}
	}
}