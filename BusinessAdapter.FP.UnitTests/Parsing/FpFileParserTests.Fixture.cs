namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class FpFileParserTests
{
    private sealed class Fixture
    {
        public string CreateExampleESSConfirmationReport()
        {
            string dir = TestContext.CurrentContext.TestDirectory;
            return Path.Combine(dir, @"Parsing/20240126_TPS_0X1001A1001A264_FINGRID_002_CNF_01-26T08-2344Z.xml");
        }
        
        public string CreateExampleESSAcknowledgeMessage()
        {
            return "";
        }
        
        public string CreateExampleESSScheduleMessage()
        {
            string dir = TestContext.CurrentContext.TestDirectory;
            return Path.Combine(dir, @"Parsing/20240125_PPS_0X1001A1001A264_FINGRID_002.xml");
        }
        
        public string CreateExampleESSAnomalyReport()
        {
            string dir = TestContext.CurrentContext.TestDirectory;
            return Path.Combine(dir, @"Parsing/20240125_PPS_0X1001A1001A264_FINGRID_002_ANO_2024-01-26T08-2344Z.xml");
        }
        
        public string CreateExampleESSStatusRequest()
        {
            return "";
        }
        
        //
        public string CreateExampleCIMConfirmationReport()
        {
            return "";
        }
        
        public string CreateExampleCIMAcknowledgeMessage()
        {
            return "";
        }
        
        public string CreateExampleCIMScheduleMessage()
        {
            return "";
        }
        
        public string CreateExampleCIMAnomalyReport()
        {
            return "";
        }
        
        public string CreateExampleCIMStatusRequest()
        {
            return "";
        }
    }
}