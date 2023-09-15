using sampleapp.Tests.IntegrationTests.Client;

namespace sampleapp.Tests.IntegrationTests.Support
{
	public class TestFixture : IAsyncLifetime
	{
		public SetEnvironment TestInstance;
		public sampleappClient sampleappClient;

		public TestFixture() 
		{
			
		}

		public async Task InitializeAsync()
		{
			SetEnvironment.Init();
			TestInstance = SetEnvironment.Instance;
			sampleappClient = new sampleappClient();
		}

		public async Task DisposeAsync()
		{
			// Run all the delete tasks currently in the queue
			var result = await TestInstance.Delegator.RunTasksAsync();
			if (result != null)
			{
				var errorList = result.ToList();
				if (!errorList.Any()) return;
				Console.WriteLine("Test Cleanup Failures:");
				foreach (var error in errorList) Console.WriteLine(error);
			}
		}
	}
}
