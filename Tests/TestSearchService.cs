using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using GoogleSearchService;
using Moq;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class TestSearchService : BaseTest
	{
		[TestCase]
		public void TestGetSearchResults_GivenInvalidRequest()
		{
			var mockClient = new Mock<IGoogleSearchClient>();
			var service = new SearchService(mockClient.Object, LoggerFactory);
			Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetSearchResults(null));
		}

		[TestCase]
		public void TestGetSearchResults_GivenHttpRequestExceptionOccurs()
		{
			var mockClient = new Mock<IGoogleSearchClient>();
			mockClient.Setup(x => x.GetGoogleSearchResultsAsync(It.IsAny<string>()))
				.ThrowsAsync(new HttpRequestException("Error"));
			var service = new SearchService(mockClient.Object, LoggerFactory);
			Assert.ThrowsAsync<HttpRequestException>(
				async () => await service.GetSearchResults(Fixture.Create<GoogleSearchRequest>()));
			Assert.False(LoggerFactory.Logger.Logs.Any(x => x.Contains("An error has occurred in GoogleSearchClient")));
		}

		[TestCase]
		public void TestGetSearchResults_GivenOtherExceptionOccurs()
		{
			var mockClient = new Mock<IGoogleSearchClient>();
			mockClient.Setup(x => x.GetGoogleSearchResultsAsync(It.IsAny<string>()))
				.ThrowsAsync(new Exception("Error"));
			var service = new SearchService(mockClient.Object, LoggerFactory);
			Assert.ThrowsAsync<Exception>(
				async () => await service.GetSearchResults(Fixture.Create<GoogleSearchRequest>()));
			Assert.True(LoggerFactory.Logger.Logs.Any(x => x.Contains("An error has occurred in GoogleSearchClient")));
		}

		[TestCase]
		public async Task TestGetSearchResults_CorrectSearchStringPassed()
		{
			var keywords = new[] { "this", "is", "a", "test" };
			var request = new GoogleSearchRequest { Keywords = keywords, UrlToFind = "http://www.test.com" };
			var mockClient = new Mock<IGoogleSearchClient>();
			mockClient
				.Setup(x => x.GetGoogleSearchResultsAsync("/search?q=this+is+a+test&num=100"))
				.ReturnsAsync("Success");

			var service = new SearchService(mockClient.Object, LoggerFactory);
			var result = await service.GetSearchResults(request);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("0", result[0]);
			mockClient.VerifyAll();
		}

		[TestCase]
		public async Task TestGetSearchResults_FindsRequiredOccurrences()
		{
			string file;
			var assembly = typeof(TestSearchService).GetTypeInfo().Assembly;

			using (var stream = assembly.GetManifestResourceStream("Tests.TestFiles.NetflixResults.txt"))
			{
				using (var reader = new StreamReader(stream))
				{
					file = reader.ReadToEnd();
				}
			}

			var keywords = new[] { "this", "is", "a", "test" };
			var request = new GoogleSearchRequest { Keywords = keywords, UrlToFind = "https://www.netflix.com" };
			var mockClient = new Mock<IGoogleSearchClient>();
			mockClient
				.Setup(x => x.GetGoogleSearchResultsAsync("/search?q=this+is+a+test&num=100"))
				.ReturnsAsync(file);

			var service = new SearchService(mockClient.Object, LoggerFactory);
			var result = await service.GetSearchResults(request);
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("11", result[0]);
			Assert.AreEqual("12", result[1]);
			mockClient.VerifyAll();
		}

		[TestCase]
		public async Task TestGetSearchResults_DoesntFindRequiredOccurrences_ReturnsZero()
		{
			string file;
			var assembly = typeof(TestSearchService).GetTypeInfo().Assembly;

			using (var stream = assembly.GetManifestResourceStream("Tests.TestFiles.NetflixResults.txt"))
			{
				using (var reader = new StreamReader(stream))
				{
					file = reader.ReadToEnd();
				}
			}

			var keywords = new[] { "this", "is", "a", "test" };
			var request = new GoogleSearchRequest { Keywords = keywords, UrlToFind = "https://www.unexisting.com" };
			var mockClient = new Mock<IGoogleSearchClient>();
			mockClient
				.Setup(x => x.GetGoogleSearchResultsAsync("/search?q=this+is+a+test&num=100"))
				.ReturnsAsync(file);

			var service = new SearchService(mockClient.Object, LoggerFactory);
			var result = await service.GetSearchResults(request);
			Assert.AreEqual(1, result.Length);
			Assert.AreEqual("0", result[0]);
			mockClient.VerifyAll();
		}
	}
}
