using Microsoft.AspNetCore.Mvc.Testing;
using sampleapp.Tests.UnitTests.Helpers;
using FluentAssertions;
using FluentAssertions.Execution;

namespace sampleapp.Tests.UnitTests
{
    [Trait("Category", "Unit")]
	[Collection("Sequential")]
	public class TestsampleappApiFactory : IClassFixture<WebApplicationFactory<Program>>
	{

		private readonly WebApplicationFactory<Program> _factory;

		public TestsampleappApiFactory(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		/// <summary>
		/// Validate UTC call works
		/// </summary>
		/// <returns></returns>
		[Theory]
		[InlineData("/")]
		[InlineData("/UTC")]
		public async Task UTCPath(string path)
		{
			using (var client = _factory.CreateClient())
			{
				var response = await client.GetAsync(path);
				response.EnsureSuccessStatusCode();
				string stamp = await response.Content.ReadAsStringAsync();
				using (new AssertionScope())
				{
					stamp.ToDateTime().Should().BeOnOrAfter(DateTime.UtcNow - TimeSpan.FromSeconds(2));
					stamp.ToDateTime().Should().BeOnOrBefore(DateTime.UtcNow + TimeSpan.FromSeconds(2));
				}
			}
		}
		/// <summary>
		/// Validate local offset call
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		[Theory]
		[InlineData(13)]
		[InlineData(-13)]
		public async Task Local(long offset)
		{
			using (var client = _factory.CreateClient())
			{
				var response = await client.GetAsync($"/local/{TimeSpan.FromHours(offset).Ticks}");
				response.EnsureSuccessStatusCode();
				string stamp = await response.Content.ReadAsStringAsync();
				var offsetTime = DateTime.UtcNow + TimeSpan.FromHours(offset);
				using (new AssertionScope())
				{
					stamp.ToDateTime().Should().BeOnOrAfter(offsetTime - TimeSpan.FromSeconds(2));
					stamp.ToDateTime().Should().BeOnOrBefore(offsetTime + TimeSpan.FromSeconds(2));
				}
			}
		}

		/// <summary>
		/// Validate local offset fail
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		[Fact]
		public async Task LocalFail()
		{
			using (var client = _factory.CreateClient())
			{
				var response = await client.GetAsync($"/local/foo");
				response.IsSuccessStatusCode.Should().BeFalse();
			}
		}

		/// <summary>
		/// Validate migrating local offset call
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		[Theory]
		[InlineData(13)]
		[InlineData(-13)]
		public async Task Migration(long offset)
		{
			long yesterday = (DateTime.UtcNow - TimeSpan.FromDays(1)).ToBinary();
			long tomorrow = (DateTime.UtcNow + TimeSpan.FromDays(1)).ToBinary();
			long ticks = TimeSpan.FromHours(offset).Ticks;
			string path = $"/migrating/{ticks}/{yesterday}/{tomorrow}";

			using (var client = _factory.CreateClient())
			{
				var response = await client.GetAsync(path);
				response.EnsureSuccessStatusCode();
				string stamp = await response.Content.ReadAsStringAsync();
				var now = DateTime.UtcNow;
				var offsetTime = now + TimeSpan.FromHours(offset);
				if (offset > 0)
				{
					using (new AssertionScope())
					{
						stamp.ToDateTime().Should().BeOnOrAfter(now, "Timestamp not in range");
						stamp.ToDateTime().Should().BeOnOrBefore(offsetTime, "Timestamp not in range");
					}
				}
				else
				{
					using (new AssertionScope())
					{
						stamp.ToDateTime().Should().BeOnOrAfter(offsetTime, "Timestamp not in range");
						stamp.ToDateTime().Should().BeOnOrBefore(now, "Timestamp not in range");
					}
				}
			}
		}
	}
}
