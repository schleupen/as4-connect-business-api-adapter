namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using System.Xml.Linq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

[TestFixture]
internal sealed partial class EssFileParserTest
{
	private Fixture fixture = new();

	[SetUp]
	public void Setup()
	{
		this.fixture = new Fixture();
	}

	private FpFile? Parse(string path)
	{
		FileInfo fi = new FileInfo(path);
		return fixture.CreateTestObject().Parse(
			XDocument.Load(path),
			fi.Name,
			path);
	}

	private sealed class Fixture
	{
		public TestData TestData { get; } = new();

		public EssFileParser CreateTestObject()
		{
			return new EssFileParser();
		}
	}

	private sealed class TestData
	{
		public string ExampleEssConfirmationReportPath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/EssFiles\20240126_TPS_FINGRID_0X1001A1001A264_002_CNF_01-26T08-2344Z.xml");

		public string ExampleEssAcknowledgeMessagePath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/EssFiles/20241018_PPS_FINGRID_10XDE-ENBW--HGJL_001_ACK_2024-10-16T10-46-50Z.xml");

		public string ExampleEssScheduleMessagePath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/EssFiles/20241022_TPS_11X0-1111-0762-I_10XDE-AOE-HTCC-C_001.xml");

		public string EssScheduleMessagePathOfWrongFile =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/InvalidFiles/20240125_PPS_FINGRID_0X1001A1001A264_003.xml");

		public string ExampleEssAnomalyReportPath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/EssFiles/20240125_PPS_FINGRID_0X1001A1001A264_002_ANO_2024-01-26T08-2344Z.xml");

		public string ExampleEssStatusRequestPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/EssFiles/20241022_SRQ_11X0-1111-0762-I_10XDE-AOE-HTCC-C.xml");

		public string EssConfirmationReportPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/EssFiles/2024-11-13T09_00_56.5778588Z_A07_1.xml");
	}
}