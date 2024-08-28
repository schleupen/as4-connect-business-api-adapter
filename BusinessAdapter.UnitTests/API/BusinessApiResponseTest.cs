// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed class BusinessApiResponseTest
	{
		[Test]
		public void Ctor_ForUseCaseWithoutException_ShouldOnlySetPayloadAndSuccessFlag()
		{
			BusinessApiResponse<bool> testObject = new BusinessApiResponse<bool>(false, true);

			Assert.That(testObject.WasSuccessful, Is.False);
			Assert.That(testObject.Message, Is.True);
			Assert.That(testObject.ApiException, Is.Null);
			Assert.That(testObject.ResponseStatusCode, Is.Null);
		}

		[Test]
		public void Ctor_ForUseCaseWithException_ShouldSetAllProperties()
		{
			BusinessApiResponse<bool> testObject = new BusinessApiResponse<bool>(true, false, HttpStatusCode.BadGateway, new ApiException("message", (int)HttpStatusCode.BadGateway, "response", new Dictionary<string, IEnumerable<string>>(), null));

			Assert.That(testObject.ApiException!.Message, Is.EqualTo("message\n\nStatus: 502\nResponse: \nresponse"));
			Assert.That(testObject.Message, Is.False);
			Assert.That(testObject.ResponseStatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
			Assert.That(testObject.WasSuccessful, Is.True);
		}
	}
}
