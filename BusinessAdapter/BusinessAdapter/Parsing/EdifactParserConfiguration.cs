// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	using System;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Parser Configuration.
	/// </summary>
	public class EdifactParserConfiguration
	{
		private string dataElementSeparator;
		private string compositeSeparator;
		private string decimalSeparator;
		private string releaseSeparator;
		private string reservedSeparator;
		private string segmentSeparator;
		private string releaseSeparatorRaw;
		private Regex? compositeRegex;
		private Regex? dataElementRegex;
		private Regex? replaceRegex;

		/// <summary>
		/// Separator for data elements (default :)
		/// </summary>
		public string DataElementSeparator
		{
			get
			{
				return dataElementSeparator;
			}
			set
			{
				dataElementSeparator = EscapeSeparator(value);
			}
		}

		/// <summary>
		/// Separator for composites. (default +)
		/// </summary>
		public string CompositeSeparator
		{
			get
			{
				return compositeSeparator;
			}
			set
			{
				compositeSeparator = EscapeSeparator(value);
			}
		}

		/// <summary>
		/// Separator for decimals. (default .)
		/// </summary>
		public string DecimalSeparator
		{
			get
			{
				return decimalSeparator;
			}
			set
			{
				decimalSeparator = EscapeSeparator(value);
			}
		}

		/// <summary>
		/// Escape symbol for EDIFACT. (default ?)
		/// </summary>
		public string ReleaseSeparator
		{
			get
			{
				return releaseSeparator;
			}
			set
			{
				releaseSeparatorRaw = value;
				releaseSeparator = EscapeSeparator(value);
			}
		}

		/// <summary>
		/// Reserved symbol in separators. (default blank)
		/// </summary>
		public string ReservedSeparator
		{
			get
			{
				return reservedSeparator;
			}
			set
			{
				reservedSeparator = EscapeSeparator(value);
			}
		}

		/// <summary>
		/// Separator for segments. (default ')
		/// </summary>
		public string SegmentSeparator
		{
			get
			{
				return segmentSeparator;
			}
			set
			{
				segmentSeparator = EscapeSeparator(value);
			}
		}
		
		/// <summary>
		/// Konstruktor. Sets the separator default values.
		/// </summary>
		public EdifactParserConfiguration()
		{
			dataElementSeparator = EscapeSeparator(":");
			compositeSeparator = EscapeSeparator("+");
			decimalSeparator = EscapeSeparator(".");
			releaseSeparatorRaw = "?";
			releaseSeparator = EscapeSeparator("?");
			reservedSeparator = EscapeSeparator(" ");
			segmentSeparator = EscapeSeparator("'");
		}

		/// <summary>
		/// Returns the regular expression for finding composites in a string.
		/// </summary>
		/// <returns>The regular expression.</returns>
		public Regex GetCompositeRegex()
		{
			return compositeRegex ??= BuildRegex(CompositeSeparator);
		}

		/// <summary>
		/// Returns the regular expression for finding data elements in a string.
		/// </summary>
		/// <returns>The regular expression.</returns>
		public Regex GetDataElementRegex()
		{
			return dataElementRegex ??= BuildRegex(dataElementSeparator);
		}

		/// <summary>
		/// Returns the EDIFACT escape symbol for the provided separator type.
		/// </summary>
		/// <param name="type">The separator type.</param>
		/// <returns>The separator symbol.</returns>
		public string GetReleaseSeparatorFor(SeparatorType type)
		{
			switch (type)
			{
				case SeparatorType.RegexEscaped:
					return releaseSeparator;
				case SeparatorType.Escaped:
					return releaseSeparatorRaw + releaseSeparatorRaw;
				case SeparatorType.Raw:
					return releaseSeparatorRaw;
				default:
					throw new InvalidOperationException($"Unknown separator type symbol '{type}'.");
			}
		}
		
		/// <summary>
		/// Replaces the EDIFACT escape symbolds.
		/// Examples:
		///		?? is turned into ?
		///		?: is turned into :
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="replacement">The replacement to use.</param>
		/// <returns>The input string without escape symbols.</returns>
		public string RegexReplace(string input, string replacement)
		{
			if (replaceRegex == null)
			{
				replaceRegex = new Regex(BuildReplaceString());
			}

			return replaceRegex.Replace(input, replacement);
		}

		private string EscapeSeparator(string separator)
		{
			return Regex.Escape(separator);
		}

		private Regex BuildRegex(string separator)
		{
			string regexString = @"(?<!" + ReleaseSeparator + ")" + separator;
			return new Regex(regexString);
		}

		private string BuildReplaceString()
		{
			return ReleaseSeparator + "(?=[" + SegmentSeparator + CompositeSeparator + DataElementSeparator + "])";
		}

		/// <summary>
		/// Separator Types.
		/// </summary>
		public enum SeparatorType
		{
			/// <summary>
			/// The separator symbol without EDIFACT escape symbol.
			/// </summary>
			Raw,

			/// <summary>
			/// The separator symbol with EDIFACT escape symbol.
			/// </summary>
			Escaped,

			/// <summary>
			/// The separator symbol with regular expression escaping.
			/// </summary>
			RegexEscaped
		}
	}
}