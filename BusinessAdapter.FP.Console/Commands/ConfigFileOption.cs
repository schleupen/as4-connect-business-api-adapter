namespace Schleupen.AS4.BusinessAdapter.FP.Commands;

using System.CommandLine;

public class ConfigFileOption() : Option<FileInfo>(new[] { "-c", "-configFile" }, () => new FileInfo("./appsettings.json"));