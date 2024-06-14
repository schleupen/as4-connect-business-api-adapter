// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;

	public sealed class EdifactParser : IDisposable
	{
		private readonly EdifactParserConfiguration configuration;
		private readonly Queue<char> buffer = new();
		private readonly Dictionary<string, ISegmentparser> segmentparser = new();
		private readonly TextReader reader;
		private bool firstSegmentRead;

		public EdifactParser(Stream edifactStream)
		{
			reader = new StreamReader(edifactStream, EdifactEncoding.GetEncoding(), false, 1024, true);
			configuration = new EdifactParserConfiguration();
		}

		public void Dispose()
		{
			reader.Dispose();
		}

		public void RegisterSegmentparser(IEnumerable<ISegmentparser> segmentparserToRegister)
		{
			foreach (ISegmentparser parser in segmentparserToRegister)
			{
				segmentparser.Add(parser.Segmentname, parser);
			}
		}

		public ParseSegmentResult? ReadNextSegment(int segmentNumber)
		{
			TryParseUna();
			string? edifactSegment = GetNextSegment();

			if (string.IsNullOrEmpty(edifactSegment))
			{
				return null;
			}

			string trimmedEdifactSegment = edifactSegment.TrimEnd(configuration.SegmentSeparator[^1]);
			trimmedEdifactSegment = trimmedEdifactSegment.Trim('\0');

			if (string.IsNullOrEmpty(trimmedEdifactSegment))
			{
				return null;
			}

			return ParseSegment(segmentNumber, trimmedEdifactSegment);
		}

		private ParseSegmentResult ParseSegment(int segmentNumber, string segment)
		{
			string identifier = segment.Substring(0, Math.Min(3, segment.Length));

			if (segmentparser.TryGetValue(identifier, out ISegmentparser? segmentParser))
			{
				Regex compositeRegex = configuration.GetCompositeRegex();
				string[] composites = compositeRegex.Split(segment.Substring(4));
				try
				{
					return segmentParser.Parse(segmentNumber, configuration, composites);
				}
				catch (InvalidOperationException ex)
				{

					string message = ex.Message + $" Segmentno. {segmentNumber}, Segment: '{segment}'.";
					throw new InvalidOperationException(message, ex);
				}
			}

            throw new InvalidOperationException($"No parser was found for the segment '{identifier}', segment number '{segmentNumber}'.", null);
		}

		private void TryParseUna()
		{
			if (firstSegmentRead)
			{
				return;
			}

			char[] unaReadBuffer = new char[9];
			reader.Read(unaReadBuffer, 0, unaReadBuffer.Length);
			string ediSegment = new StringBuilder().Append(unaReadBuffer).ToString();

			if (ediSegment.Substring(0, 3).Equals("UNA", StringComparison.OrdinalIgnoreCase))
			{
				configuration.DataElementSeparator = ediSegment.Substring(3, 1);
				configuration.CompositeSeparator = ediSegment.Substring(4, 1);
				configuration.DecimalSeparator = ediSegment.Substring(5, 1);
				configuration.ReleaseSeparator = ediSegment.Substring(6, 1);
				configuration.ReservedSeparator = ediSegment.Substring(7, 1);
				configuration.SegmentSeparator = ediSegment.Substring(8, 1);
			}
			else
			{
				foreach (char c in ediSegment)
				{
					buffer.Enqueue(c);
				}
			}

			firstSegmentRead = true;
		}

		private string? GetNextSegment()
		{
			StringBuilder segmentBuilder = new StringBuilder();
			char? lastReadChar = null;
			int anzReleaseChar = 0;
			while (PeekChar() >= 0)
			{
				char actualChar = ReadChar();

				if (actualChar == '\r' || actualChar == '\n')
				{
					// nothing to do
				}
				else if (lastReadChar == configuration.SegmentSeparator[^1] && char.IsWhiteSpace(actualChar))
				{
					// nothing to do
				}
				else if (segmentBuilder.Length == 0 && char.IsWhiteSpace(actualChar))
				{
					// nothing to do
				}
				else
				{
					segmentBuilder.Append(actualChar);
					if (anzReleaseChar % 2 == 0 && actualChar == configuration.SegmentSeparator[^1])
					{
						return segmentBuilder.ToString();
					}
				}

				lastReadChar = actualChar;

				if (lastReadChar == configuration.ReleaseSeparator[^1])
				{
					anzReleaseChar++;
				}
				else
				{
					anzReleaseChar = 0;
				}
			}

			return segmentBuilder.Length > 0 ? segmentBuilder.ToString() : null;
		}

		private char ReadChar()
		{
			if (buffer.Count > 0)
			{
				return buffer.Dequeue();
			}

			return (char)reader.Read();
		}

		private int PeekChar()
		{
			if (buffer.Count > 0)
			{
				return buffer.Peek();
			}

			return reader.Peek();
		}
	}
}
