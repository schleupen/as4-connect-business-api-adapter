namespace Schleupen.AS4.BusinessAdapter.MP.Commands;

using System.CommandLine;

public class ConfigFileOption : Option<FileInfo>
{
	public ConfigFileOption() : base("-configFile", "-c")
	{
		DefaultValueFactory = _ => new FileInfo("./appsettings.json");
	}
}
