using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GoogleSearchIntegration.DTO;
using GoogleSearchIntegration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NetCoreDemo.Pages
{
	public class IndexModel : PageModel
	{
		public IndexModel(IGoogleSearchService service)
		{
			this.service = service;
			this.SearchResults = Array.Empty<string>();
		}

		readonly IGoogleSearchService service;

		public async Task OnPostAsync()
		{
			var request = new GoogleSearchRequest
			{
				Keywords = SplitKeywords(),
				UrlToFind = URL
			};
			SearchResults = await service.GetSearchResults(request);
		}

		[BindProperty]
		[Display(Name = "Keywords")]
		[Required]
		public string Keywords { get; set; }

		[BindProperty]
		[Display(Name = "URL To Find")]
		[DataType(DataType.Url)]
		[Required]
		public string URL { get; set; }

		public string[] SearchResults { get; private set; }

		string[] SplitKeywords()
		{
			if (Keywords.Contains(','))
			{
				return Keywords.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
			}

			return Keywords.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
		}
	}
}
