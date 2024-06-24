namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Options;

public class ReceiveOptionsSetup(IOptions<AdapterOptions> adapterOptions) : IConfigureOptions<ReceiveOptions>
{
	public void Configure(ReceiveOptions options)
	{
		options.RetryCount = adapterOptions.Value.ReceivingRetryCount;
		options.MessageLimitCount = adapterOptions.Value.ReceivingMessageLimitCount;
	}
}