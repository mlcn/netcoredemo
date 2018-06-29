using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GoogleSearchIntegration.DTO
{
	public class GoogleSearchRequest
	{
		public string[] Keywords { get; set; }

		public string UrlToFind { get; set; }

		public int NumberOfResults { get; set;  }
	}
}
