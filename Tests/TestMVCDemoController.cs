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
	public class TestMVCDemoController : BaseTest
	{
		[TestCase]
		public async Task TestMVCSearchGoogle_GivenInvalidModel()
		{
			var mockService = new Mock<IGoogleSearchService>();
			var controller = new MVCDemoController(mockService.Object, LoggerFactory);
			controller.ModelState.AddModelError("error", "invalid model");
			var request = Fixture.Create<GoogleSearchRequest>();

			var result = await controller.SearchGoogle(request) as ViewResult;
			Assert.IsNotNull(result);
			Assert.AreEqual("Invalid request", result.ViewData["SearchResults"]);
		}

		[TestCase]
		public async Task TestMVCSearchGoogle_GivenHttpRequestExceptionOccurs()
		{
			var mockService = new Mock<IGoogleSearchService>();
			Fixture.Customize<GoogleSearchRequest>(x => x.With(u => u.UrlToFind, "http://www.yahoo.com"));
			var request = Fixture.Create<GoogleSearchRequest>();
			mockService.Setup(x => x.GetSearchResults(request)).ThrowsAsync(new HttpRequestException("Error"));

			var controller = new MVCDemoController(mockService.Object, LoggerFactory);

			var result = await controller.SearchGoogle(request) as ViewResult;
			Assert.IsNotNull(result);
			Assert.AreEqual("Error", result.ViewData["SearchResults"]);
		}

		[TestCase]
		public async Task TestMVCSearchGoogle_GivenOtherExceptionOccurs()
		{
			var mockService = new Mock<IGoogleSearchService>();
			var request = Fixture.Create<GoogleSearchRequest>();
			Fixture.Customize<GoogleSearchRequest>(x => x.With(u => u.UrlToFind, "http://www.yahoo.com"));
			mockService.Setup(x => x.GetSearchResults(request)).ThrowsAsync(new Exception("Error"));

			var controller = new MVCDemoController(mockService.Object, LoggerFactory);

			var result = await controller.SearchGoogle(request) as StatusCodeResult;
			Assert.IsNotNull(result);
			Assert.AreEqual(500, result.StatusCode);
		}

		[TestCase]
		public async Task TestMVCSearchGoogle_GivenInvalidURLToMatch()
		{
			var mockService = new Mock<IGoogleSearchService>();
			var request = Fixture.Create<GoogleSearchRequest>();
			request.UrlToFind = "invalid";
			var controller = new MVCDemoController(mockService.Object, LoggerFactory);

			var result = await controller.SearchGoogle(request) as ViewResult;
			Assert.IsNotNull(result);
			Assert.AreEqual("Invalid URL to find", result.ViewData["SearchResults"]);
		}
	}
}
