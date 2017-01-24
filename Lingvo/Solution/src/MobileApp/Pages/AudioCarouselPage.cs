using Lingvo.Common.Entities;
using Lingvo.MobileApp.Controllers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Pages
{
	public class AudioCarouselPage : CarouselPage
	{
		private AudioPage lastSelectedPage;

		public AudioCarouselPage(Workbook workbook, IPage selectedPage)
		{
			Title = workbook.Title;

			//workbook.Pages.ForEach((p) => Children.Add(new AudioPage(p, workbook.Pages.Count)));
			Children.Add(new AudioPage(workbook.Pages[0], workbook.Pages.Count));

			SelectedItem = new List<ContentPage>(Children).Find((p) => ((AudioPage)p).Page.Equals(selectedPage));
			((AudioPage)SelectedItem).IsActive = true;

			lastSelectedPage = (AudioPage)SelectedItem;

			CurrentPageChanged += AudioCarouselPage_CurrentPageChanged;

			StudentPageController.Instance.SelectedPage = ((AudioPage)SelectedItem).Page;
		}

		private void AudioCarouselPage_CurrentPageChanged(object sender, System.EventArgs e)
		{
			lastSelectedPage.IsActive = false;
			((AudioPage)SelectedItem).IsActive = true;
			StudentPageController.Instance.SelectedPage = ((AudioPage)SelectedItem).Page;

			lastSelectedPage = (AudioPage)SelectedItem;
		}
	}
}

