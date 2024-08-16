namespace Schleupen.AS4.BusinessAdapter;

using NUnit.Framework;

public partial class InboundMessagesTests
{
    [Test]
    public async Task Test()
    {
        await this.fixture.StartAsync();
        var m = 123;
    }
}