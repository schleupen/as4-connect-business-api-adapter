﻿namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

[TestFixture]
public class FpParsedFileValidatorTests
{
	private FpParsedFileValidator validator;

	[SetUp]
	public void Setup()
	{
		validator = new FpParsedFileValidator();
	}

	[Test]
	public void ValidateParsedFpFile_ValidFile_DoesNotThrowException()
	{
		// Arrange
		var fpFile = CreateValidFpFile();

		// Act & Assert
		Assert.That(() => validator.ValidateParsedFpFile(fpFile), Throws.Nothing);
	}

	[Test]
	public void ValidateParsedFpFile_InvalidMessageType_ThrowsValidationException()
	{
		// Arrange
		var fpFile = CreateValidFpFile("002", "0X1001A1001A264", BDEWDocumentTypes.A16);

		// Act & Assert
		Assert.That(() => validator.ValidateParsedFpFile(fpFile),
			Throws.TypeOf<ValidationException>()
				.With.Message.EqualTo("Parsed DocumentType 'A16' does not match filename DocumentType 'ConfirmationReport'"));
	}

	[Test]
	public void ValidateParsedFpFile_InvalidSender_ThrowsValidationException()
	{
		// Arrange
		var fpFile = CreateValidFpFile("1", "InvalidSender", BDEWDocumentTypes.A59);

		// Act & Assert
		Assert.That(() => validator.ValidateParsedFpFile(fpFile),
			Throws.TypeOf<ValidationException>()
				.With.Message.EqualTo("Parsed SenderID InvalidSender does not match filename SenderID FINGRID"));
	}

	[Test]
	public void ValidateParsedFpFile_InvalidDocumentVersion_ThrowsValidationException()
	{
		// Arrange
		var fpFile = CreateValidFpFile("InvalidDocNo");

		// Act & Assert
		Assert.That(() => validator.ValidateParsedFpFile(fpFile),
			Throws.TypeOf<ValidationException>()
				.With.Message.EqualTo("Parsed Document Version 'InvalidDocNo' does not match filename Document Version '2'"));
	}

	private FpFile CreateValidFpFile(string bdewDocumentNo = "2", string senderCode = "0X1001A1001A264", string bdewDocumentType = BDEWDocumentTypes.A07)
	{
		if (bdewDocumentType == BDEWDocumentTypes.A59)
		{
			return new FpFile(
				new EIC(senderCode),
				new EIC("TSO002"),
				null,
				Path.Combine(TestContext.CurrentContext.TestDirectory,
					@"Parsing/20240126_TPS_FINGRID_0X1001A1001A264.xml"),
				"filePath",
				new FpBDEWProperties(bdewDocumentType, bdewDocumentNo, "Fulfilmentdate", "0X1001A1001A264", "A01"));
		}

		return new FpFile(
			new EIC(senderCode),
			new EIC("TSO002"),
			null,
			Path.Combine(TestContext.CurrentContext.TestDirectory,
				@"Parsing/20240126_TPS_FINGRID_0X1001A1001A264_002_CNF_2024-01-26T08-23-44Z.xml"),
			"filePath",
			new FpBDEWProperties(bdewDocumentType, bdewDocumentNo, "2024-01-26", "0X1001A1001A264", "A01"));
	}
}