using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Lingvo.Backend.ViewModels
{
    public class PageModel
    {
		[Required]
		public int WorkbookID { get; set; }

		public string Description { get; set; }

		[Required]
		public int PageNumber { get; set; }

		public IFormFile UploadedFile { get; set; }

		public IFormFile RecordedFile { get; set; }
	}
}
