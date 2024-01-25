// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System.Net;
	using BusinessApi;
	using NUnit.Framework;

	[TestFixture]
	internal sealed class MessageResponseTest
	{
		[Test]
		public void Ctor_ForUseCaseWithoutException_ShouldOnlySetPayloadAndSuccessFlag()
		{
			MessageResponse<bool> testObject = new MessageResponse<bool>(false, true);
			
			Assert.That(testObject.WasSuccessful, Is.False);
			Assert.That(testObject.Payload, Is.True);
			Assert.That(testObject.ApiException, Is.Null);
			Assert.That(testObject.ResponseStatusCode, Is.Null);
		}

		[Test]
		public void Ctor_ForUseCaseWithException_ShouldSetAllProperties()
		{
			MessageResponse<bool> testObject = new MessageResponse<bool>(true, false, HttpStatusCode.BadGateway, new ApiException("message", (int)HttpStatusCode.BadGateway, "response", new Dictionary<string, IEnumerable<string>>(), null));

			Assert.That(testObject.ApiException!.Message, Is.EqualTo("message\n\nStatus: 502\nResponse: \nresponse"));
			Assert.That(testObject.Payload, Is.False);
			Assert.That(testObject.ResponseStatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
			Assert.That(testObject.WasSuccessful, Is.True);
		}
	}
}
