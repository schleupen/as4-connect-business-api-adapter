// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	public static class Codenummer
	{
		private const string EdifactUnbCodeBdew = "500";
		private const string EdifactUnbCodeDvgw = "502";
		private const string EdifactUnbCodeGs1Germany = "14";
		
		public static CodeVergebendeStelle? CreateCodeVergebendeStelleAusEdifactUnbCode(string? ediCode)
		{
			switch (ediCode)
			{
				case EdifactUnbCodeBdew:
					return CodeVergebendeStelle.Bdew;
				case EdifactUnbCodeDvgw:
					return CodeVergebendeStelle.Dvgw;
				case EdifactUnbCodeGs1Germany:
					return CodeVergebendeStelle.Gs1Germany;
				default:
					return null;
			}
		}
	}
}