namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.ComponentModel.DataAnnotations;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpParsedFileValidator : IFpParsedFileValidator
{
	public void ValidateParsedFpFile(FpFile fpFile)
	{
		var fileName = FpFileName.FromFileName(fpFile.FileName);

		ValidateFpMessageType(fpFile.BDEWProperties, fileName.MessageType);
		ValidateFpMessageVersion(fpFile.BDEWProperties.BDEWDocumentNo, fileName);
		ValidateFpMessageSender(fpFile.Sender.Code, fileName);
	}

	private void ValidateFpMessageSender(string fpFilesSender, FpFileName fileName)
	{
		if (fileName.MessageType is FpMessageType.Schedule or FpMessageType.StatusRequest)
		{
			if (fileName.EicNameBilanzkreis != fpFilesSender)
			{
				throw new ValidationException($"Parsed SenderID {fpFilesSender} does not match filename SenderID {fileName.EicNameBilanzkreis}");
			}
		}
	}

	private void ValidateFpMessageType(FpBDEWProperties bdewProperties, FpMessageType fpMessageType)
	{
		try
		{
			var parsedMessageType = bdewProperties.ToMessageType();
			if (parsedMessageType != fpMessageType)
			{
				throw new ValidationException($"Parsed DocumentType '{bdewProperties.BDEWDocumentType}' does not match filename DocumentType '{fpMessageType}'");
			}
		}
		catch (NotSupportedException e)
		{
			throw new ValidationException($"Parsed DocumentType '{bdewProperties.BDEWDocumentType}' is unsupported", e);
		}
	}

	private void ValidateFpMessageVersion(string bdewDocumentNo, FpFileName fileName)
	{
		if (bdewDocumentNo != fileName.Version)
		{
			throw new ValidationException(
				$"Parsed Document Version '{bdewDocumentNo}' does not match filename Document Version '{fileName.Version}'");
		}
	}
}