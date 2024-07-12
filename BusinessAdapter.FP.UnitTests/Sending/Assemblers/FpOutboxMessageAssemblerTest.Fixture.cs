namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending.Assemblers;

using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

public partial class FpOutboxMessageAssemblerTest
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
		fixture = null;
	}

	private sealed class Fixture
	{
		public Mocks Mocks { get; } = new();

		public TestData Data { get; } = new();

		public Fixture()
		{
		}

		public FpOutboxMessageAssembler CreateTestObjectWithMapping()
		{
			this.Mocks.EICMapping.Setup(x => x.Value).Returns(this.Data.Mapping);
			return new FpOutboxMessageAssembler(Mocks.EICMapping.Object);
		}
	}

	private sealed class Mocks
	{
		public Mock<IOptions<EICMapping>> EICMapping { get; } = new();
	}

	private sealed class TestData
	{
		public static SendingParty Sender = new SendingParty(PartyId1, PartyType);
		public static ReceivingParty Receiver = new ReceivingParty(PartyId2, PartyType);
		public static readonly FpBDEWProperties BDEWProperties = new FpBDEWProperties("1", "2", "3", "4", "5");

		public EICMapping Mapping = new EICMapping()
		{
			{ EICCodeSender, Sender },
			{ EICCodeReceiver, Receiver }
		};

		public const string EICCodeSender = "eic_1";
		public const string PartyType = "type";
		public const string PartyId1 = "party_id_1";
		public const string PartyId2 = "party_id_2";
		public const string EICCodeReceiver = "eic_2";
	}
}