// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System;
	using System.IO;
	using System.Text;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Parsing;
	using Schleupen.AS4.BusinessAdapter.Sending;

	public class EdifactFile : IEdifactFile
	{
		public static readonly Encoding DefaultEncoding = EdifactEncoding.GetEncoding();

		private IEdifactHeaderinformationParser? metadataParser;
		private byte[]? content;

		public EdifactFile(string path)
		{
			Path = path;
		}

		public string Path { get; }

		public string? SenderIdentificationNumber
		{
			get
			{
				ParseFile();
				return metadataParser!.GetAbsenderCodenummer();
			}
		}

		public string? ReceiverIdentificationNumber
		{
			get
			{
				ParseFile();
				return metadataParser!.GetEmpfaengerCodenummer();
			}
		}

		public string? ReceiverIdentificationNumberType
		{
			get
			{
				ParseFile();
				return Enum.Parse(typeof(CodeVergebendeStelle), metadataParser!.GetReceiverIdentificationNumberType().ToString()).ToString();
			}
		}

		public string? Dataformatname
		{
			get
			{
				ParseFile();
				return metadataParser!.GetDataformatname();
			}
		}

		public string? DocumentNumber
		{
			get
			{
				ParseFile();
				return metadataParser!.GetDocumentnumber();
			}
		}

		public DateTimeOffset DocumentDate
		{
			get
			{
				ParseFile();
				return metadataParser!.GetErstellungszeitpunkt();
			}
		}

		public byte[]? GetContent()
		{
			ParseFile();
			return content;
		}

		public OutboxMessage CreateOutboxMessage()
		{
			if (ReceiverIdentificationNumber == null)
			{
				throw new ArgumentException($"Could not retrieve receiver code number from file {Path}.");
			}

			if (ReceiverIdentificationNumberType == null)
			{
				throw new ArgumentException($"Could not retrieve receiver code number type from file {Path}.");
			}

			if (DocumentNumber == null)
			{
				throw new ArgumentException($"Could not retrieve document number from file {Path}.");
			}

			if (Dataformatname == null)
			{
				throw new ArgumentException($"Could not retrieve data format name from file {Path}.");
			}

			byte[]? payload = GetContent();
			if (payload == null)
			{
				throw new ArgumentException($"Could not retrieve the payload from file {Path}.");
			}

			return new OutboxMessage(
				new ReceivingParty(ReceiverIdentificationNumber, ReceiverIdentificationNumberType),
				string.Empty,
				DocumentNumber,
				Dataformatname,
				payload,
				System.IO.Path.GetFileName(Path),
				DocumentDate);
		}

		private void ParseFile()
		{
			if (metadataParser != null)
			{
				return;
			}

			metadataParser = new EdifactHeaderinformationParser();
			using (Stream stream = new FileStream(Path, FileMode.Open))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					content = memoryStream.ToArray();
					stream.Position = 0;
				}

				metadataParser.Parse(stream);
			}
		}
	}
}
