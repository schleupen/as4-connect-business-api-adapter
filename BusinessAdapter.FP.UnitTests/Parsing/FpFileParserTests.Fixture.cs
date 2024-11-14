namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class FpFileParserTests
{
	private sealed class TestData
	{
		public string ExampleEssConfirmationReportPath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240126_TPS_FINGRID_0X1001A1001A264_002_CNF_01-26T08-2344Z.xml");

		public string ExampleEssAcknowledgeMessagePath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/20241018_PPS_FINGRID_10XDE-ENBW--HGJL_001_ACK_2024-10-16T10-46-50Z.xml");

		public string ExampleEssScheduleMessagePath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20241022_TPS_11X0-1111-0762-I_10XDE-AOE-HTCC-C_001.xml");

		public string EssScheduleMessagePathOfWrongFile =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240125_PPS_FINGRID_0X1001A1001A264_003.xml");

		public string ExampleEssAnomalyReportPath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240125_PPS_FINGRID_0X1001A1001A264_002_ANO_2024-01-26T08-2344Z.xml");

		public string ExampleEssStatusRequestPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/20241022_SRQ_11X0-1111-0762-I_10XDE-AOE-HTCC-C.xml");

		public string ExampleCimConfirmationReportPath => "";

		public string ExampleCimAcknowledgeMessagePath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/20241016_PPS_11Y0-0000-2483-X_10XDE-VE-FRISMK_086_ACK_2024-10-16T10-48-22Z.xml");

		public string ExampleCimScheduleMessagePath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20241008_TPS_10XEN-VE-FRISMK_11X0-1111-0619-M_002.xml");

		public string ExampleCimAnomalyReportPath => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/20241016_PPS_10YEN-XIN------1_10XEN-XIN-NETZ-C_148_ANO_2024-10-15T22-00Z.xml");

		public string EssConfirmationReportGzip => Path.Combine(TestContext.CurrentContext.TestDirectory,
			@"Parsing/2024-11-13T09_00_56.5778588Z_A07_1.edi.gz");

		public string ExampleCimStatusRequestPath => "";
	}

	private sealed class Fixture : IDisposable
	{
		public TestData TestData { get; private set; } = new();

		public void Dispose()
		{
		}
	}
}