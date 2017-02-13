using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Lingvo.Common.Entities;

namespace Lingvo.Backend.ViewModels
{
    public class WorkbookModel
    {
		public WorkbookModel() { }

		public WorkbookModel(Workbook workbook)
		{
			Workbook = workbook;
		}

		[BindNever]
		public Workbook Workbook { get; }

		public int? Id { get; set; }

		[Required, Remote(action: nameof(Controllers.HomeController.UniqueWorkbookTitle), controller: "Home", AdditionalFields = nameof(Id))]
		public string Title { get; }

		public string Subtitle { get; }
	}
}
