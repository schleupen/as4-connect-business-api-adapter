namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

public class FpParsedFileValidator : IFpParsedFileValidator
{
	public void ValidateParsedFpFile(FpFile fpFile)
	{
		var fileName = FpFileName.Parse(fpFile.FileName);

		ValidateFpMessageType(fpFile.BDEWProperties.BDEWDocumentType, fileName.MessageType);
		ValidateFpMessageVersion(fpFile.BDEWProperties.BDEWDocumentNo, fileName);
		ValidateFpMessageSender(fpFile.Sender.Code, fileName);
	}

	private void ValidateFpMessageSender(string fpFilesSender, FpFileName fileName)
	{
		if (fileName.EicNameTso != fpFilesSender)
		{
			throw new ValidationException(
				$"Parsed SenderID {fpFilesSender} does not match filename SenderID {fileName.EicNameTso}");
		}
	}

	private void ValidateFpMessageType(string bdewDocumentType, FpMessageType fpMessageType)
	{
		var validDocumentTypes = new Dictionary<FpMessageType, string[]>
		{
			{ FpMessageType.Acknowledge, new[] { "A17" } },
			{ FpMessageType.Schedule, new[] { "A01" } },
			{ FpMessageType.Confirmation, new[] { "A07", "A08", "A09" } },
			{ FpMessageType.Anomaly, new[] { "A16" } },
			{ FpMessageType.Status, new[] { "A59" } }
		};

		if (!validDocumentTypes.TryGetValue(fpMessageType, out var allowedDocumentTypes)
		    || !allowedDocumentTypes.Contains(bdewDocumentType))
		{
			throw new ValidationException($"Parsed DocumentType {bdewDocumentType} does not match filename DocumentType {fpMessageType}");
		}
	}

	private void ValidateFpMessageVersion(string bdewDocumentNo, FpFileName fileName)
	{
		if (bdewDocumentNo != fileName.Version)
		{
			throw new ValidationException($"Parsed Document Version {bdewDocumentNo} does not match filename Document Version {fileName.Version}");
		}
	}
}
