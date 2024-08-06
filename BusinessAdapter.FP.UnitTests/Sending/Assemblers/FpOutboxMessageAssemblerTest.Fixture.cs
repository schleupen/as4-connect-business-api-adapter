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
		fixture = null!;
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
		public static SendingFpParty Sender = new SendingFpParty(PartyId1, PartyType, "FpType", "Bilanzkreis");
		public static ReceivingFpParty Receiver = new ReceivingFpParty(PartyId2, PartyType, "FpType", "Bilanzkreis");
		public static readonly FpBDEWProperties BDEWProperties = new FpBDEWProperties("1", "2", "3", "4", "5");

		public EICMapping Mapping = new EICMapping()
		{
			{ PartyId1, MappingParty1  },
			{ PartyId2 , MappingParty2 }
		};

		public const string EICCodeSender = "eic_1";
		public const string PartyType = "type";
		public const string PartyId1 = "party_id_1";
		public const string PartyId2 = "party_id_2";
		public const string EICCodeReceiver = "eic_2";
		
		public static List<EICMappingEntry> MappingParty1 = new List<EICMappingEntry>()
		{
			new EICMappingEntry()
			{
				Bilanzkreis = Sender.Bilanzkreis,
				EIC = EICCodeSender,
				FpType = Sender.FpType,
				MpType = Sender.Type
			}
		};
		
		public static List<EICMappingEntry> MappingParty2 = new List<EICMappingEntry>()
		{
			new EICMappingEntry()
			{
				Bilanzkreis = Receiver.Bilanzkreis,
				EIC = EICCodeReceiver,
				FpType = Receiver.FpType,
				MpType = Receiver.Type
			}
		};
	}
}