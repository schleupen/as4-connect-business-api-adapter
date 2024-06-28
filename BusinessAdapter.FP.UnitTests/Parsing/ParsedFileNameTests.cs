namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;
using System;
using Schleupen.AS4.BusinessAdapter.FP;

[TestFixture]
public class ParsedFileNameTests
{
    [Test]
    public void Parse_ValidAckFilename_ShouldReturnParsedFilename()
    {
        string filename = "20240126_TPS_EIC1_EIC2_001_ACK_2024-01-26T08-22-52Z.XML";

        var parsed = ParsedFileName.Parse(filename);

        Assert.That(parsed.Date, Is.EqualTo("20240126"));
        Assert.That(parsed.Type, Is.EqualTo("TPS"));
        Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
        Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
        Assert.That(parsed.Version, Is.EqualTo("001"));
        Assert.That(parsed.Timestamp, Is.EqualTo("2024-01-26T08-22-52Z"));
        Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Acknowledge));
    }

    [Test]
    public void Parse_ValidAnoFilename_ShouldReturnParsedFilename()
    {
        string filename = "20240126_TPS_EIC1_EIC2_001_ANO_2024-01-26T08-22-52Z.XML";

        var parsed = ParsedFileName.Parse(filename);

        Assert.That(parsed.Date, Is.EqualTo("20240126"));
        Assert.That(parsed.Type, Is.EqualTo("TPS"));
        Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
        Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
        Assert.That(parsed.Version, Is.EqualTo("001"));
        Assert.That(parsed.Timestamp, Is.EqualTo("2024-01-26T08-22-52Z"));
        Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Anomaly));
    }

    [Test]
    public void Parse_ValidCnfFilename_ShouldReturnParsedFilename()
    {
        string filename = "20240126_TPS_EIC1_EIC2_001_CNF_2024-01-26T08-22-52Z.XML";

        var parsed = ParsedFileName.Parse(filename);

        Assert.That(parsed.Date, Is.EqualTo("20240126"));
        Assert.That(parsed.Type, Is.EqualTo("TPS"));
        Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
        Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
        Assert.That(parsed.Version, Is.EqualTo("001"));
        Assert.That(parsed.Timestamp, Is.EqualTo("2024-01-26T08-22-52Z"));
        Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Confirmation));
    }

    [Test]
    public void Parse_ValidStatusFilename_ShouldReturnParsedFilename()
    {
        string filename = "20240126_TPS_EIC1_EIC2_CRQ.XML";

        var parsed = ParsedFileName.Parse(filename);

        Assert.That(parsed.Date, Is.EqualTo("20240126"));
        Assert.That(parsed.Type, Is.EqualTo("TPS"));
        Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
        Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
        Assert.That(parsed.Version, Is.Null);
        Assert.That(parsed.Timestamp, Is.Null);
        Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Status));
    }

    [Test]
    public void Parse_ValidScheduleFilename_ShouldReturnParsedFilename()
    {
        string filename = "20240126_TPS_EIC1_EIC2_001.XML";

        var parsed = ParsedFileName.Parse(filename);

        Assert.That(parsed.Date, Is.EqualTo("20240126"));
        Assert.That(parsed.Type, Is.EqualTo("TPS"));
        Assert.That(parsed.EicNameBilanzkreis, Is.EqualTo("EIC1"));
        Assert.That(parsed.EicNameTso, Is.EqualTo("EIC2"));
        Assert.That(parsed.Version, Is.EqualTo("001"));
        Assert.That(parsed.Timestamp, Is.Null);
        Assert.That(parsed.MessageType, Is.EqualTo(FpMessageType.Schedule));
    }

    [Test]
    public void Parse_InvalidFilename_ShouldThrowFormatException()
    {
        string filename = "InvalidFilename.XML";

        Assert.That(() => ParsedFileName.Parse(filename), Throws.TypeOf<FormatException>());
    }
}