namespace Schleupen.AS4.BusinessAdapter.FP.Configuration;

using Microsoft.Extensions.Configuration;

public class ConfigurationFromFileProvider
{
	public IConfigurationRoot FromFile(FileInfo configFile)
	{
		return new ConfigurationBuilder()
			.AddJsonFile(configFile.FullName, optional: false, reloadOnChange: true)
			.Build();
	}
}