using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace sampleapp.Tests.IntegrationTests.Client
{
	public class sampleappClient
	{
		private readonly HttpClient HttpClient;

		public sampleappClient()
		{
			var url = Environment.GetEnvironmentVariable("API_BASE_URL"); // get the url from the environment variable,
																		  // or grab from cloud platform
			if (string.IsNullOrEmpty(url))
			{
				// Defaulting to Integreation(QA) environment base URL if nothing is retrieved
				// example url = $"https://api-serviceName-qa.netdocuments.com";
				url = "https://analyticsreporting.googleapis.com";
			}
			HttpClient = new HttpClient { BaseAddress = new Uri(url) };
		}

		public string GetUrlForDebugging() {
			return HttpClient.BaseAddress.ToString();
		}

		// All the API endponts are created here:
		// - gives access to each api call
		// - users can build cleanup into all their calls
		// This is an example to show the creation of creating calls to all the endpoints
		// allowing us to call these endpoints as building blocks for the testing work
		#region Analytics Reporting
		public async Task<HttpResponseMessage> GoogleAnalyticsReportingExample()
		{
			var urlPath = $"/v4/reports:batchGet";

			return await HttpClient.PostAsync(urlPath, null);
		}


		// Example API endponts are created here:
		// - gives access to each api call
		// - shows example to delete the data on cleanup at the end of the tests
		public async Task<HttpResponseMessage> CreateDataExample(string thingsToCreate)
		{
			var urlPath = $"/createEndpoint";
			var httpContent = new StringContent(
				JsonConvert.SerializeObject(
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto,
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				})
				, Encoding.UTF8, "application/json");

			return await HttpClient.PostAsync(urlPath, httpContent);
		}

		public async Task<HttpResponseMessage> DeleteDataExample(string info)
		{
			var urlPath = $"/deleteEndpoint";

			return await HttpClient.DeleteAsync(urlPath);
		}
		#endregion
	}
}
