using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreDemo.Models.MVCDemo
{
	public class MVCDemoModel
	{
		[BindProperty]
		[Display(Name = "Keywords")]
		[Required]
		public string Keywords { get; set; }

		[BindProperty]
		[Display(Name = "URL To Find")]
		[Required]
		[DataType(DataType.Url)]
		public string UrlToFind { get; set; }
	}
}
