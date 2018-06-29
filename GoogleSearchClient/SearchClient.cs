using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoogleSearchService
{
	/// <summary>
	/// A Search Client for sending the request to Google.
	/// Implements Disposable interface to get disposed together with other Transient Dependencies
	/// </summary>
	public class SearchClient : IGoogleSearchClient, IDisposable
	{
		public SearchClient(ILoggerFactory loggerFactory)
		{
			this.logger = loggerFactory.CreateLogger(nameof(SearchClient)) ?? throw new ArgumentNullException(nameof(loggerFactory));

			var handlerWithDecompression = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
			};

			this.client = new HttpClient(handlerWithDecompression)
			{
				BaseAddress = new Uri("http://www.google.com"),
				DefaultRequestHeaders =
				{
					Accept = { MediaTypeWithQualityHeaderValue.Parse("text/html"), MediaTypeWithQualityHeaderValue.Parse("application/xhtml+xml"), MediaTypeWithQualityHeaderValue.Parse("application/xml")},
					AcceptEncoding = { StringWithQualityHeaderValue.Parse("gzip"), StringWithQualityHeaderValue.Parse("deflate")},
					UserAgent = { ProductInfoHeaderValue.Parse("Chrome/10.0.648.151")}
				}
			};
		}

		readonly ILogger logger;
		readonly HttpClient client;

		/// <summary>
		/// Sends request to Google Search Page.
		/// </summary>
		/// <param name="searchString">search string to append to base URL</param>
		/// <returns>Raw HTML received from server as string, or null in case an error has occurred.</returns>
		public async Task<string> GetGoogleSearchResultsAsync(string searchString)
		{
			using (logger.BeginScope(nameof(GetGoogleSearchResultsAsync)))
			{
				try
				{
					using (var response = await client.GetAsync(searchString))
					using (var content = response.Content)
					{
						if (response.IsSuccessStatusCode)
						{
							logger.LogInformation("Successfully performed search");
							return await content.ReadAsStringAsync();
						}

						logger.LogError("Failed searching Google. Error Code {0}", response.StatusCode);
						return null;
					}
				}
				catch (HttpRequestException ex)
				{
					logger.LogError(ex, "Error sending request");
					throw;
				}
			}
		}

		public void Dispose()
		{
			client?.Dispose();
		}
	}
}
