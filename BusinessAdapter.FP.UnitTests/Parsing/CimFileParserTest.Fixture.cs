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

	private FpPayloadInfo ParsePayload(string path)
	{
		return fixture.CreateTestObject().ParsePayload(XDocument.Load(path));
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
		private readonly string cimFilesDir = Path.Combine("Parsing", "CimFiles");

		private string CimFilePath(string fileName)
		{
			return Path.Combine(cimFilesDir, fileName);
		}

		public string AcknowledgeMessagePath => CimFilePath("20241016_PPS_11Y0-0000-2483-X_10XDE-VE-FRISMK_086_ACK_2024-10-16T10-48-22Z.xml");

		public string ScheduleMessagePath => CimFilePath("20241008_TPS_10XEN-VE-FRISMK_11X0-1111-0619-M_002.xml");

		public string AnomalyReportPath => CimFilePath("20241016_PPS_10YEN-XIN------1_10XEN-XIN-NETZ-C_148_ANO_2024-10-15T22-00-11Z.xml");

		public string ConfirmationReportPath => CimFilePath("20241104_TPS_11X0-0000-0706-U_10XDE-EON-NETZ-C_007_CNF_2024-11-03T14-18-21Z.xml");

		public string StatusRequestPath => CimFilePath("20241104_SRQ_11X0-0000-0619-M_10XDE-VE-TRANSMK.xml");
	}
}