// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing
{
	using System;
	using System.Globalization;
	using System.Text;

	public static class EdifactEncoding
	{
		public static Encoding GetEncoding()
		{
			return GetEncoding("UNOC");
		}

		private static Encoding GetEncoding(string syntaxKennung)
		{
			ArgumentNullException.ThrowIfNull(syntaxKennung);

			switch (syntaxKennung.ToUpper(CultureInfo.InvariantCulture))
			{
				case "UNOA":
					return Encoding.GetEncoding("us-ascii"); // eigentlich ISO-640
				case "UNOB":
					return Encoding.GetEncoding("us-ascii"); // eigentlich ISO-640 inkl. Kleinbuchstaben
				case "UNOC":
					return Encoding.GetEncoding("iso-8859-1");
				case "UNOD":
					return Encoding.GetEncoding("iso-8859-2");
				case "UNOE":
					return Encoding.GetEncoding("iso-8859-5");
				case "UNOF":
					return Encoding.GetEncoding("iso-8859-7");
			}

            throw new InvalidOperationException($"The syntax identifier {syntaxKennung} is not supported.");
		}
	}
}
