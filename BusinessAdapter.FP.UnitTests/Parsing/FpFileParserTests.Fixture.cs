namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class FpFileParserTests
{
	private sealed class TestData
	{
		public string ExampleEssConfirmationReportPath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240126_TPS_0X1001A1001A264_FINGRID_002_CNF_01-26T08-2344Z.xml");

		public string ExampleEssAcknowledgeMessagePath => "";

		public string ExampleEssScheduleMessagePath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240125_PPS_0X1001A1001A264_FINGRID_002.xml");

		public string EssScheduleMessagePathOfWrongFile =>
		    Path.Combine(TestContext.CurrentContext.TestDirectory,
			    @"Parsing/20240125_PPS_0X1001A1001A264_FINGRID_003.xml");

		public string ExampleEssAnomalyReportPath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240125_PPS_0X1001A1001A264_FINGRID_002_ANO_2024-01-26T08-2344Z.xml");

		public string ExampleEssStatusRequestPath => "";

		public string ExampleCimConfirmationReportPath => "";

		public string ExampleCimAcknowledgeMessagePath => "";

		public string ExampleCimScheduleMessagePath =>
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20241008_TPS_11X0-0000-0619-M_10XDE-VE-TRANSMK_002.xml");

		public string ExampleCimAnomalyReportPath => "";

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