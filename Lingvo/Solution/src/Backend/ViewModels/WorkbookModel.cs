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
			Id = workbook.Id;
			Title = workbook.Title;
			Subtitle = workbook.Subtitle;
		}

		[BindNever]
		public Workbook Workbook { get; }

		public int? Id { get; set; }

		[Required(ErrorMessage = "Es muss ein Titel angegeben werden.")]
		[Remote(action: nameof(Controllers.HomeController.UniqueWorkbookTitle), controller: "Home",
			AdditionalFields = nameof(Id),
			ErrorMessage = "Ein Arbeitsheft mit diesem Titel existiert bereits.")]
		public string Title { get; }

		public string Subtitle { get; }
	}
}
