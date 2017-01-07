using System;
using Lingvo.Common;
namespace Lingvo.Backend
{
	public class EditorController
	{
		private IRecorder recorder;
		private IPlayer player;

		private Page CurrentPage
		{
			get;
			set;
		}

		private Workbook CurrentWorkbook
		{
			get;
			set;
		}

		public EditorController()
		{
		}

		public void createPage(Workbook workbook, int pageNumber)
		{
			CurrentPage = new Page();
		}
	}
}
