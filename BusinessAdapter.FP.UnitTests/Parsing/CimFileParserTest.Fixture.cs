namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using System.Xml.Linq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

[TestFixture]
internal sealed partial class CimFileParserTest
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

		public CimFileParser CreateTestObject()
		{
			return new CimFileParser();
		}
	}

	private sealed class TestData
	{
		public string ExampleCimAcknowledgeMessagePath => Path.Combine(TestContext.CurrentContext.TestDirectory, @"Parsing/CimFiles/20241016_PPS_11Y0-0000-2483-X_10XDE-VE-FRISMK_086_ACK_2024-10-16T10-48-22Z.xml");

		public string ExampleCimScheduleMessagePath => Path.Combine(TestContext.CurrentContext.TestDirectory, @"Parsing/CimFiles/20241008_TPS_10XEN-VE-FRISMK_11X0-1111-0619-M_002.xml");

		public string ExampleCimAnomalyReportPath => Path.Combine(TestContext.CurrentContext.TestDirectory, @"Parsing/CimFiles/20241016_PPS_10YEN-XIN------1_10XEN-XIN-NETZ-C_148_ANO_2024-10-15T22-00-11Z.xml");
	}
}