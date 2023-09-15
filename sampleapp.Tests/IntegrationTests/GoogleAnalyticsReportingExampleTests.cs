using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sampleapp.Tests.IntegrationTests.Support;
using System.Net;
using Xunit.Abstractions;

namespace sampleapp.Tests.IntegrationTests
{
	[Collection("IntegrationTests")]
	[Trait("Category", "Integration")]
	public class GoogleAnalyticsReportingExampleTests : IAsyncLifetime
	{
		
		private readonly TestFixture _fixture;
		private readonly ITestOutputHelper _output;

		public GoogleAnalyticsReportingExampleTests(TestFixture fixture, ITestOutputHelper output) 
		{
			_fixture = fixture;
			_output = output;

			_output.WriteLine($"BaseUrl: {_fixture.sampleappClient.GetUrlForDebugging()}");
		}

		public async Task InitializeAsync() { }

		public async Task DisposeAsync()
		{
			// This is not being used
			// Instead, we are waiting for the TestFixture to run all queued tasks
		}

		// This is an example call to show the following benefits:
		// - uses the serviceClient that has access to all api endpoint calls you created
		// - naming conventions for testing "endpointname,action_intentofthetest_expectedresults" shows in test explorer
		// - verify the status code and contents of the body if needed
		// - shows the trait for testcases in ADO so they can be traced to each other
		[Fact]
		[Trait("TestCase", "123456")]
		public async Task AnalyticsReportPost_WhenCallingApiWithNoCreditials_ThenVerifyStatusCode401()
		{
			var postResponse = await _fixture.sampleappClient.GoogleAnalyticsReportingExample();
			postResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized, 
							"Post test failed to return 401 status for authorization, discontinue test");

			var stringContent = await postResponse.Content.ReadAsStringAsync();
			var jsonContent = (JObject)JsonConvert.DeserializeObject(stringContent);
			jsonContent["error"]["message"].ToString().Should().Contain("Request is missing required authentication credential.");
		}

		// this is an example of how to clean up your test data as you go
		// all data should be randomized to help keep the tests clean and working
		[Fact(Skip = "This is an example to show delegator to delete any data that is created")]
		public async Task ExampleCreate_WhenCreatingData_ThenDeleteDataWithDelagator()
		{
			var createResponse = await _fixture.sampleappClient.CreateDataExample("pass needed info to create");
			createResponse.StatusCode.Should().Be(HttpStatusCode.OK, "failed, discontinue...");

			// assert needed data calls

			// this is will call all tasks at the end of the tests to cleanup the test data
			_fixture.TestInstance.Delegator.AddTask(async () => await _fixture.sampleappClient.DeleteDataExample(
																					"pass needed info to delete"));
		}
	}
}
