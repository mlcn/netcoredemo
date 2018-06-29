using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoogleSearchService
{
	/// <summary>
	/// A service which uses IGoogleSearchClient to send a request and parses the output.
	/// </summary>
	public class SearchService : IGoogleSearchService
	{
		public SearchService(IGoogleSearchClient client, ILoggerFactory loggerFactory)
		{
			this.client = client ?? throw new ArgumentNullException(nameof(client));
			this.logger = loggerFactory.CreateLogger(nameof(SearchService)) ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		readonly IGoogleSearchClient client;
		readonly ILogger logger;

		public async Task<string[]> GetSearchResults(GoogleSearchRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var numberOfResultsToReturn = request.NumberOfResults == 0 ? 100 : request.NumberOfResults;
			var searchString = $"/search?q={string.Join('+', request.Keywords)}&num={numberOfResultsToReturn}";
			using (logger.BeginScope(nameof(GetSearchResults)))
			{
				try
				{
					var rawSearchResultsHtml = await client.GetGoogleSearchResultsAsync(searchString);
					if (TryProcessSearchResults(rawSearchResultsHtml, request.UrlToFind, out string[] results))
					{
						logger.LogInformation("Parsed search results with the following output: {0}", string.Join(", ", results));
					}
					return results;
				}
				catch (Exception ex)
				{
					if (!(ex is HttpRequestException))
					{
						logger.LogError(ex, "An error has occurred in GoogleSearchClient");
					}
					throw;
				}
			}
		}

		bool TryProcessSearchResults(string rawSearchResultsHtml, string urlToFind, out string[] results)
		{
			results = new[] {"0"};
			if (!string.IsNullOrWhiteSpace(rawSearchResultsHtml))
			{
				var pattern = "<h3 class=\"r\">(.*?)<\\/h3>";
				var matches = Regex.Matches(rawSearchResultsHtml, pattern);
				if (matches.Any())
				{
					var output = new List<string>();
					for (var i = 0; i < matches.Count; i++)
					{
						if (matches[i].Value.Contains(urlToFind))
						{
							output.Add(i.ToString());
						}
					}

					if (output.Any())
					{
						results = output.ToArray();
					}
					return true;
				}
			}

			return false;
		}
	}
}
