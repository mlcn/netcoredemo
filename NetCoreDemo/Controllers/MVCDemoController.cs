using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreDemo.Controllers
{
	[Route("[controller]")]
	public class MVCDemoController : Controller
	{
		public MVCDemoController(IGoogleSearchService searchService, ILoggerFactory loggerFactory)
		{
			this.searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
			logger = loggerFactory?.CreateLogger(nameof(MVCDemoController)) ?? throw new ArgumentNullException(nameof(logger));
		}

		readonly IGoogleSearchService searchService;
		readonly ILogger logger;

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("Search")]
		public async Task<IActionResult> SearchGoogle([FromForm] GoogleSearchRequest request)
		{
			if (!ModelState.IsValid)
			{
				ViewData["SearchResults"] = "Invalid request";
				return View("Index");
			}

			if (!Uri.IsWellFormedUriString(request.UrlToFind, UriKind.Absolute))
			{
				ViewData["SearchResults"] = "Invalid URL to find";
				return View("Index");
			}

			try
			{
				var result = await searchService.GetSearchResults(request);
				ViewData["SearchResults"] = string.Join(", ", result);
				return View("Index");
			}
			catch (HttpRequestException ex)
			{
				ViewData["SearchResults"] = ex.Message;
				return View("Index");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error has occurred");
				return StatusCode(500);
			}
		}
	}
}