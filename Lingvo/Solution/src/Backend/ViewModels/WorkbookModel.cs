﻿using System.ComponentModel.DataAnnotations;
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
		public Workbook Workbook { get; set; }

		[BindNever]
		public int? Id { get; set; }

		[Required(ErrorMessage = "Es muss ein Titel angegeben werden.")]
		[Remote(action: nameof(Controllers.WorkbookController.UniqueWorkbookTitle), controller: "Workbook",
			AdditionalFields = nameof(Id),
			ErrorMessage = "Ein Arbeitsheft mit diesem Titel existiert bereits.")]
		public string Title { get; set; }

		public string Subtitle { get; set; }
	}
}
