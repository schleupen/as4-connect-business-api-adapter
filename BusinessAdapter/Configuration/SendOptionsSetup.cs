namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Options;

public class SendOptionsSetup(IOptions<AdapterOptions> adapterOptions) : IConfigureOptions<SendOptions>
{
	public void Configure(SendOptions options)
	{
		options.RetryCount = adapterOptions.Value.DeliveryRetryCount;
		options.MessageLimitCount = adapterOptions.Value.DeliveryMessageLimitCount;
	}
}