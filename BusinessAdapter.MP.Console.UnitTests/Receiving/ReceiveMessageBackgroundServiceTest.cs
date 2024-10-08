﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Receiving;

using NUnit.Framework;

[TestFixture]
internal sealed partial class ReceiveMessageBackgroundServiceTest : IDisposable
{
	private ReceiveMessageBackgroundServiceTest.Fixture? fixture;

	[SetUp]
	public void Setup()
	{
		fixture = new ReceiveMessageBackgroundServiceTest.Fixture();
	}

	[TearDown]
	public void Dispose()
	{
		fixture?.Dispose();
		fixture = null;
	}

	[Test]
	public async Task StartAsync_ShouldCallController()
	{
		fixture!.PrepareStart();
		using ReceiveMessageBackgroundService testObject = fixture!.CreateTestObject();
		using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(520));

		Task task =  testObject.StartAsync(cancellationTokenSource.Token);

		await Task.WhenAny(task, Task.Delay(TimeSpan.FromMinutes(1)));
		fixture.VerifyControllerWasCalled();
	}

	[Test]
	public async Task StartAsync_WithErrorInController_ShouldLogError()
	{
		fixture!.PrepareStartWithError();
		using ReceiveMessageBackgroundService testObject = fixture!.CreateTestObject();
		using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(10));

		await testObject.StartAsync(cancellationTokenSource.Token);

		// Verify is performed during dispose
	}

	[Test]
	public async Task StartAsync_WithCatastrophicErrorInController_ShouldLogErrorAndCrash()
	{
		fixture!.PrepareStartWithCatastrophicError();
		using ReceiveMessageBackgroundService testObject = fixture!.CreateTestObject();
		using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(10));

		// ReSharper disable once AccessToDisposedClosure
		try
		{
			await testObject.StartAsync(cancellationTokenSource.Token);
		}
		catch (CatastrophicException e)
		{
			Assert.That(e.Message, Is.EqualTo("Expected Catastrophic Exception during test."));
			Assert.Pass();
		}

		Assert.Fail("Expected a CatastrophicException to be thrown which did not occur.");
	}
}