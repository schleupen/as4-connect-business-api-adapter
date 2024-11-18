namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;
using System;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

[TestFixture]
public class FpFileNameTest
{
	[Test]
	public void FromFileName_AcknowledgementMessage_ShouldReturnParsedFilename()
	{
		string filename = "20240126_TPS_EIC1_EIC2_001_ACK_2024-01-26T08-22-52Z.XML";

		var parsed = FpFileName.FromFileName(filename);

		Assert.That(parsed.Date, Is.EqualTo("20240126"));
		Assert.That(parsed.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
		Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
		Assert.That(parsed.Version, Is.EqualTo("1"));
		Assert.That(parsed.Timestamp, Is.EqualTo("2024-01-26T08-22-52Z"));
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Acknowledge));
	}

	[Test]
	public void FromFileName_AnomalyReport_ShouldReturnParsedFilename()
	{
		string filename = "20240126_TPS_EIC1_EIC2_001_ANO_2024-01-26T08-22-52Z.XML";

		var parsed = FpFileName.FromFileName(filename);

		Assert.That(parsed.Date, Is.EqualTo("20240126"));
		Assert.That(parsed.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
		Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
		Assert.That(parsed.Version, Is.EqualTo("1"));
		Assert.That(parsed.Timestamp, Is.EqualTo("2024-01-26T08-22-52Z"));
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Anomaly));
	}

	[Test]
	public void FromFileName_ValidConfirmationReport_ShouldReturnParsedFilename()
	{
		string filename = "20240126_TPS_EIC1_EIC2_001_CNF_2024-01-26T08-22-52Z.XML";

		var parsed = FpFileName.FromFileName(filename);

		Assert.That(parsed.Date, Is.EqualTo("20240126"));
		Assert.That(parsed.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
		Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
		Assert.That(parsed.Version, Is.EqualTo("1"));
		Assert.That(parsed.Timestamp, Is.EqualTo("2024-01-26T08-22-52Z"));
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Confirmation));
	}

	[Test]
	public void FromFileName_StatusRequest_ShouldReturnParsedFilename()
	{
		string filename = "20240126_TPS_EIC1_EIC2.XML";

		var parsed = FpFileName.FromFileName(filename);

		Assert.That(parsed.Date, Is.EqualTo("20240126"));
		Assert.That(parsed.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
		Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
		Assert.That(parsed.Version, Is.EqualTo("1"));
		Assert.That(parsed.Timestamp, Is.Null);
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Status));
	}

	[Test]
	public void FromFileName_ScheduleMessage_ShouldReturnParsedFilename()
	{
		string filename = "20240126_TPS_EIC1_EIC2_001.XML";

		var parsed = FpFileName.FromFileName(filename);

		Assert.That(parsed.Date, Is.EqualTo("20240126"));
		Assert.That(parsed.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
		Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
		Assert.That(parsed.Version, Is.EqualTo("1"));
		Assert.That(parsed.Timestamp, Is.Null);
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Schedule));
	}

	[Test]
	[TestCase("InvalidFilename.XML")]
	[TestCase(null)]
	[TestCase("")]
	[TestCase("20240126_TPS_EIC1_EIC2_001.json")]
	public void FromFileName_InvalidFilename_ShouldThrowFormatException(string invalidFileName)
	{
		Assert.That(() => FpFileName.FromFileName(invalidFileName), Throws.TypeOf<FormatException>());
	}
}