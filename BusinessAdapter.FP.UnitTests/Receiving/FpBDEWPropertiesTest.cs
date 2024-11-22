namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Receiving;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpBDEWPropertiesTest
{
	[TestCase(BDEWDocumentTypes.A07, FpMessageType.ConfirmationReport)]
	[TestCase(BDEWDocumentTypes.A08, FpMessageType.ConfirmationReport)]
	[TestCase(BDEWDocumentTypes.A09, FpMessageType.ConfirmationReport)]
	[TestCase(BDEWDocumentTypes.A01, FpMessageType.Schedule)]
	[TestCase(BDEWDocumentTypes.A17, FpMessageType.Acknowledge)]
	[TestCase(BDEWDocumentTypes.A16, FpMessageType.AnomalyReport)]
	[TestCase(BDEWDocumentTypes.A59, FpMessageType.StatusRequest)]
	public void ToMessageType_ShouldReturnCorrectMessageType(string documentType, FpMessageType expectedMessageType)
	{
		FpBDEWProperties properties = new FpBDEWProperties(
			documentType,
			"no",
			"date",
			"subject",
			"subjectRole");

		// Assert
		Assert.That(properties.ToMessageType(), Is.EqualTo(expectedMessageType));
	}
}