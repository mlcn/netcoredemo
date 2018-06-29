using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreDemo.Controllers
{
	[Produces("application/json")]
	[Route("api")]
	public class APIController : Controller
	{
		public APIController(IGoogleSearchService searchService, ILoggerFactory loggerFactory)
		{
			this.searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
			logger = loggerFactory?.CreateLogger(nameof(APIController)) ?? throw new ArgumentNullException(nameof(logger));
		}

		readonly IGoogleSearchService searchService;
		readonly ILogger logger;

		[HttpPost]
		[Route("Search")]
		public async Task<IActionResult> SearchGoogle([FromBody] GoogleSearchRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Invalid request");
			}

			if (!Uri.IsWellFormedUriString(request.UrlToFind, UriKind.Absolute))
			{
				return BadRequest("Invalid URL to find");
			}

			try
			{
				var result = await searchService.GetSearchResults(request);
				return Ok(result);
			}
			catch (HttpRequestException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error has occurred");
				return StatusCode(500);
			}
		}
	}
}