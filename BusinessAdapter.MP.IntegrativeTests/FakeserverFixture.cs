namespace Schleupen.AS4.BusinessAdapter.MP;

public class FakeServerFixture
{
	private const string FakeServerHttpBaseAddress = "http://localhost:8081";
	private const string HealthEndpoint = FakeServerHttpBaseAddress + "/health";
	private const string FpResetEndpoint = FakeServerHttpBaseAddress + "/v1/fp/messages/reset";
	private const string MpResetEndpoint = FakeServerHttpBaseAddress + "/v1/mp/messages/reset";

	public async Task ShouldBeHealthyAsync()
	{
		using (var client = new HttpClient())
		{
			var response = await client.GetAsync(new Uri(HealthEndpoint));
			response.EnsureSuccessStatusCode();
		}
	}

	public async Task ResetFpMessagesAsync()
	{
		using (var client = new HttpClient())
		{
			var response = await client.PostAsync(new Uri(FpResetEndpoint), null);
			response.EnsureSuccessStatusCode();
		}
	}

	public async Task ResetMpMessagesAsync()
	{
		using (var client = new HttpClient())
		{
			var response = await client.PostAsync(new Uri(MpResetEndpoint), null);
			response.EnsureSuccessStatusCode();
		}
	}
}