namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Receiving;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpBDEWPropertiesTest
{
	[TestCase("A07", FpMessageType.Confirmation)]
	[TestCase("A08", FpMessageType.Confirmation)]
	[TestCase("A09", FpMessageType.Confirmation)]
	[TestCase("A01", FpMessageType.Schedule)]
	[TestCase("A17", FpMessageType.Acknowledge)]
	[TestCase("A16", FpMessageType.Anomaly)]
	[TestCase("A59", FpMessageType.Status)]
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