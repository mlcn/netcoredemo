using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Moq;
using NetCoreDemo.Pages;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class TestRazorPage : BaseTest
	{
		[TestCaseSource(nameof(KeywordsSource))]
		public async Task TestRazorPage_CorrectlyProcessesKeywords(string keywords, string[] splitKeywords, string[] results)
		{
			var mockService = new Mock<IGoogleSearchService>();
			var page = new IndexModel(mockService.Object);
			page.Keywords = keywords;
			page.URL = "http://www.google.com";

			var request = new GoogleSearchRequest
			{
				Keywords = splitKeywords,
				NumberOfResults = 0,
				UrlToFind = "http://www.google.com"
			};

			mockService
				.Setup(x => x.GetSearchResults(It.Is<GoogleSearchRequest>(r => new GoogleSearchRequestComparer().Equals(r, request))))
				.ReturnsAsync(results);
			await page.OnPostAsync();

			Assert.AreEqual(results, page.SearchResults);
			mockService.VerifyAll();
		}

		static IEnumerable<TestCaseData> KeywordsSource
		{
			get
			{
				yield return new TestCaseData("this, is, a, test", new[] { "this", "is", "a", "test" }, new[] { "1", "2", "3" });
				yield return new TestCaseData("this,is,a,test", new[] { "this", "is", "a", "test" }, new[] { "1", "2", "3" });
				yield return new TestCaseData("this is a test", new[] { "this", "is", "a", "test" }, new[] { "1", "2", "3" });
				yield return new TestCaseData("this , is , a,  test", new[] { "this", "is", "a", "test" }, new[] { "1", "2", "3" });
				yield return new TestCaseData(" this , is , a ,  test ", new[] { "this", "is", "a", "test" }, new[] { "1", "2", "3" });
			}
		}
	}

	class GoogleSearchRequestComparer : IEqualityComparer<GoogleSearchRequest>
	{
		public bool Equals(GoogleSearchRequest x, GoogleSearchRequest y)
		{
			return x == null && y == null || x != null && y != null && x.UrlToFind == y.UrlToFind && x.Keywords.SequenceEqual(y.Keywords) && x.NumberOfResults == y.NumberOfResults;
		}

		public int GetHashCode(GoogleSearchRequest obj)
		{
			return obj.Keywords.GetHashCode() * obj.UrlToFind.GetHashCode();
		}
	}
}
