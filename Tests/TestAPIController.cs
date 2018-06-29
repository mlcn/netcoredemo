using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreDemo.Controllers;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class TestAPIController : BaseTest
	{
		[TestCase]
		public async Task TestSearchGoogle_GivenInvalidModel()
		{
			var mockService = new Mock<IGoogleSearchService>();
			var controller = new APIController(mockService.Object, LoggerFactory);
			controller.ModelState.AddModelError("error", "invalid model");
			var request = Fixture.Create<GoogleSearchRequest>();

			var result = await controller.SearchGoogle(request) as BadRequestObjectResult;
			Assert.IsNotNull(result);
		}

		[TestCase]
		public async Task TestSearchGoogle_GivenHttpRequestExceptionOccurs()
		{
			var mockService = new Mock<IGoogleSearchService>();
			Fixture.Customize<GoogleSearchRequest>(x => x.With(u => u.UrlToFind, "http://www.yahoo.com"));
			var request = Fixture.Create<GoogleSearchRequest>();
			mockService.Setup(x => x.GetSearchResults(request)).ThrowsAsync(new HttpRequestException("Error"));

			var controller = new APIController(mockService.Object, LoggerFactory);

			var result = await controller.SearchGoogle(request) as BadRequestObjectResult;
			Assert.IsNotNull(result);
			Assert.AreEqual("Error", result.Value);
		}

		[TestCase]
		public async Task TestSearchGoogle_GivenOtherExceptionOccurs()
		{
			var mockService = new Mock<IGoogleSearchService>();
			var request = Fixture.Create<GoogleSearchRequest>();
			Fixture.Customize<GoogleSearchRequest>(x => x.With(u => u.UrlToFind, "http://www.yahoo.com"));
			mockService.Setup(x => x.GetSearchResults(request)).ThrowsAsync(new Exception("Error"));

			var controller = new APIController(mockService.Object, LoggerFactory);

			var result = await controller.SearchGoogle(request) as StatusCodeResult;
			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestCase]
		public async Task TestSearchGoogle_GivenInvalidURLToMatch()
		{
			var mockService = new Mock<IGoogleSearchService>();
			var request = Fixture.Create<GoogleSearchRequest>();
			request.UrlToFind = "invalid";
			var controller = new APIController(mockService.Object, LoggerFactory);

			var result = await controller.SearchGoogle(request) as BadRequestObjectResult;
			Assert.IsNotNull(result);
			Assert.AreEqual("Invalid URL to find", result.Value);
		}
	}
}
