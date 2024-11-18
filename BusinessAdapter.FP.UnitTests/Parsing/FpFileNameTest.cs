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
		Assert.That(parsed.Timestamp.Value.ToString(FpFileName.TimestampFormat), Is.EqualTo("2024-01-26T08-22-52Z"));
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
		Assert.That(parsed.Timestamp.Value.ToString(FpFileName.TimestampFormat), Is.EqualTo("2024-01-26T08-22-52Z"));
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
		Assert.That(parsed.Timestamp.Value.ToString(FpFileName.TimestampFormat), Is.EqualTo("2024-01-26T08-22-52Z"));
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
	[TestCase("20240126_TPS_EIC1_EIC2_001_CNF_2024-01-26T08-22-52.XML")]
	public void FromFileName_InvalidFilename_ShouldThrowFormatException(string invalidFileName)
	{
		Assert.That(() => FpFileName.FromFileName(invalidFileName), Throws.TypeOf<FormatException>());
	}

	[Test]
	[TestCase(null)]
	[TestCase("")]
	[TestCase("2024-01-26")]
	[TestCase("20249926")]
	public void ToFileName_InvalidDate_ShouldThrowFormatException(string date)
	{
		var fileName = new FpFileName()
		{
			MessageType = FpMessageType.Confirmation,
			EicNameTso = "1",
			Date = date,
			Version = "1",
			Timestamp = DateTime.Now,
			EicNameBilanzkreis = "BLK",
			FahrplanHaendlerTyp = "TPS"
		};

		Assert.That(() => fileName.ToFileName(), Throws.TypeOf<FormatException>());
	}

	[Test]
	[TestCase("20240126_TPS_EIC1_EIC2_001_CNF_2024-01-26T08-22-52Z.XML")]
	public void ToFileName_Valid_ShouldWork(string invalidFileName)
	{
		var fileName = FpFileName.FromFileName(invalidFileName);
		Assert.DoesNotThrow(() => fileName.ToFileName());
	}
}