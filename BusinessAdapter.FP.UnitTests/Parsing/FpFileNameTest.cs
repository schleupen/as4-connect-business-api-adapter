﻿namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

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
		Assert.That(parsed.Timestamp.Value.ToFileTimestamp(), Is.EqualTo("2024-01-26T08-22-52Z"));
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
		Assert.That(parsed.Timestamp.Value.ToFileTimestamp(), Is.EqualTo("2024-01-26T08-22-52Z"));
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.AnomalyReport));
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
		Assert.That(parsed.Timestamp.Value.ToFileTimestamp(), Is.EqualTo("2024-01-26T08-22-52Z"));
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.ConfirmationReport));
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
		Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.StatusRequest));
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
	[TestCase("20240101:12")]
	public void ToFileName_InvalidDate_ShouldThrowFormatException(string date)
	{
		var fileName = new FpFileName()
		{
			MessageType = FpMessageType.ConfirmationReport,
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
	[TestCase("20240126_TPS_EIC1_EIC2_001_CNF_2024-01-26T08-22-52Z.xml")]
	[TestCase("20240126_TPS_EIC1_EIC2_001_ACK_2024-01-26T08-22-52Z.xml")]
	[TestCase("20240126_TPS_EIC1_EIC2_001.xml")]
	public void ToFileName_Valid_ShouldWork(string validFileName)
	{
		var fileName = FpFileName.FromFileName(validFileName);
		var fileNameString = fileName.ToFileName();
		Assert.That(fileNameString, Is.EqualTo(validFileName));
	}
}